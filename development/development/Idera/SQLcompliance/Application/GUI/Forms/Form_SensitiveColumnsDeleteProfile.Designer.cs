namespace Idera.SQLcompliance.Application.GUI.Forms
{
    partial class Form_SensitiveColumnsDeleteProfile
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
            this.ubtnDelete = new Infragistics.Win.Misc.UltraButton();
            this.ubtnClose = new Infragistics.Win.Misc.UltraButton();
            this.ubtnDefaultSettings = new Infragistics.Win.Misc.UltraButton();
            this.ultraComboEditorProfiles = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            ((System.ComponentModel.ISupportInitialize)(this.ultraComboEditorProfiles)).BeginInit();
            this.SuspendLayout();
            // 
            // ubtnDelete
            // 
            this.ubtnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ubtnDelete.Location = new System.Drawing.Point(23, 106);
            this.ubtnDelete.Margin = new System.Windows.Forms.Padding(14);
            this.ubtnDelete.Name = "ubtnDelete";
            this.ubtnDelete.Size = new System.Drawing.Size(75, 23);
            this.ubtnDelete.TabIndex = 2;
            this.ubtnDelete.Text = "Delete";
            this.ubtnDelete.Click += new System.EventHandler(this.ubtnDelete_Click);
            // 
            // ubtnClose
            // 
            this.ubtnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ubtnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ubtnClose.Location = new System.Drawing.Point(136, 106);
            this.ubtnClose.Margin = new System.Windows.Forms.Padding(14);
            this.ubtnClose.Name = "ubtnClose";
            this.ubtnClose.Size = new System.Drawing.Size(75, 23);
            this.ubtnClose.TabIndex = 3;
            this.ubtnClose.Text = "Cancel";
            this.ubtnClose.Click += new System.EventHandler(this.ubtnClose_Click);
            // 
            // ubtnDefaultSettings
            // 
            this.ubtnDefaultSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ubtnDefaultSettings.Location = new System.Drawing.Point(23, 62);
            this.ubtnDefaultSettings.Margin = new System.Windows.Forms.Padding(14, 7, 14, 7);
            this.ubtnDefaultSettings.Name = "ubtnDefaultSettings";
            this.ubtnDefaultSettings.Size = new System.Drawing.Size(188, 23);
            this.ubtnDefaultSettings.TabIndex = 1;
            this.ubtnDefaultSettings.Text = "Restore Default Settings";
            this.ubtnDefaultSettings.Click += new System.EventHandler(this.ubtnDefaultSettings_Click);
            // 
            // ultraComboEditorProfiles
            // 
            this.ultraComboEditorProfiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraComboEditorProfiles.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.ultraComboEditorProfiles.Location = new System.Drawing.Point(23, 23);
            this.ultraComboEditorProfiles.Margin = new System.Windows.Forms.Padding(14);
            this.ultraComboEditorProfiles.Name = "ultraComboEditorProfiles";
            this.ultraComboEditorProfiles.Size = new System.Drawing.Size(188, 21);
            this.ultraComboEditorProfiles.TabIndex = 0;
            this.ultraComboEditorProfiles.ValueChanged += new System.EventHandler(this.ultraComboEditorProfiles_ValueChanged);
            // 
            // Form_SensitiveColumnsDeleteProfile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(234, 152);
            this.Controls.Add(this.ultraComboEditorProfiles);
            this.Controls.Add(this.ubtnDefaultSettings);
            this.Controls.Add(this.ubtnClose);
            this.Controls.Add(this.ubtnDelete);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(800, 190);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(250, 190);
            this.Name = "Form_SensitiveColumnsDeleteProfile";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Delete Search Profile";
            this.Load += new System.EventHandler(this.Form_SensitiveColumnsDeleteProfile_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ultraComboEditorProfiles)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Infragistics.Win.Misc.UltraButton ubtnDelete;
        private Infragistics.Win.Misc.UltraButton ubtnClose;
        private Infragistics.Win.Misc.UltraButton ubtnDefaultSettings;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor ultraComboEditorProfiles;
    }
}