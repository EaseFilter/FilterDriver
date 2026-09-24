use std::{env, path::PathBuf};

fn main() {
    let manifest_dir = PathBuf::from(env::var("CARGO_MANIFEST_DIR").unwrap());
    let libdir_path = manifest_dir
        .join("..")
        .join("..")
        .join("..")
        .join("Bin")
        .join("x64");

    println!("cargo:rustc-link-search=dylib={}", libdir_path.display());
    println!("cargo:rustc-link-lib=FilterAPI");

    #[cfg(feature = "bindgen")]
    {
        let bindings_path = PathBuf::from("src").join("bindings.rs");

        if !bindings_path.exists() {
            println!("cargo:rerun-if-changed=headers/wrapper.h");
            println!("cargo:rerun-if-changed=headers/FilterAPI.h");

            bindgen::Builder::default()
                .header("headers/wrapper.h")
                .clang_arg("-fms-extensions")
                .clang_arg("-x")
                .clang_arg("c++")
                .generate()
                .expect("unable to generate bindings")
                .write_to_file(&bindings_path)
                .expect("couldn't write bindings");
        }
    }
}
