using System;
using System.Collections.Generic;

namespace Idera.SQLcompliance.Application.GUI.Helper
{
    using System.Windows.Forms;

    using Idera.SQLcompliance.Application.GUI.Forms;
    using Core.Templates.AuditTemplates;

    /// <summary>
    /// Selection Client Interface to update UI Controls after Registering the UI Controls
    /// </summary>
    public interface IDeselectionClient
    {
        void UpdateUiControls(bool currentControlChecked, DeselectValues deselectValues);
    }

    /// <summary>
    /// Stores Deselect Values required by Deselection Manager
    /// </summary>
    public class DeselectValues
    {
        //SQLCM-5581, 5582
        public DeselectValues()
        { }
        public DeselectValues(DeselectControls deselectControl, DeselectOptions deselectOption, EventHandler checkedPreventEventHandler = null)
        {
            this.DeselectControl = deselectControl;
            this.DeselectOption = deselectOption;
            this.CheckedPreventEventHandler = checkedPreventEventHandler;
        }

        /// <summary>
        /// Identifies the parent member associated with the control and hierarchy
        /// </summary>
        public DeselectControls DeselectControl { get; set; }

        /// <summary>
        /// Defines the deselect option at current level or other level
        /// </summary>
        public DeselectOptions DeselectOption { get; set; }

        /// <summary>
        /// Checked Prevent handler if needed to be prevented in case user cancels the selection
        /// </summary>
        public EventHandler CheckedPreventEventHandler { get; set; }
    }

    /// <summary>
    /// Handles and manages the deselection of the controls
    /// </summary>
    public class DeselectionManager
    {
        /// <summary>
        /// Defines the Update Ui Control Delegate needs to be implemented by all the controls for registering for deselection
        /// </summary>
        private readonly Action<bool, DeselectValues> updateUiControlsAction;

        /// <summary>
        /// Maps Control's name with the Deselect Values
        /// </summary>
        private readonly Dictionary<string, DeselectValues> deselectValuesMap;

        /// <summary>
        /// Maps Control Name to the Deselect Values
        /// </summary>
        private readonly Dictionary<DeselectControls, DeselectValues> deselectControlsMap;

        private const int UNDEFINED = -1;
        private Dictionary<int, RegulationSettings> _regulationSettings = null;
        private int serverRegulations;
        private int databaseRegulations;
        private int serverCombinedSettings;
        private int databaseCombinedSettings;
        private bool inDefaultAuditSettingsPage;

        public int ServerRegulations
        {
            set{ serverRegulations = value; }
        }

        public int DatabaseRegulations
        {
            set { databaseRegulations = value; }
        }

        public bool InDefaultAuditSettingsPage
        {
            set { inDefaultAuditSettingsPage = value; }
        }

        /// <summary>
        /// Initializes an instance of <see cref="DeselectionManager"/>
        /// </summary>
        /// <param name="deselectionClient">Used to update the Parent to update dependent control in case of deselection</param>
        public DeselectionManager(IDeselectionClient deselectionClient)
        {

            serverRegulations = databaseRegulations = serverCombinedSettings = databaseCombinedSettings = UNDEFINED;

            this.updateUiControlsAction = deselectionClient.UpdateUiControls;
            this.deselectValuesMap = new Dictionary<string, DeselectValues>();
            this.deselectControlsMap = new Dictionary<DeselectControls, DeselectValues>();

            this.inDefaultAuditSettingsPage = false;
        }

        public void LoadSettings()
        {
            _regulationSettings = RegulationDAL.LoadRegulationCategories(Globals.Repository.Connection);

            if (serverRegulations != UNDEFINED)
            {
                serverCombinedSettings = RegulationDAL.GetCombinedRegulationSettingsServer(_regulationSettings, serverRegulations);
            }

            if (databaseRegulations != UNDEFINED)
            {
                databaseCombinedSettings = RegulationDAL.GetCombinedRegulationSettingsDatabase(_regulationSettings, databaseRegulations);
            }
        }

