# easefilter-rs

Idiomatic Rust bindings for the [EaseFilter](https://easefilter.com) library.
The bindings include the following features:

* Monitoring of file/folder access

* Control of file/folder access

* Filesystem-level encryption of specific folders and files

* Monitoring of the Windows Registry

* Monitoring of specific processes

The crate `easefilter-sys` has direct bindings generated using [bindgen],
and the `easefilter` crate provides a higher level interface.

[bindgen]: https://rust-lang.github.io/rust-bindgen/

> [!NOTE]
> A **license key is required** to use this program. Contact
> [info@easefilter.com](mailto:info@easefilter.com) for a trial key.

If you run `cargo check` or `cargo clippy`, always run them specifically with
`-p easefilter`, to avoid recompiling `easefilter-sys`.

## Examples

Multiple Rust examples that use the bindings are provided in the
`easefilter/examples/` folder. To run an example, use the command:

    cargo run --example <example name>

> [!NOTE]
> Run from an elevated prompt (administrator) since the EaseFilter driver
> requires it. Your license key is read from the `EASEFILTER_TEST_LICENSE_KEY`
> environment variable.

### File and Directory Monitoring

**`monitor_dir`** — Monitor a directory for file-system events (create, delete,
rename, write). Logs each event with user SID, account name, and process info.

    cargo run --example monitor_dir -- "C:\path\to\watch"

### File and Directory Access Control

**`control_static`** — Deny write and delete at the driver level via
`AccessFlag`. Operations are rejected at the driver level.

    cargo run --example control_static -- "C:\path\to\protect"

**`control_dynamic`** — Userspace callback code inspects every pre-operation
I/O event and dynamically denies renames and deletes.

    cargo run --example control_dynamic -- "C:\path\to\protect"

### Transparent Encryption

**`encrypt_static`** — Transparent encryption with a static AES-256 key derived
from a passphrase via Argon2id. The key is embedded in the rule at install time.

    cargo run --example encrypt_static -- "C:\path\to\protect" "my passphrase"

**`encrypt_callback`** — Transparent encryption where every encrypt/decrypt
request fires a userspace event to dynamically supply the key.

    cargo run --example encrypt_callback -- "C:\path\to\protect"

Files created in the protected directory will be encrypted at rest on the disk,
and decrypted whenever the filter rule is active.

### Process Monitoring

**`process_monitor`** — Monitor process/thread creation and termination events
for a given executable or glob pattern. This invocation monitors all
`notepad.exe` processes.

    cargo run --example process_monitor -- "C:\Windows\System32\notepad.exe"

### Process Control

**`process_control_static`** — Block a specific executable from launching at
the driver level. The following command prevents `notepad.exe` from starting:

    cargo run --example process_control_static -- "C:\Windows\System32\notepad.exe"

**`process_control_dynamic`** — Allow or deny process creation dynamically. The
following command prevents `cmd.exe` from starting when it has `rmdir` in its
arguments:

    cargo run --example process_control_dynamic -- "C:\Windows\System32\cmd.exe" "rmdir"

### Registry Monitoring

**`registry_monitor`** — Monitor registry key operations (create, delete, set
value, rename, open). Logs each event with the key path, user SID, and process
info. The key mask is a glob pattern.

    cargo run --example registry_monitor -- "*\EasefilterRust\*"

### Registry Access Control

**`registry_control_static`** — Deny specific registry operations at the driver
level via `RegControlFlag`. The key mask is a glob pattern.

    cargo run --example registry_control_static -- "*\EasefilterRust\*"

**`registry_control_dynamic`** — Userspace callback code inspects every
registry pre-operation event and dynamically denies keys whose path contains
`"deny"`. The key mask is a glob pattern.

    cargo run --example registry_control_dynamic -- "*\EasefilterRust\*"

