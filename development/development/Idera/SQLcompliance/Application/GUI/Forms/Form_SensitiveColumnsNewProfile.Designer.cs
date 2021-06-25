namespace Idera.SQLcompliance.Application.GUI.Forms
{
    partial class Form_SensitiveColumnsNewProfile
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.uteProfileName = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ubtnClose = new Infragistics.Win.Misc.UltraButton();
            this.ubtnSave = new Infragistics.Win.Misc.UltraButton();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.uteProfileName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // uteProfileName
            // 
            this.uteProfileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.errorProvider.SetIconPadding(this.uteProfileName, -18);
            this.uteProfileName.Location = new System.Drawing.Point(23, 23);
            this.uteProfileName.Margin = new System.Windows.Forms.Padding(14);
            this.uteProfileName.Name = "uteProfileName";
            this.uteProfileName.Size = new System.Drawing.Size(219, 21);
            this.uteProfileName.TabIndex = 0;
            this.uteProfileName.ValueChanged += new System.EventHandler(this.uteProfileName_ValueChanged);
            // 
            // ubtnClose
            // 
            this.ubtnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ubtnClose.Location = new System.Drawing.Point(151, 76);
            this.ubtnClose.Margin = new System.Windows.Forms.Padding(14);
            this.ubtnClose.Name = "ubtnClose";
            this.ubtnClose.Size = new System.Drawing.Size(91, 23);
            this.ubtnClose.TabIndex = 2;
            this.ubtnClose.Text = "Cancel";
            this.ubtnClose.Click += new System.EventHandler(this.ubtnClose_Click);
            // 
            // ubtnSave
            // 
            this.ubtnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ubtnSave.Enabled = false;
            this.ubtnSave.Location = new System.Drawing.Point(23, 76);
            this.ubtnSave.Margin = new System.Windows.Forms.Padding(14);
            this.ubtnSave.Name = "ubtnSave";
            this.ubtnSave.Size = new System.Drawing.Size(91, 23);
            this.ubtnSave.TabIndex = 1;
            this.ubtnSave.Text = "Save";
            this.ubtnSave.Click += new System.EventHandler(this.ubtnSave_Click);
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // Form_SensitiveColumnsNewProfile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(264, 122);
            this.Controls.Add(this.ubtnSave);
            this.Controls.Add(this.ubtnClose);
            this.Controls.Add(this.uteProfileName);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(800, 160);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(280, 160);
            this.Name = "Form_SensitiveColumnsNewProfile";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Save New Search Profile";
            ((System.ComponentModel.ISupportInitialize)(this.uteProfileName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.UltraWinEditors.UltraTextEditor uteProfileName;
        private Infragistics.Win.Misc.UltraButton ubtnClose;
        private Infragistics.Win.Misc.UltraButton ubtnSave;
        private System.Windows.Forms.ErrorProvider errorProvider;
    }
}