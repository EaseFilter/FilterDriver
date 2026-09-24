use super::{FilterRule, FilterRuleInstalled};
use crate::EaseFilterErr;
use crate::FilterController;
use crate::enums::{RegCallbackClass, RegControlFlag};
use crate::filter_api;

/// Registry monitor and control rule.
///
/// Configure the registry key mask, process name, callback class, and access
/// flags, then call [`install`](FilterRule::install) to send it to the driver.
///
/// # Examples
///
/// ```
/// # use easefilter::rules::RegistryRule;
/// # use easefilter::enums::{RegCallbackClass, RegControlFlag};
/// let rule = RegistryRule {
///     key_mask: "*\\Software\\MyApp\\*".into(),
///     proc_name: "*".into(),
///     callback_class: RegCallbackClass::REG_PRE_CREATE_KEY
///         | RegCallbackClass::REG_PRE_DELETE_KEY,
///     access_flag: RegControlFlag::REG_MAX_ACCESS_FLAG,
///     exclude_filter: false,
/// };
/// ```
pub struct RegistryRule {
    /// Registry key path pattern (may contain `*` wildcards).
    pub key_mask: String,

    /// Executable path to match (may be a glob, e.g. `"C:\\Program Files\\*"`).
    ///
    /// Use `"*"` to match all processes.
    pub proc_name: String,

    /// Registry callback class flags to subscribe to.
    pub callback_class: RegCallbackClass,

    /// Registry access permission flags.
    pub access_flag: RegControlFlag,

    /// If true, excludes matching events instead of including them.
    pub exclude_filter: bool,
}

impl FilterRule for RegistryRule {
    type Installed = RegistryRuleInstalled;

    fn install(self, ef: &mut FilterController) -> Result<Self::Installed, EaseFilterErr> {
        let rule_id = ef.next_rule_id();

        let RegistryRule {
            key_mask,
            proc_name,
            callback_class,
            access_flag,
            exclude_filter,
        } = self;

        filter_api::add_registry_filter_rule(
            &key_mask,
            &proc_name,
            callback_class,
            access_flag,
            exclude_filter,
            rule_id,
        )?;

        Ok(RegistryRuleInstalled { proc_name, rule_id })
    }
}

/// A [`RegistryRule`] that has been installed into the filter driver.
///
/// The rule is removed from the driver when [`uninstall`](FilterRuleInstalled::uninstall)
/// is called.
pub struct RegistryRuleInstalled {
    proc_name: String,
    rule_id: u32,
}

impl FilterRuleInstalled for RegistryRuleInstalled {
    fn rule_id(&self) -> u32 {
        self.rule_id
    }

    fn uninstall(self, _ef: &mut FilterController) -> Result<(), EaseFilterErr> {
        filter_api::remove_registry_filter_rule_by_process_name(&self.proc_name)
    }
}
