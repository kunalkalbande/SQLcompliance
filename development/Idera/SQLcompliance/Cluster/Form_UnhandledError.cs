using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Idera.SQLcompliance.Cluster
{
	/// <summary>
	/// Summary description for Form_UnhandledError.
	/// </summary>
	public class Form_UnhandledError : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label _lblExplanation;
		private System.Windows.Forms.TextBox _tbError;
		private System.Windows.Forms.Button _btnClose;
		private System.Windows.Forms.Button _btnCopyToClipboard;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Form_UnhandledError(string errorString)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			_tbError.Text = errorString; 
			_tbError.SelectionLength = 0 ;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this._lblExplanation = new System.Windows.Forms.Label();
			this._tbError = new System.Windows.Forms.TextBox();
			this._btnClose = new System.Windows.Forms.Button();
			this._btnCopyToClipboard = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// _lblExplanation
			// 
			this._lblExplanation.Location = new System.Drawing.Point(8, 8);
			this._lblExplanation.Name = "_lblExplanation";
			this._lblExplanation.Size = new System.Drawing.Size(384, 48);
			this._lblExplanation.TabIndex = 0;
			this._lblExplanation.Text = "An unhandled exception has occurred in SQLcompliance Cluster Support Manager.  Us" +
				"e the button below to copy information about this exception to the clipboard.  P" +
				"lease forward this information to support@idera.com.";
			// 
			// _tbError
			// 
			this._tbError.Location = new System.Drawing.Point(8, 56);
			this._tbError.Multiline = true;
			this._tbError.Name = "_tbError";
			this._tbError.ReadOnly = true;
			this._tbError.Size = new System.Drawing.Size(384, 232);
			this._tbError.TabIndex = 1;
			this._tbError.Text = "";
			// 
			// _btnClose
			// 
			this._btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this._btnClose.Location = new System.Drawing.Point(320, 296);
			this._btnClose.Name = "_btnClose";
			this._btnClose.TabIndex = 2;
			this._btnClose.Text = "Close";
			// 
			// _btnCopyToClipboard
			// 
			this._btnCopyToClipboard.Location = new System.Drawing.Point(184, 296);
			this._btnCopyToClipboard.Name = "_btnCopyToClipboard";
			this._btnCopyToClipboard.Size = new System.Drawing.Size(120, 23);
			this._btnCopyToClipboard.TabIndex = 3;
			this._btnCopyToClipboard.Text = "Copy To Clipboard";
			this._btnCopyToClipboard.Click += new System.EventHandler(this._btnCopyToClipboard_Click);
			// 
			// Form_UnhandledError
			// 
			this.AcceptButton = this._btnClose;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this._btnClose;
			this.ClientSize = new System.Drawing.Size(400, 326);
			this.ControlBox = false;
			this.Controls.Add(this._btnCopyToClipboard);
			this.Controls.Add(this._btnClose);
			this.Controls.Add(this._tbError);
			this.Controls.Add(this._lblExplanation);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "Form_UnhandledError";
			this.ShowInTaskbar = false;
			this.Text = "Unexpected Error";
			this.ResumeLayout(false);

		}
		#endregion

		private void _btnCopyToClipboard_Click(object sender, System.EventArgs e)
		{
			Clipboard.SetDataObject(_tbError.Text, true) ;
		}
	}
}
