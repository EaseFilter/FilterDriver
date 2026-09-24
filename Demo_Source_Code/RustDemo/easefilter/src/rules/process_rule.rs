use super::{FilterRule, FilterRuleInstalled};
use crate::EaseFilterErr;
use crate::FilterController;
use crate::enums::ProcessControlFlag;
use crate::filter_api;

/// Process/thread monitor and control rule.
///
/// Configure the executable mask and control flags, then call
/// [`install`](FilterRule::install) to send it to the driver.
///
/// # Examples
///
/// ```
/// # use easefilter::rules::ProcessRule;
/// # use easefilter::enums::ProcessControlFlag;
/// let rule = ProcessRule {
///     executable_mask: "notepad.exe".into(),
///     control_flag: ProcessControlFlag::PROCESS_CREATION_NOTIFICATION
///         | ProcessControlFlag::PROCESS_TERMINATION_NOTIFICATION,
/// };
/// ```
pub struct ProcessRule {
    /// Executable path to monitor (may be a glob, e.g. `"C:\\Program Files\\*"`).
    ///
    /// This must be unique among process rules, otherwise it overwrites the older rule.
    pub executable_mask: String,

    /// Process/thread event flags to subscribe to or deny.
    pub control_flag: ProcessControlFlag,
}

impl Default for ProcessRule {
    fn default() -> Self {
        Self {
            executable_mask: String::new(),
            control_flag: ProcessControlFlag::empty(),
        }
    }
}

impl FilterRule for ProcessRule {
    type Installed = ProcessRuleInstalled;

    fn install(self, ef: &mut FilterController) -> Result<Self::Installed, EaseFilterErr> {
        let rule_id = ef.next_rule_id();

        let ProcessRule {
            executable_mask,
            control_flag,
        } = self;

        filter_api::add_process_filter_rule(&executable_mask, control_flag, rule_id)?;

        Ok(ProcessRuleInstalled {
            executable_mask,
            rule_id,
        })
    }
}

/// A [`ProcessRule`] that has been installed into the filter driver.
///
/// The rule is removed from the driver when [`uninstall`](FilterRuleInstalled::uninstall)
/// is called.
pub struct ProcessRuleInstalled {
    executable_mask: String,
    rule_id: u32,
}

impl FilterRuleInstalled for ProcessRuleInstalled {
    fn rule_id(&self) -> u32 {
        self.rule_id
    }

    fn uninstall(self, _ef: &mut FilterController) -> Result<(), EaseFilterErr> {
        filter_api::remove_process_filter_rule(&self.executable_mask)
    }
}
