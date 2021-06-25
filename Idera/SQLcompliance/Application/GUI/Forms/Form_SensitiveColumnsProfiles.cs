using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Idera.SQLcompliance.Application.GUI.Helper;
using Idera.SQLcompliance.Core.Stats;
using Infragistics.Win.UltraMessageBox;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
    public partial class Form_SensitiveColumnsProfiles : Form
    {
        private readonly SensitiveColumnProfileHelper _helper;
        private readonly Regex _definitionRegex;
        private string _activeProfileName;
        private bool _haveOtherProfiles;
        private bool _selectAll = true;
        private ProfilerObject _searchStringToEdit;

        public Form_SensitiveColumnsProfiles()
        {
            _helper = new SensitiveColumnProfileHelper();
            _definitionRegex = new Regex("^%[^%]+$|^[^%]+%$|^%[^%]+%$");
            InitializeComponent();
        }

        private void Form_SensitiveColumnsProfiles_Load(object sender, EventArgs e)
        {
            LoadActiveProfile();
            ultraGridProfiles.DisplayLayout.Bands[0].Columns["Category"].SortIndicator = SortIndicator.Ascending;
        }

        private void Form_SensitiveColumnsProfiles_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!HaveActiveProfile())
            {
                e.Cancel = UltraMessageBoxManager.Show(
                               "No Active Search Profile has selected, do you want to leave Configure Search? If you select Yes, no Active Search Profile will be set and you will not be able to Perform Search. Select No to remain in Configure Search to select or configure an Active Search Profile.",
                               "SQL Compliance Manager",
                               MessageBoxButtons.YesNo,
                               MessageBoxIcon.Warning,
                               MessageBoxDefaultButton.Button2) == DialogResult.No;
            } else if (CheckChanges())
            {
                e.Cancel = !PerformCancel();
            }
        }

        private void LoadActiveProfile()
        {
            ultraDataSourceProfiles.Rows.Clear();
            _activeProfileName = _helper.GetActiveProfile();
            uteActiveSearchProfile.Text = HaveActiveProfile()
                ? _activeProfileName
                : "None Selected";

            var activeProfileDetails = _helper.GetActiveProfileDetails();

            foreach (var profileDetail in activeProfileDetails)
            {
                ultraDataSourceProfiles.Rows.Add(true, new object[]
                {
                    profileDetail.Category,
                    profileDetail.SearchStringName,
                    profileDetail.Definition,
                    profileDetail.IsStringChecked,
                    profileDetail.IsStringChecked
                });
            }

            UpdateProfilesState();
        }

        private void ubtnClose_Click(object sender, EventArgs e)
        {
            if (!CheckChanges() || !HaveActiveProfile())
            {
                Close();
                return;
            }
            if (PerformCancel())
            {
                UpdateButtonState();
            }
        }

        private void ubtnSaveProfile_Click(object sender, EventArgs e)
        {
            if (HaveActiveProfile())
            {
                if (!CheckSelection())
                {
                    UltraMessageBoxManager.Show(
                        string.Format(
                            "Active profile cannot be empty. Select at least one record to Save profile '{0}'.",
                            _activeProfileName),
                        "SQL Compliance Manager",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }
                if (UltraMessageBoxManager.Show(
                        string.Format(
                            "Changes will be saved to '{0}'. Yes to confirm and return to SQL Conlumn Search. Select No to remain in Configuration Search.",
                            _activeProfileName),
                        "SQL Compliance Manager",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning,
                        MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    _helper.UpdateProfile(GetProfileFromGrid());
                    Close();
                }
            }
        }

        private void ubtnSaveNewProfile_Click(object sender, EventArgs e)
        {
            using (var saveNewProfileForm = new Form_SensitiveColumnsNewProfile(GetProfileFromGrid()))
            {
                if (saveNewProfileForm.ShowDialog(this) == DialogResult.OK)
                {
                    LoadActiveProfile();
                }
            }
        }

        private void ubtnDeleteProfile_Click(object sender, EventArgs e)
        {
            using (var deleteForm = new Form_SensitiveColumnsDeleteProfile())
            {
                if (deleteForm.ShowDialog(this) == DialogResult.OK)
                {
                    LoadActiveProfile();
                }
            }
        }

        private void ubtnOpenProfile_Click(object sender, EventArgs e)
        {
            using (var openProfileForm = new Form_SensitiveColumnsOpenProfile())
            {
                if (openProfileForm.ShowDialog(this) == DialogResult.OK)
                {
                    LoadActiveProfile();
                }
            }
        }

        private void ubtnNewString_Click(object sender, EventArgs e)
        {
            InitializeEditStringPanel(false);
        }

        private void ultraGridProfiles_CellChange(object sender, CellEventArgs e)
        {
            if (e.Cell.Column.DataType == typeof(bool))
            {
                e.Cell.Row.Update();
            }
        }

        private void ultraGridProfiles_AfterCellUpdate(object sender, CellEventArgs e)
        {
            UpdateButtonState();
        }

        private void ultraGridProfiles_AfterRowActivate(object sender, EventArgs e)
        {
            UpdateStringState();
        }

        private List<ProfilerObject> GetProfileFromGrid()
        {
            var profileItems = new List<ProfilerObject>(ultraDataSourceProfiles.Rows.Count);
            foreach (UltraDataRow profileRow in ultraDataSourceProfiles.Rows)
            {
                profileItems.Add(new ProfilerObject()
                {
                    Category = (string)profileRow.GetCellValue("Category"),
                    SearchStringName = (string)profileRow.GetCellValue("Search String Name"),
                    Definition = (string)profileRow.GetCellValue("Definition"),
                    IsStringChecked = (bool)profileRow.GetCellValue("Select"),
                    ProfileName = _activeProfileName
                });
            }

            return profileItems;
        }

        private void llToggleSelection_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            foreach (UltraDataRow profileRow in ultraDataSourceProfiles.Rows)
            {
                profileRow.SetCellValue("Select", SelectAll);
            }

            SelectAll = !SelectAll;
            UpdateButtonState();
        }

        private ProfilerObject GetSelectedSearchRow()
        {
            return new ProfilerObject()
            {
                Category = (string)ultraGridProfiles.ActiveRow.GetCellValue("Category"),
                SearchStringName = (string)ultraGridProfiles.ActiveRow.GetCellValue("Search String Name"),
                Definition = (string)ultraGridProfiles.ActiveRow.GetCellValue("Definition"),
            };
        }

        private ProfilerObject GetSearchStringFromEditor()
        {
            return new ProfilerObject()
            {
                Category = ultraComboEditorCategory.Text ?? ultraComboEditorCategory.SelectedItem.DisplayText,
                SearchStringName = ultraTextEditorSearchStringName.Text,
                Definition = ultraTextEditorDefinition.Text
            };
        }

        private void DiscardEditStringPanel()
        {
            _searchStringToEdit = null;

            ultraComboEditorCategory.Clear();
            ultraComboEditorCategory.Items.Clear();
            ultraTextEditorSearchStringName.Clear();
            ultraTextEditorDefinition.Clear();

            ProfileMode = true;
        }

        private void InitializeEditStringPanel(bool forEdit)
        {
            DiscardEditStringPanel();

            var categories = GetProfileFromGrid().Select(po => po.Category).Distinct().OrderBy(c => c).ToList();

            foreach (var category in categories)
            {
                ultraComboEditorCategory.Items.Add(category);
            }

            if (forEdit)
            {
                _searchStringToEdit = GetSelectedSearchRow();

                ultraComboEditorCategory.SelectedIndex =
                    categories.IndexOf(_searchStringToEdit.Category);
                ultraTextEditorSearchStringName.Text = _searchStringToEdit.SearchStringName;
                ultraTextEditorDefinition.Text = _searchStringToEdit.Definition;
            }

            ugbString.Text = forEdit
                ? "Edit Search String Configuration"
                : "New Search String Configuration";

            ProfileMode = false;
        }

        private void ubtnEditString_Click(object sender, EventArgs e)
        {
            InitializeEditStringPanel(true);
        }

        private void ubtnSaveString_Click(object sender, EventArgs e)
        {
            _helper.SaveOrUpdateSearchString(GetSearchStringFromEditor(), _searchStringToEdit);
            DiscardEditStringPanel();
            LoadActiveProfile();
        }

        private void ubtnDeleteString_Click(object sender, EventArgs e)
        {

            if (UltraMessageBoxManager.Show(
                    "Selected search string record will be removed. Are you sure?",
                    "SQL Compliance Manager",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                _helper.DeleteSearchString(GetSelectedSearchRow());
                LoadActiveProfile();
            }
        }

        private void ubtnDiscardString_Click(object sender, EventArgs e)
        {
            DiscardEditStringPanel();
        }

        private void ultraTextEditorDefinition_TextChanged(object sender, EventArgs e)
        {
            UpdateStringState();
        }

        private void ultraTextEditorSearchStringName_TextChanged(object sender, EventArgs e)
        {
            UpdateStringState();
        }

        private void ultraComboEditorCategory_TextChanged(object sender, EventArgs e)
        {
            UpdateStringState();
        }

        public bool ProfileMode
        {
            get { return ultraGridProfiles.Visible; }
            set
            {
                ultraPanelEditString.Dock = DockStyle.Fill;
                ultraPanelEditString.Visible = !value;
                ultraGridProfiles.Dock = DockStyle.Fill;
                ultraGridProfiles.Visible = value;
                UpdateButtonState();
            }
        }

        private bool SelectAll
        {
            get { return _selectAll; }
            set
            {
                _selectAll = value;
                llToggleSelection.Text = _selectAll ? "Select All" : "Deselect All";
            }
        }

        private void UpdateProfilesState()
        {
            _haveOtherProfiles = _helper.GetProfiles()
                                     .Count(profile => profile != _activeProfileName) > 0;
            UpdateButtonState();
        }

        private bool CheckChanges()
        {
            foreach (UltraDataRow profileRow in ultraDataSourceProfiles.Rows)
            {
                if ((bool)profileRow.GetCellValue("Select")
                    != (bool)profileRow.GetCellValue("OriginalSelect"))
                {
                    return true;
                }
            }
            return false;
        }

        private bool HaveActiveProfile()
        {
            return !string.IsNullOrWhiteSpace(_activeProfileName);
        }

        private bool CheckSelection()
        {
            return GetProfileFromGrid()
                .Any(profileItem => profileItem.IsStringChecked);
        }

        private bool CheckIfAllProfilesSelected()
        {
            return GetProfileFromGrid()
                .All(profileItem => profileItem.IsStringChecked);
        }
        
        private void UpdateButtonState()
        {
            var checkSelection = CheckSelection();
            llToggleSelection.Enabled = ProfileMode;
            ubtnClose.Enabled = ProfileMode;
            ubtnDeleteProfile.Enabled = ProfileMode && (_haveOtherProfiles || HaveActiveProfile());
            ubtnOpenProfile.Enabled = ProfileMode && _haveOtherProfiles;
            ubtnSaveNewProfile.Enabled = ProfileMode && checkSelection;
            ubtnSaveProfile.Enabled = ProfileMode && HaveActiveProfile() && CheckChanges();

            if (CheckIfAllProfilesSelected())
            {
                SelectAll = false;
            }
            else if (!checkSelection)
            {
                SelectAll = true;
            }

            ubtnClose.Text = HaveActiveProfile() && CheckChanges() ? "Cancel" : "Close";
            UpdateStringState();
        }

        private void UpdateStringState()
        {
            ubtnEditString.Enabled = ProfileMode && ultraGridProfiles.ActiveRow != null;
            ubtnNewString.Enabled = ProfileMode;
            ubtnDeleteString.Enabled = ProfileMode && ultraGridProfiles.ActiveRow != null;
            ubtnSaveString.Enabled = CanSaveString();
            ubtnDiscardString.Enabled = !ProfileMode;
        }

        private bool CanSaveString()
        {
            if (ProfileMode)
            {
                return false;
            }

            var newString = GetSearchStringFromEditor();

            if (!IsDefinitionValid(newString.Definition)
                || string.IsNullOrWhiteSpace(newString.Category)
                || string.IsNullOrWhiteSpace(newString.SearchStringName))
            {
                return false;
            }

            if (_searchStringToEdit != null
                && _searchStringToEdit.Category == newString.Category
                && _searchStringToEdit.Definition == newString.Definition
                && _searchStringToEdit.SearchStringName == newString.SearchStringName)
            {
                return false;
            }

            return true;
        }

        private bool IsDefinitionValid(string definition)
        {
            var needShowError = !string.IsNullOrWhiteSpace(definition) &&
                                definition.Split(',').Any(subDefinition => !_definitionRegex.IsMatch(subDefinition));
            var isValid = !string.IsNullOrWhiteSpace(definition) || needShowError;
            errorProvider.SetError(ultraTextEditorDefinition,
                !needShowError ? null : "Definition must have leading '%' or trailing '%' or both leading and trailing '%'");
            return isValid;
        }

        private bool PerformCancel()
        {
            if (UltraMessageBoxManager.Show(
                    "Do you want to cancel all the changes since the last save?",
                    "SQL Compliance Manager",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                foreach (UltraDataRow profileRow in ultraDataSourceProfiles.Rows)
                {
                    profileRow.SetCellValue("Select", profileRow.GetCellValue("OriginalSelect"));
                }
                return true;
            }
            return false;
        }

        private void llHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            HelpAlias.ShowHelp(this, HelpAlias.SSHELP_SensitiveColumnSearch);
        }
    }
}
