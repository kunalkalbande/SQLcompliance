using System;
using System.Windows.Forms;
using Idera.SQLcompliance.Application.GUI.Helper;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
    public partial class Form_SensitiveColumnsOpenProfile : Form
    {
        private readonly SensitiveColumnProfileHelper _helper;
        private string _activeProfile;

        public Form_SensitiveColumnsOpenProfile()
        {
            _helper = new SensitiveColumnProfileHelper();
            InitializeComponent();
        }

        private void Form_SensitiveColumnsOpenProfile_Load(object sender, EventArgs e)
        {
            _activeProfile = _helper.GetActiveProfile();
            foreach (var profile in _helper.GetProfiles())
            {
                var currentItem = ultraComboEditorProfiles.Items.Add(profile);
                if (profile == _activeProfile)
                {
                    ultraComboEditorProfiles.SelectedItem = currentItem;
                }
            }
            ubtnOpen.Enabled = false;
        }

        private void ultraComboEditorProfiles_ValueChanged(object sender, EventArgs e)
        {
            ubtnOpen.Enabled = _activeProfile != ultraComboEditorProfiles.SelectedItem.DisplayText;
        }

        private void ubtnOpen_Click(object sender, EventArgs e)
        {
            _helper.SetActiveProfile(ultraComboEditorProfiles.SelectedItem.DisplayText);
            Close();
        }
    }
}