        #region Handle CheckBox Controls

        /// <summary>
        /// Register Checkbox with Deselect Control updates the UI and registers the click event for further updates
        /// </summary>
        public void RegisterCheckbox(CheckBox checkBox, DeselectControls deselectControl, EventHandler suppressCheckedEventHandler= null)
        {
            if (this.deselectValuesMap.ContainsKey(checkBox.Name) || this.deselectControlsMap.ContainsKey(deselectControl))
            {
                return;
            }

            var deselectValue = new DeselectValues(deselectControl, DeselectOptions.None, suppressCheckedEventHandler);
            // Update Deselect Control Map
            this.deselectValuesMap.Add(checkBox.Name, deselectValue);
            this.deselectControlsMap.Add(deselectControl, deselectValue);
            
            // Update UI
            if (checkBox.Checked)
            {
                this.HandleCheckBoxChanged(checkBox, deselectValue);
            }

            // Register for further changes
            checkBox.Click += this.CheckBoxClick;
        }

        /// <summary>
        /// Handles Check Box Click event after registering
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBoxClick(object sender, EventArgs e)
        {
            var checkBox = sender as CheckBox;
            if (checkBox != null && this.deselectValuesMap.ContainsKey(checkBox.Name))
            {
                this.HandleCheckBoxChanged(checkBox, this.deselectValuesMap[checkBox.Name]);
            }
        }

