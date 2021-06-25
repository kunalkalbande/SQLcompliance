namespace Idera.SQLcompliance.Application.GUI.Forms
{
    partial class Form_SaveFileDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_SaveFileDialog));
            this.rbApplyRegulation = new System.Windows.Forms.RadioButton();
            this.rbApplySaveRegulation = new System.Windows.Forms.RadioButton();
            this.lblCustomTemplate = new System.Windows.Forms.Label();
            this.txtBoxCTName = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // rbApplyRegulation
            // 
            this.rbApplyRegulation.AutoSize = true;
            this.rbApplyRegulation.Checked = true;
            this.rbApplyRegulation.Location = new System.Drawing.Point(40, 50);
            this.rbApplyRegulation.Name = "rbApplyRegulation";
            this.rbApplyRegulation.Size = new System.Drawing.Size(105, 17);
            this.rbApplyRegulation.TabIndex = 0;
            this.rbApplyRegulation.TabStop = true;
            this.rbApplyRegulation.Text = "Apply Regulation";
            this.rbApplyRegulation.UseVisualStyleBackColor = true;
            this.rbApplyRegulation.CheckedChanged += new System.EventHandler(this.rbApplyRegulation_CheckedChanged);
            // 
            // rbApplySaveRegulation
            // 
            this.rbApplySaveRegulation.AutoSize = true;
            this.rbApplySaveRegulation.Location = new System.Drawing.Point(40, 73);
            this.rbApplySaveRegulation.Name = "rbApplySaveRegulation";
            this.rbApplySaveRegulation.Size = new System.Drawing.Size(248, 17);
            this.rbApplySaveRegulation.TabIndex = 1;
            this.rbApplySaveRegulation.Text = "Apply Regulation and Save with Custom Name.";
            this.rbApplySaveRegulation.UseVisualStyleBackColor = true;
            this.rbApplySaveRegulation.CheckedChanged += new System.EventHandler(this.rbApplySaveRegulation_CheckedChanged);
            // 
            // lblCustomTemplate
            // 
            this.lblCustomTemplate.AutoSize = true;
            this.lblCustomTemplate.Location = new System.Drawing.Point(39, 110);
            this.lblCustomTemplate.Name = "lblCustomTemplate";
            this.lblCustomTemplate.Size = new System.Drawing.Size(191, 13);
            this.lblCustomTemplate.TabIndex = 2;
            this.lblCustomTemplate.Text = "Regulation Guidelines Template Name:";
            // 
            // txtBoxCTName
            // 
            this.txtBoxCTName.Enabled = false;
            this.txtBoxCTName.Location = new System.Drawing.Point(42, 126);
            this.txtBoxCTName.Name = "txtBoxCTName";
            this.txtBoxCTName.Size = new System.Drawing.Size(355, 20);
            this.txtBoxCTName.TabIndex = 3;
            this.txtBoxCTName.TextChanged += new System.EventHandler(this.txtBoxCTName_TextChanged);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(277, 170);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "OK";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(358, 170);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // Form_SaveFileDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(444, 212);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtBoxCTName);
            this.Controls.Add(this.lblCustomTemplate);
            this.Controls.Add(this.rbApplySaveRegulation);
            this.Controls.Add(this.rbApplyRegulation);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form_SaveFileDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Save File Option";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton rbApplyRegulation;
        private System.Windows.Forms.RadioButton rbApplySaveRegulation;
        private System.Windows.Forms.Label lblCustomTemplate;
        private System.Windows.Forms.TextBox txtBoxCTName;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
    }
}