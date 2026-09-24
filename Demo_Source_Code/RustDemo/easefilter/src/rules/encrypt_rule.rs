// (C) Copyright 2025 EaseFilter Technologies
// All Rights Reserved
//
// This software is part of a licensed software product and may
// only be used or copied in accordance with the terms of that license.
//
// NOTE:  THIS MODULE IS UNSUPPORTED SAMPLE CODE
//
// This module contains sample code provided for convenience and
// demonstration purposes only,this software is provided on an
// "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
//  either express or implied.

use super::{FilterRule, FilterRuleInstalled};
use crate::FilterController;
use crate::enums::{AccessFlag, BooleanConfig};
use crate::filter_api;
use crate::{EaseFilterErr, InvalidInput};

/// Transparent encryption rule.
///
/// Can use a static key (stored in the driver) or dynamic key
/// (provided via callback with `REQUEST_ENCRYPT_KEY_IV_AND_TAGDATA_FROM_SERVICE`).
pub struct EncryptRule {
    /// File path to encrypt (may be a glob, e.g. `"*"`).
    pub file_path: String,

    /// Static encryption key (16, 24, or 32 bytes).
    ///
    /// If `None`, requires [`BooleanConfig::REQUEST_ENCRYPT_KEY_IV_AND_TAGDATA_FROM_SERVICE`]
    /// in `boolean_config` so your callback provides the key dynamically.
    pub encryption_key: Option<Vec<u8>>,

    /// Allow/disallow flags for file access.
    ///
    /// Must include [`AccessFlag::ENABLE_FILE_ENCRYPTION_RULE`] for encryption to work.
    pub access_flag: AccessFlag,

    /// Rule-specific configuration flags.
    pub boolean_config: BooleanConfig,
}

impl Default for EncryptRule {
    fn default() -> Self {
        Self {
            file_path: String::new(),
            encryption_key: None,
            access_flag: AccessFlag::ALLOW_MAX_RIGHT_ACCESS
                | AccessFlag::ENABLE_FILE_ENCRYPTION_RULE,
            boolean_config: BooleanConfig::empty(),
        }
    }
}

impl FilterRule for EncryptRule {
    type Installed = EncryptRuleInstalled;

    fn install(self, ef: &mut FilterController) -> Result<Self::Installed, EaseFilterErr> {
        let rule_id = ef.next_rule_id();

        let EncryptRule {
            file_path,
            encryption_key,
            access_flag,
            boolean_config,
        } = self;

        if let Some(ref key) = encryption_key {
            let ln = key.len();
            if ln != 16 && ln != 24 && ln != 32 {
                return Err(EaseFilterErr::InvalidInput(
                    InvalidInput::InvalidKeyLength { got: ln },
                ));
            }
        } else if !boolean_config
            .contains(BooleanConfig::REQUEST_ENCRYPT_KEY_IV_AND_TAGDATA_FROM_SERVICE)
        {
            return Err(EaseFilterErr::InvalidInput(
                InvalidInput::EncryptionKeyRequired {
                    got: boolean_config,
                },
            ));
        }

        filter_api::add_file_filter_rule(access_flag, &file_path, false, rule_id)?;
        filter_api::add_boolean_config_to_filter_rule(&file_path, boolean_config)?;

        if let Some(ref key) = encryption_key {
            filter_api::add_encryption_key_to_filter_rule(&file_path, key)?;
        }

        Ok(EncryptRuleInstalled { file_path, rule_id })
    }
}

/// An [`EncryptRule`] that has been installed into the filter driver.
#[derive(Debug)]
pub struct EncryptRuleInstalled {
    file_path: String,
    rule_id: u32,
}

impl FilterRuleInstalled for EncryptRuleInstalled {
    fn rule_id(&self) -> u32 {
        self.rule_id
    }

    fn uninstall(self, _ef: &mut FilterController) -> Result<(), EaseFilterErr> {
        filter_api::remove_filter_rule(&self.file_path)
    }
}

#[cfg(test)]
mod tests {
    use super::*;
    use crate::enums::BooleanConfig;
    use crate::{EaseFilterErr, InvalidInput};

    #[test]
    fn encrypt_rule_rejects_short_key() {
        let mut ef = FilterController::new();
        let rule = EncryptRule {
            file_path: "C:\\test".into(),
            encryption_key: Some(vec![0u8; 15]),
            ..Default::default()
        };
        let err = FilterRule::install(rule, &mut ef).unwrap_err();
        assert!(matches!(
            err,
            EaseFilterErr::InvalidInput(InvalidInput::InvalidKeyLength { got: 15 })
        ));
    }

    #[test]
    fn encrypt_rule_rejects_long_key() {
        let mut ef = FilterController::new();
        let rule = EncryptRule {
            file_path: "C:\\test".into(),
            encryption_key: Some(vec![0u8; 33]),
            ..Default::default()
        };
        let err = FilterRule::install(rule, &mut ef).unwrap_err();
        assert!(matches!(
            err,
            EaseFilterErr::InvalidInput(InvalidInput::InvalidKeyLength { got: 33 })
        ));
    }

    #[test]
    fn encrypt_rule_requires_key_or_config() {
        let mut ef = FilterController::new();
        let rule = EncryptRule {
            file_path: "C:\\test".into(),
            encryption_key: None,
            boolean_config: BooleanConfig::empty(),
            ..Default::default()
        };
        let err = FilterRule::install(rule, &mut ef).unwrap_err();
        assert!(matches!(
            err,
            EaseFilterErr::InvalidInput(InvalidInput::EncryptionKeyRequired { .. })
        ));
    }

    #[test]
    fn encrypt_rule_required_error_shows_got() {
        let mut ef = FilterController::new();
        let rule = EncryptRule {
            file_path: "C:\\test".into(),
            encryption_key: None,
            boolean_config: BooleanConfig::ENABLE_DEFAULT_IV_TAG
                | BooleanConfig::ENABLE_NO_RECALL_FLAG,
            ..Default::default()
        };
        let err = FilterRule::install(rule, &mut ef).unwrap_err();
        let msg = err.to_string();
        assert!(
            msg.ends_with("(got: BooleanConfig(ENABLE_NO_RECALL_FLAG | ENABLE_DEFAULT_IV_TAG))"),
            "expected error to end with debug BooleanConfig, got: {msg}"
        );
    }
}
