using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Idera.SQLcompliance.Application.GUI.Helper;
using Idera.SQLcompliance.Core.Stats;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
    public partial class Form_SensitiveColumnsNewProfile : Form
    {
        private readonly SensitiveColumnProfileHelper _helper;
        private readonly IEnumerable<ProfilerObject> _searchStrings;

        public Form_SensitiveColumnsNewProfile(IEnumerable<ProfilerObject> searchStrings)
        {
            _searchStrings = searchStrings;
            _helper = new SensitiveColumnProfileHelper();
            InitializeComponent();
        }

        private void ubtnClose_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void ubtnSave_Click(object sender, EventArgs e)
        {
            if (!IsProfileNameExists())
            {
                _helper.CreateNewProfile(uteProfileName.Text, _searchStrings);
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void uteProfileName_ValueChanged(object sender, EventArgs e)
        {
            ubtnSave.Enabled = !string.IsNullOrWhiteSpace(uteProfileName.Text)
                && !IsProfileNameExists();
            errorProvider.SetError(uteProfileName,
                ubtnSave.Enabled ? null : "This profile name already exists");
        }

        private bool IsProfileNameExists()
        {
            return _helper.GetProfiles()
                .Any(p => uteProfileName.Text.Equals(p, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
