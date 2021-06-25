using System;
using System.Windows.Forms;
using Idera.SQLcompliance.Application.GUI.Helper;
using Infragistics.Win.UltraMessageBox;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
    public partial class Form_SensitiveColumnsDeleteProfile : Form
    {
        private readonly SensitiveColumnProfileHelper _helper;
        public Form_SensitiveColumnsDeleteProfile()
        {
            _helper = new SensitiveColumnProfileHelper();
            InitializeComponent();
        }

        private void Form_SensitiveColumnsDeleteProfile_Load(object sender, EventArgs e)
        {
            UpdateProfilesList();
        }

        private void UpdateProfilesList()
        {
            ultraComboEditorProfiles.Items.Clear();
            ultraComboEditorProfiles.Clear();
            var activeProfile = _helper.GetActiveProfile();
            foreach (var profileName in _helper.GetProfiles())
            {
                ultraComboEditorProfiles.Items.Add(profileName);
                if (profileName == activeProfile)
                {
                    ultraComboEditorProfiles.SelectedIndex = ultraComboEditorProfiles.Items.Count - 1;
                }
            }
            ubtnDelete.Enabled = ultraComboEditorProfiles.SelectedItem != null;
            ubtnDefaultSettings.Enabled = ultraComboEditorProfiles.Items.Count > 0;
        }

        private void ubtnDelete_Click(object sender, EventArgs e)
        {
            var profileToDelete = ultraComboEditorProfiles.SelectedItem.DisplayText;
            if (UltraMessageBoxManager.Show(
                    string.Format("'{0}' profile will be deleted. Are you sure?", profileToDelete),
                    "SQL Compliance Manager",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                _helper.DeleteProfile(profileToDelete);
                UpdateProfilesList();
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void ubtnClose_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void ubtnDefaultSettings_Click(object sender, EventArgs e)
        {
            if (UltraMessageBoxManager.Show(
                    "All user created profiles will be deleted. Are you sure?",
                    "SQL Compliance Manager",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                _helper.DeleteAllProfiles();
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void ultraComboEditorProfiles_ValueChanged(object sender, EventArgs e)
        {
            ubtnDelete.Enabled = ultraComboEditorProfiles.SelectedItem != null;
        }
    }
}
