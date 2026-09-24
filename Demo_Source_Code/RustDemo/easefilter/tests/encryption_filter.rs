mod common;

use std::fs;
use std::sync::{
    Arc, Mutex,
    atomic::{AtomicBool, Ordering},
};
use std::time::Duration;

use common::{RULE_ACTIVATION_DELAY, require_admin, temp_dir, wait_until_true};
use easefilter::FilterController;
use easefilter::enums::{AccessFlag, BooleanConfig, EncryptEventType, FilterType};
use easefilter::events::{DenyReply, EncryptReply, Event, Reply};
use easefilter::rules::{EncryptRule, FilterRule, FilterRuleInstalled};
use serial_test::serial;

#[test]
#[serial]
fn encryption_static_key() {
    require_admin();

    let tmp = temp_dir("ef_test_enc_static");

    let mut ef = FilterController::new();
    ef.on_message(|_: &Event, _: bool| None::<Box<dyn Reply>>);
    ef.start(common::TEST_LICENSE_KEY, FilterType::ENCRYPTION)
        .unwrap();

    let test_file = tmp.join("secret.txt");
    let plaintext = b"lorem ipsum dolor sit amet";

    let key = b"0123456789abcdef";

    let rule = EncryptRule {
        file_path: test_file.to_str().unwrap().to_string(),
        encryption_key: Some(key.to_vec()),
        ..EncryptRule::default()
    };
    let installed = rule.install(&mut ef).unwrap();

    std::thread::sleep(RULE_ACTIVATION_DELAY);

    // Write and read back -- transparent encryption/decryption
    fs::write(&test_file, plaintext).unwrap();
    let read_back = fs::read(&test_file).unwrap();
    assert_eq!(read_back, plaintext);

    // Uninstall the rule; file on disk is now raw encrypted bytes
    installed.uninstall(&mut ef).unwrap();

    let encrypted = fs::read(&test_file).unwrap();
    assert_ne!(encrypted, plaintext, "uninstalled file should be encrypted");

    // Reinstall with ALLOW_READ_ENCRYPTED_FILES removed; reading returns ciphertext
    let rule2 = EncryptRule {
        file_path: test_file.to_str().unwrap().to_string(),
        encryption_key: Some(key.to_vec()),
        access_flag: AccessFlag::ALLOW_MAX_RIGHT_ACCESS & !AccessFlag::ALLOW_READ_ENCRYPTED_FILES,
        ..EncryptRule::default()
    };
    let installed2 = rule2.install(&mut ef).unwrap();

    std::thread::sleep(RULE_ACTIVATION_DELAY);

    let still_encrypted = fs::read(&test_file).unwrap();
    assert_eq!(
        still_encrypted, encrypted,
        "without ALLOW_READ_ENCRYPTED_FILES, read should return raw ciphertext"
    );

    installed2.uninstall(&mut ef).unwrap();
    ef.stop();
    let _ = fs::remove_dir_all(&tmp);
}

#[test]
#[serial]
fn encryption_callback() {
    require_admin();

    let tmp = temp_dir("ef_test_enc_callback");

    let enc_folder = tmp.join("encrypted");
    fs::create_dir(&enc_folder).unwrap();
    let file1 = enc_folder.join("good.txt");
    let file2 = enc_folder.join("bad.txt");

    let enc_key: Vec<u8> = b"1234567890abcdef1234567890abcdef".to_vec();
    let iv: [u8; 16] = *b"1234567890abcdef";

    let denied = Arc::new(AtomicBool::new(false));
    let denied_cb = denied.clone();
    let got_encrypt_event = Arc::new(AtomicBool::new(false));
    let got_encrypt_event_cb = got_encrypt_event.clone();
    let callback_file = Arc::new(Mutex::new(None::<String>));
    let callback_file_cb = callback_file.clone();
    let file2_cb = file2.clone();

    let mut ef = FilterController::new();
    ef.on_message(move |event: &Event, _can_reply: bool| {
        let Event::Encrypt(enc) = event else {
            return None::<Box<dyn Reply>>;
        };
        got_encrypt_event_cb.store(true, Ordering::SeqCst);
        *callback_file_cb.lock().unwrap() = Some(enc.file_name.to_string_lossy().to_string());

        if enc.file_name == file2_cb {
            denied_cb.store(true, Ordering::SeqCst);
            return Some(Box::new(DenyReply));
        }

        Some(Box::new(EncryptReply {
            encryption_key: enc_key.clone(),
            iv: Some(iv),
            tag_data: None,
        }))
    });
    ef.start(common::TEST_LICENSE_KEY, FilterType::ENCRYPTION)
        .unwrap();

    let pattern = format!(r"{}\*", enc_folder.to_str().unwrap());
    let rule = EncryptRule {
        file_path: pattern,
        encryption_key: None,
        boolean_config: BooleanConfig::REQUEST_ENCRYPT_KEY_IV_AND_TAGDATA_FROM_SERVICE,
        ..EncryptRule::default()
    };
    let installed = rule.install(&mut ef).unwrap();

    std::thread::sleep(RULE_ACTIVATION_DELAY);

    let test_data = b"lorem ipsum dolor sit amet";
    fs::write(&file1, test_data).unwrap();
    let read_back = fs::read(&file1).unwrap();
    assert_eq!(read_back, test_data);

    assert!(
        wait_until_true(Duration::from_secs(3), || got_encrypt_event
            .load(Ordering::SeqCst)),
        "encryption event did not fire for file1"
    );

    // Writing file2 should be denied by the callback
    let result = fs::write(&file2, test_data);
    assert!(
        result.is_err(),
        "file2 write should be denied, got {:?}",
        result
    );

    assert!(
        denied.load(Ordering::SeqCst),
        "DenyReply should have been returned for file2"
    );

    installed.uninstall(&mut ef).unwrap();
    ef.stop();
    let _ = fs::remove_dir_all(&tmp);
}

