using System.Windows.Forms;
using Idera.SQLcompliance.Application.GUI.Properties ;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
	/// <summary>
	/// Summary description for UidPwdForm.
	/// </summary>
	public partial class UidPwdForm : System.Windows.Forms.Form
	{
		public UidPwdForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
         this.Icon = Resources.SQLcompliance_product_ico;

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}
    
        private string login;
        public string Login {
            get {
                return this.login;
            }
            set {
                this.login = value;
            }
        }

        private string password;

        private void btnOK_Click(object sender, System.EventArgs e) {
            if ( this.tbLogin.Text != null && !this.tbLogin.Text.Trim().Equals("") ) {
                this.login = this.tbLogin.Text.Trim();
                if ( this.tbPassword.Text != null && !this.tbPassword.Text.Trim().Equals("") ) {
                    this.password = this.tbPassword.Text.Trim();
                } else {
                    this.password = null;
                }
            } else {
                if ( MessageBox.Show("Make Login and Password blank?", "Confirm", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question) == DialogResult.Yes ) {
                    this.login = null;
                    this.password = null;
                } else {
                    this.DialogResult = DialogResult.None;
                }
            }
        }

   
        public string Password {
            get {
                return this.password;
            }
            set {
                this.password = value;
            }
        }

        public string MessageForUser {
           get { return tbUserMessage.Text; }
           set { this.tbUserMessage.Text = value; }
        }
    
    }
}
