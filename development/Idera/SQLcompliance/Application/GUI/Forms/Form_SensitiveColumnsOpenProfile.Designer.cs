namespace Idera.SQLcompliance.Application.GUI.Forms
{
    partial class Form_SensitiveColumnsOpenProfile
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
            this.ultraComboEditorProfiles = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.btnCancel = new Infragistics.Win.Misc.UltraButton();
            this.ubtnOpen = new Infragistics.Win.Misc.UltraButton();
            ((System.ComponentModel.ISupportInitialize)(this.ultraComboEditorProfiles)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraComboEditorProfiles
            // 
            this.ultraComboEditorProfiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraComboEditorProfiles.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.ultraComboEditorProfiles.Location = new System.Drawing.Point(23, 23);
            this.ultraComboEditorProfiles.Margin = new System.Windows.Forms.Padding(14);
            this.ultraComboEditorProfiles.Name = "ultraComboEditorProfiles";
            this.ultraComboEditorProfiles.Size = new System.Drawing.Size(228, 21);
            this.ultraComboEditorProfiles.TabIndex = 0;
            this.ultraComboEditorProfiles.ValueChanged += new System.EventHandler(this.ultraComboEditorProfiles_ValueChanged);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(151, 76);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(14);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            // 
            // ubtnOpen
            // 
            this.ubtnOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ubtnOpen.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.ubtnOpen.Location = new System.Drawing.Point(23, 76);
            this.ubtnOpen.Margin = new System.Windows.Forms.Padding(14);
            this.ubtnOpen.Name = "ubtnOpen";
            this.ubtnOpen.Size = new System.Drawing.Size(100, 23);
            this.ubtnOpen.TabIndex = 1;
            this.ubtnOpen.Text = "Open";
            this.ubtnOpen.Click += new System.EventHandler(this.ubtnOpen_Click);
            // 
            // Form_SensitiveColumnsOpenProfile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(274, 122);
            this.Controls.Add(this.ubtnOpen);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.ultraComboEditorProfiles);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(800, 160);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(280, 160);
            this.Name = "Form_SensitiveColumnsOpenProfile";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Open Search Profile";
            this.Load += new System.EventHandler(this.Form_SensitiveColumnsOpenProfile_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ultraComboEditorProfiles)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.UltraWinEditors.UltraComboEditor ultraComboEditorProfiles;
        private Infragistics.Win.Misc.UltraButton btnCancel;
        private Infragistics.Win.Misc.UltraButton ubtnOpen;
    }
}