#[test]
#[serial]
fn encryption_tag_data() {
    require_admin();

    let tmp = temp_dir("ef_test_enc_tag");

    let enc_folder = tmp.join("encrypted");
    fs::create_dir(&enc_folder).unwrap();

    let enc_key: Vec<u8> = b"1234567890abcdef".to_vec();

    let found_tag = Arc::new(AtomicBool::new(false));
    let found_tag_cb = found_tag.clone();
    let bad_tag = Arc::new(AtomicBool::new(false));
    let bad_tag_cb = bad_tag.clone();
    let tag_event = Arc::new(AtomicBool::new(false));
    let tag_event_cb = tag_event.clone();

    let mut ef = FilterController::new();
    ef.on_message(move |event: &Event, _can_reply: bool| {
        let Event::Encrypt(enc) = event else {
            return None::<Box<dyn Reply>>;
        };
        tag_event_cb.store(true, Ordering::SeqCst);

        if matches!(enc.kind, EncryptEventType::RequestIvAndKey)
            && let Some(ref tag) = enc.tag_data
        {
            found_tag_cb.store(true, Ordering::SeqCst);
            let file_name = enc.file_name.to_string_lossy();
            if tag.as_slice() != file_name.as_bytes() {
                bad_tag_cb.store(true, Ordering::SeqCst);
            }
        }

        let tag = enc.file_name.to_string_lossy().to_string().into_bytes();
        Some(Box::new(EncryptReply {
            encryption_key: enc_key.clone(),
            iv: None,
            tag_data: Some(tag),
        }))
    });
    ef.start(common::TEST_LICENSE_KEY, FilterType::ENCRYPTION)
        .unwrap();

    let pattern = format!(r"{}\*", enc_folder.to_str().unwrap());
    let rule = EncryptRule {
        file_path: pattern,
        encryption_key: None,
        boolean_config: BooleanConfig::REQUEST_ENCRYPT_KEY_IV_AND_TAGDATA_FROM_SERVICE,
        ..EncryptRule::default()
    };
    let installed = rule.install(&mut ef).unwrap();

    std::thread::sleep(RULE_ACTIVATION_DELAY);

    let name = "file.txt";
    let f = enc_folder.join(name);
    let test_text = b"lorem ipsum";
    fs::write(&f, test_text).unwrap();
    assert_eq!(fs::read(&f).unwrap(), test_text);

    assert!(
        wait_until_true(Duration::from_secs(3), || tag_event.load(Ordering::SeqCst)),
        "no encryption event fired"
    );
    assert!(
        found_tag.load(Ordering::SeqCst),
        "tag_data should have been found for RequestIvAndKey"
    );
    assert!(
        !bad_tag.load(Ordering::SeqCst),
        "tag_data should match the file name"
    );

    installed.uninstall(&mut ef).unwrap();
    ef.stop();
    let _ = fs::remove_dir_all(&tmp);
}

#[test]
#[serial]
fn encrypted_data_is_mostly_binary() {
    require_admin();

    let tmp = temp_dir("ef_test_enc_binary");

    let mut ef = FilterController::new();
    ef.on_message(|_: &Event, _: bool| None::<Box<dyn Reply>>);
    ef.start(common::TEST_LICENSE_KEY, FilterType::ENCRYPTION)
        .unwrap();

    let test_file = tmp.join("test.txt");
    let plaintext =
        b"Hello World! This is a test file with enough data to make encryption meaningful. ";
    let long_plaintext: Vec<u8> = plaintext.iter().copied().cycle().take(1024).collect();

    let key = b"0123456789abcdef";

    let rule = EncryptRule {
        file_path: test_file.to_str().unwrap().to_string(),
        encryption_key: Some(key.to_vec()),
        ..EncryptRule::default()
    };
    let installed = rule.install(&mut ef).unwrap();

    std::thread::sleep(RULE_ACTIVATION_DELAY);

    fs::write(&test_file, &long_plaintext).unwrap();
    assert_eq!(fs::read(&test_file).unwrap(), long_plaintext);

    // Uninstall the rule and read raw encrypted bytes
    installed.uninstall(&mut ef).unwrap();

    let encrypted = fs::read(&test_file).unwrap();
    assert_ne!(encrypted, long_plaintext);

    // Count printable ASCII bytes (0x20-0x7E)
    let printable = encrypted
        .iter()
        .filter(|&&b| b.is_ascii_graphic() || b == b' ')
        .count();
    let ratio = printable as f64 / encrypted.len() as f64;

    // AES ciphertext is pseudorandom; ~37% of bytes fall in printable range by chance.
    // If the data were plaintext, ratio would be near 100%.
    assert!(
        ratio < 0.45,
        "encrypted data should be mostly non-printable; \
         {printable}/{len} bytes ({ratio:.1}%) are printable",
        len = encrypted.len()
    );

    ef.stop();
    let _ = fs::remove_dir_all(&tmp);
}