        private int DeselectControlToRegulationCategory(DeselectControls deselectControls)
        {
            switch (deselectControls)
            {
                case DeselectControls.ServerLogins:
                    return (int)RegulationSettings.RegulationServerCategory.Logins;
                case DeselectControls.ServerLogouts:
                    return (int)RegulationSettings.RegulationServerCategory.Logouts;
                case DeselectControls.ServerFailedLogins:
                    return (int)RegulationSettings.RegulationServerCategory.FailedLogins;
                case DeselectControls.ServerAdministrativeActivities:
                    return (int)RegulationSettings.RegulationServerCategory.AdminActivity;
                case DeselectControls.ServerDatabaseDefinition:
                    return (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition;
                case DeselectControls.ServerFilterEventsPassOnly:
                    return (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess;
                case DeselectControls.ServerFilterEventsFailedOnly:
                    return (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess;
                case DeselectControls.ServerSecurityChanges:
                    return (int)RegulationSettings.RegulationServerCategory.SecurityChanges;
                case DeselectControls.ServerUserDefined:
                    return (int)RegulationSettings.RegulationServerCategory.UserDefined;
                case DeselectControls.DbDatabaseDefinition:
                    return (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition;
                case DeselectControls.DbSecurityChanges:
                    return (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges;
                case DeselectControls.DbAdministrativeActivities:
                    return (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity;
                case DeselectControls.DbDatabaseModifications:
                    return (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification;
                case DeselectControls.DbDatabaseSelect:
                    return (int)RegulationSettings.RegulationDatabaseCategory.Select;
                case DeselectControls.DbCaptureSqlDmlSelect:
                    return (int)RegulationSettings.RegulationDatabaseCategory.SQLText;
                case DeselectControls.DbCaptureSqlTransactionStatus:
                    return (int)RegulationSettings.RegulationDatabaseCategory.Transactions;
                case DeselectControls.DbFilterEventsPassOnly:
                    return (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess;
                case DeselectControls.DbFilterEventsFailedOnly:
                    return (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess;
            }
            return 0;
        }

        private bool showConfirmDialog(DeselectValues deselectValue)
        {
            switch (deselectValue.DeselectControl)
            {
                case DeselectControls.ServerLogins:
                case DeselectControls.ServerLogouts:
                case DeselectControls.ServerFailedLogins:
                case DeselectControls.ServerAdministrativeActivities:
                case DeselectControls.ServerDatabaseDefinition:
                case DeselectControls.ServerFilterEventsPassOnly:
                case DeselectControls.ServerFilterEventsFailedOnly:
                case DeselectControls.ServerSecurityChanges:
                case DeselectControls.ServerUserDefined:
                case DeselectControls.DbDatabaseDefinition:
                case DeselectControls.DbSecurityChanges:
                case DeselectControls.DbAdministrativeActivities:
                case DeselectControls.DbDatabaseModifications:
                case DeselectControls.DbDatabaseSelect:
                case DeselectControls.DbCaptureSqlDmlSelect:
                case DeselectControls.DbCaptureSqlTransactionStatus:
                case DeselectControls.DbCaptureSqlDdlSecurity:
                case DeselectControls.DbFilterEventsPassOnly:
                case DeselectControls.DbFilterEventsFailedOnly:
                    int regulationCategory = DeselectControlToRegulationCategory(deselectValue.DeselectControl);
                    if ((serverCombinedSettings & regulationCategory) == regulationCategory)
                    {
                        return true;
                    }
                    break;
                case DeselectControls.ServerFilterEvents:
                    if(((serverCombinedSettings & (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) 
                        || ((serverCombinedSettings & (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess))
                    {
                        return true;
                    }
                    break;
                case DeselectControls.DbFilterEvents:
                    if (((databaseCombinedSettings & (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess)
                        || ((databaseCombinedSettings & (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess))
                    {
                        return true;
                    }
                    break;
            }
            return false;
        }

        /// <summary>
        /// Handles all the check box changed events for deselection
        /// </summary>
        /// <param name="checkBox">CheckBox Control</param>
        /// <param name="deselectValue">Deselect Control Type and selected Options</param>
        private void HandleCheckBoxChanged(CheckBox checkBox, DeselectValues deselectValue)
        {
            // Ignore selection Cases
            if (checkBox.Checked)
            {
                deselectValue.DeselectOption = DeselectOptions.None;
                this.updateUiControlsAction(true, deselectValue);
                return;
            }

            // Deselect Success
            if (this.ShowDeselectDialog(checkBox, deselectValue))
            {
                if(!inDefaultAuditSettingsPage && showConfirmDialog(deselectValue))
                {
                    Form_ConfirmDialog form = new Form_ConfirmDialog();
                    if(form.ShowDialog() == DialogResult.Cancel)
                    {
                        checkBox.Checked = true;

                        if (deselectValue.CheckedPreventEventHandler != null)
                        {
                            checkBox.CheckedChanged += deselectValue.CheckedPreventEventHandler;
                        }

                        deselectValue.DeselectOption = DeselectOptions.None;
                        this.updateUiControlsAction(true, deselectValue);

                        return;
                    }
                }

                this.updateUiControlsAction(false, deselectValue);
                return;
            }

            if (deselectValue.CheckedPreventEventHandler != null)
            {
                checkBox.CheckedChanged -= deselectValue.CheckedPreventEventHandler;
            }

            // Deselect Canceled
            checkBox.Checked = true;

            if (deselectValue.CheckedPreventEventHandler != null)
            {
                checkBox.CheckedChanged += deselectValue.CheckedPreventEventHandler;
            }

            deselectValue.DeselectOption = DeselectOptions.None;
            this.updateUiControlsAction(true, deselectValue);
        }
        #endregion

        #region Handle Radio Button Controls

        /// <summary>
        /// Register RadioButton with Deselect Control updates the UI and registers the click event for further updates
        /// </summary>
        public void RegisterRadioButton(RadioButton radioButton, DeselectControls deselectControl, EventHandler suppressCheckedEventHandler = null)
        {
            if (this.deselectValuesMap.ContainsKey(radioButton.Name) || this.deselectControlsMap.ContainsKey(deselectControl))
            {
                return;
            }

            var deselectValue = new DeselectValues(deselectControl, DeselectOptions.None, suppressCheckedEventHandler);
            // Update Deselect Control Map
            this.deselectValuesMap.Add(radioButton.Name, deselectValue);
            this.deselectControlsMap.Add(deselectControl, deselectValue);

            // Update UI
            if (radioButton.Checked)
            {
                this.HandleRadioButtonChanged(radioButton, deselectValue);
            }

            // Register for further changes
            radioButton.Click += this.RadioButtonClick;
        }

        private void RadioButtonClick(object sender, EventArgs e)
        {
            var radioButton = sender as RadioButton;

            /*if (serverCombinedSettings != UNDEFINED || databaseCombinedSettings != UNDEFINED)
            {
                DeselectValues deselectValue = this.deselectValuesMap[radioButton.Name];
                DeselectValues temp = deselectValue;
                if (deselectValue.DeselectControl == DeselectControls.ServerFilterEventsPassOnly)
                {
                    temp = new DeselectValues(DeselectControls.ServerFilterEventsFailedOnly, DeselectOptions.None);
                }
                else if (deselectValue.DeselectControl == DeselectControls.ServerFilterEventsFailedOnly)
                {
                    temp = new DeselectValues(DeselectControls.ServerFilterEventsPassOnly, DeselectOptions.None);
                }
                else if (deselectValue.DeselectControl == DeselectControls.DbFilterEventsPassOnly)
                {
                    temp = new DeselectValues(DeselectControls.DbFilterEventsFailedOnly, DeselectOptions.None);
                }
                else if (deselectValue.DeselectControl == DeselectControls.DbFilterEventsFailedOnly)
                {
                    temp = new DeselectValues(DeselectControls.DbFilterEventsPassOnly, DeselectOptions.None);
                }

                if (showConfirmDialog(temp))
                {
                    Form_ConfirmDialog form = new Form_ConfirmDialog();
                    if (form.ShowDialog() == DialogResult.Cancel)
                    {
                        return;
                    }
                }
            }*/

            if (radioButton != null && this.deselectValuesMap.ContainsKey(radioButton.Name))
            {
                this.HandleRadioButtonChanged(radioButton, this.deselectValuesMap[radioButton.Name]);
            }

        }

        private void HandleRadioButtonChanged(RadioButton radioButton, DeselectValues deselectValue)
        {
            // Ignore unselected Cases for RadioButton
            if (!radioButton.Checked)
            {
                return;
            }
            
            this.updateUiControlsAction(true, deselectValue);
        }
        #endregion

        /// <summary>
        /// Shows the Deselection Dialog, <see cref="Form_SelectionLogicDialog"/>
        /// </summary>
        /// <param name="control">For setting result from the Checkbox</param>
        /// <param name="deselectValue">Deselect Value</param>
        /// <returns>True if dialog results updated the deselected option in the control's tag property</returns>
        private bool ShowDeselectDialog(Control control, DeselectValues deselectValue)
        {
            var propertyName = UIUtils.GetDeselectedControlText(deselectValue.DeselectControl);
            var selectionHeader = string.Format(UIUtils.DefaultSelectionLogicMessageFormat, propertyName);
            var selectionTitle = UIUtils.SelectionLogicTitle;
            string currentLevelText = null;
            string otherLevelText = null;
            switch (deselectValue.DeselectControl)
            {
                case DeselectControls.None:
                case DeselectControls.ServerLogins:
                case DeselectControls.ServerLogouts:
                case DeselectControls.ServerFailedLogins:
                case DeselectControls.ServerSecurityChanges:
                case DeselectControls.ServerDatabaseDefinition:
                case DeselectControls.ServerAdministrativeActivities:
                case DeselectControls.ServerFilterEvents:
                case DeselectControls.ServerUserDefined:
                    currentLevelText = UIUtils.DeselectCurrentLevel;
                    otherLevelText = UIUtils.DeselectServerAndAllDatabases;
                    break;
                case DeselectControls.ServerUserLogins:
                case DeselectControls.ServerUserLogouts:
                case DeselectControls.ServerUserFailedLogins:
                case DeselectControls.ServerUserAdministrativeActivities:
                case DeselectControls.ServerUserDatabaseDefinition:
                case DeselectControls.ServerUserUde:
                case DeselectControls.ServerUserSecurityChanges:
                case DeselectControls.ServerUserDatabaseModifications:
                case DeselectControls.ServerUserDatabaseSelect:
                case DeselectControls.ServerUserFilterEvents:
                case DeselectControls.ServerUserCaptureSqlDmlSelect:
                case DeselectControls.ServerUserCaptureSqlTransactionStatus:
                case DeselectControls.ServerUserCaptureSqlDdlSecurity:
                    currentLevelText = UIUtils.DeselectCurrentLevel;
                    otherLevelText = UIUtils.DeselectPrivilegeUsersDatabase;
                    break;
                case DeselectControls.DbDatabaseDefinition:
                case DeselectControls.DbSecurityChanges:
                case DeselectControls.DbAdministrativeActivities:
                case DeselectControls.DbDatabaseModifications:
                case DeselectControls.DbDatabaseSelect:
                case DeselectControls.DbFilterEvents:
                case DeselectControls.DbCaptureSqlDmlSelect:
                case DeselectControls.DbCaptureSqlTransactionStatus:
                case DeselectControls.DbCaptureSqlDdlSecurity:
                    currentLevelText = UIUtils.DeselectDatabaseLevel;
                    otherLevelText = UIUtils.DeselectPrivilegeUsersDatabase;
                    break;
            }

            var selectionLogic = new DeselectionLogic(propertyName,
                selectionHeader,
                selectionTitle,
                currentLevelText,
                DeselectOptions.CurrentLevelOnly,
                otherLevelText,
                DeselectOptions.OtherLevels);

            var frmSelectionLogicDialog = new Form_SelectionLogicDialog(selectionLogic);

            var isDeselected = DialogResult.OK == frmSelectionLogicDialog.ShowDialog();
            deselectValue.DeselectOption = DeselectOptions.None;

            if (!isDeselected)
            {
                return false;
            }

            // Deselected
            if (this.deselectValuesMap.ContainsKey(control.Name))
            {
                this.deselectValuesMap[control.Name].DeselectOption = frmSelectionLogicDialog.SelectedDeselectOptions;
            }
            return true;
        }

        /// <summary>
        /// Get Deselect Values based on the Checkbox
        /// </summary>
        public DeselectValues GetDeselectValues(CheckBox checkBox)
        {
            if (checkBox!= null && this.deselectValuesMap.ContainsKey(checkBox.Name))
            {
                return this.deselectValuesMap[checkBox.Name];
            }
            return null;
        }

        /// <summary>
        /// Gets Deselect Values for a particular member
        /// </summary>
        public DeselectValues GetDeselectValues(DeselectControls deselectControl)
        {
            if (deselectControl != DeselectControls.None && this.deselectControlsMap.ContainsKey(deselectControl))
            {
                return this.deselectControlsMap[deselectControl];
            }
            return null;
        }

        /// <summary>
        /// Get Suppressed Event for particular control
        /// </summary>
        /// <param name="deselectControl"></param>
        /// <returns></returns>
        public EventHandler GetDeselectSuppressCheckBoxChanged(DeselectControls deselectControl)
        {
            var deselectValue = GetDeselectValues(deselectControl);
            return deselectValue != null ? deselectValue.CheckedPreventEventHandler : null;
        }

        /// <summary>
        /// Get Deselect Options for particular control
        /// </summary>
        /// <returns></returns>
        public DeselectOptions GetDeselectOptions(CheckBox checkBox)
        {
            var deselectValue = GetDeselectValues(checkBox);
            return deselectValue != null ? deselectValue.DeselectOption : DeselectOptions.None;
        }

        /// <summary>
        /// Perform Grey out operation - disabled and checked
        /// </summary>
        internal static void GreyOutCheckboxControls(bool savedValue, params CheckBox[] dependentControls)
        {
            if (!savedValue) return;
            foreach (var checkBoxControl in dependentControls)
            {
                if (checkBoxControl == null)
                {
                    continue;
                }
                checkBoxControl.Enabled = false;
                checkBoxControl.Checked = true;
            }
        }

        /// <summary>
        /// Perform Grey out operation - disabled and checked
        /// </summary>
        internal static void GreyOutRadioButtonControls(bool savedValue, params RadioButton[] dependentControls)
        {
            if (!savedValue) return;
            foreach (var checkBoxControl in dependentControls)
            {
                if (checkBoxControl == null)
                {
                    continue;
                }

                checkBoxControl.Enabled = false;
                checkBoxControl.Checked = true;
            }
        }

        /// <summary>
        /// Perform disable operation - disabled and unchecked
        /// </summary>
        internal static void DisableRadioButtonControls(bool savedValue, params RadioButton[] dependentControls)
        {
            if (!savedValue) return;
            foreach (var checkBoxControl in dependentControls)
            {
                if (checkBoxControl == null)
                {
                    continue;
                }
                checkBoxControl.Enabled = false;
                checkBoxControl.Checked = false;
            }
        }

        /// <summary>
        /// Perform disable operation - disabled and set checked with passed <paramref name="savedValue"/>
        /// </summary>
        internal static void DisableSetCheckBoxControls(bool savedValue, params CheckBox[] dependentControls)
        {
            foreach (var checkBoxControl in dependentControls)
            {
                if (checkBoxControl == null)
                {
                    continue;
                }
                checkBoxControl.Enabled = false;
                checkBoxControl.Checked = savedValue;
            }
        }

        /// <summary>
        /// Additive set the checkbox, only if <paramref name="checkBoxControls"/> are not true
        /// </summary>
        /// <param name="checkedValue"></param>
        /// <param name="checkBoxControls"></param>
        internal static void SetAdditiveCheckbox(bool checkedValue, params CheckBox[] checkBoxControls)
        {
            foreach (var checkBoxControl in checkBoxControls)
            {
                if (checkBoxControl == null)
                {
                    continue;
                }
                if (!checkBoxControl.Checked && checkedValue)
                    checkBoxControl.Checked = true;
            }
        }

        /// <summary>
        /// Set and grey out the check box
        /// </summary>
        internal static void SetAndGreyOutAdditiveCheckbox(bool checkedValue, params CheckBox[] checkBoxControls)
        {
            foreach (var checkBoxControl in checkBoxControls)
            {
                if (checkBoxControl == null)
                {
                    continue;
                }
                if (!checkBoxControl.Checked && checkedValue)
                    checkBoxControl.Checked = true;
                if (checkBoxControl.Checked)
                {
                    checkBoxControl.Enabled = false;
                }
            }
        }

        /// <summary>
        /// Un-register Checkbox with Deselect Control updates the UI and registers the click event for further updates
        /// </summary>
        public void UnRegisterCheckbox(CheckBox checkBox, DeselectControls deselectControl)
        {
            if (!this.deselectValuesMap.ContainsKey(checkBox.Name) || !this.deselectControlsMap.ContainsKey(deselectControl))
            {
                return;
            }

            // Update Deselect Control Map
            this.deselectValuesMap.Remove(checkBox.Name);
            this.deselectControlsMap.Remove(deselectControl);

            checkBox.Click -= this.CheckBoxClick;
        }

        /// <summary>
        /// Un-register  RadioButton with Deselect Control updates the UI and registers the click event for further updates
        /// </summary>
        public void UnRegisterRadioButton(RadioButton radioButton, DeselectControls deselectControl)
        {
            if (!this.deselectValuesMap.ContainsKey(radioButton.Name) || !this.deselectControlsMap.ContainsKey(deselectControl))
            {
                return;
            }


            // Update Deselect Control Map
            this.deselectValuesMap.Remove(radioButton.Name);
            this.deselectControlsMap.Remove(deselectControl);

            radioButton.Click -= this.RadioButtonClick;
        }
    }
}
