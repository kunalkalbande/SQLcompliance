using System;
using System.Windows.Forms;
using Idera.SQLcompliance.Application.GUI.Helper;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
    /// <summary>
    /// Helps in performing un-check action on Server or all Databases upon deselection of Server Level Properties
    /// </summary>
    public partial class Form_SelectionLogicDialog : Form
    {
        private readonly DeselectOptions currentOptionsValue;
        private readonly DeselectOptions otherOptionsValue;

        /// <summary>
        /// Initializes an instance of <see cref="Form_SelectionLogicDialog"/>
        /// </summary>
        /// <param name="deselectionLogic"></param>
        public Form_SelectionLogicDialog(DeselectionLogic deselectionLogic)
        {
            InitializeComponent();
            this.SelectedDeselectOptions = DeselectOptions.None;
            this.PropertyName = deselectionLogic.Name;
            this.Text = deselectionLogic.Title;
            this.selectionlabel.Text = deselectionLogic.Header;
            this.rbCurrentOption.Text = deselectionLogic.CurrentOption;
            this.rbOtherOption.Text = deselectionLogic.OthersOption;
            this.currentOptionsValue = deselectionLogic.CurrentOptionsValue;
            this.otherOptionsValue = deselectionLogic.OthersOptionValue;
        }

        /// <summary>
        /// Provides User selected option
        /// </summary>
        public DeselectOptions SelectedDeselectOptions { get; private set; }

        /// <summary>
        /// Property Name - like Database Definition, etc
        /// </summary>
        public string PropertyName { get; private set; }

        private void BtnSaveClick(object sender, EventArgs e)
        {
            if (this.rbCurrentOption.Checked)
            {
                this.SelectedDeselectOptions = this.currentOptionsValue;
            }
            else if (this.rbOtherOption.Checked)
            {
                this.SelectedDeselectOptions = this.otherOptionsValue;
            }
            else
            {
                this.SelectedDeselectOptions = DeselectOptions.None;
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void BtnCancelClick(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
