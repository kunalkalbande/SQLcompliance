namespace Idera.SQLcompliance.Application.GUI.Forms
{
    using Idera.SQLcompliance.Application.GUI.Properties;

    partial class Form_SelectionLogicDialog
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
            this.rbCurrentOption = new System.Windows.Forms.RadioButton();
            this.rbOtherOption = new System.Windows.Forms.RadioButton();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.selectionlabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // rbCurrentOption
            // 
            this.rbCurrentOption.AutoSize = true;
            this.rbCurrentOption.Checked = true;
            this.rbCurrentOption.Location = new System.Drawing.Point(40, 67);
            this.rbCurrentOption.Name = "rbCurrentOption";
            this.rbCurrentOption.Size = new System.Drawing.Size(93, 17);
            this.rbCurrentOption.TabIndex = 0;
            this.rbCurrentOption.TabStop = true;
            this.rbCurrentOption.Text = "Current Option";
            this.rbCurrentOption.UseVisualStyleBackColor = true;
            // 
            // rbOtherOption
            // 
            this.rbOtherOption.AutoSize = true;
            this.rbOtherOption.Location = new System.Drawing.Point(40, 95);
            this.rbOtherOption.Name = "rbOtherOption";
            this.rbOtherOption.Size = new System.Drawing.Size(85, 17);
            this.rbOtherOption.TabIndex = 1;
            this.rbOtherOption.Text = "Other Option";
            this.rbOtherOption.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(197, 150);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "Apply";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.BtnSaveClick);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(278, 150);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.BtnCancelClick);
            // 
            // selectionlabel
            // 
            this.selectionlabel.AutoSize = true;
            this.selectionlabel.Location = new System.Drawing.Point(27, 21);
            this.selectionlabel.MaximumSize = new System.Drawing.Size(350, 0);
            this.selectionlabel.Name = "selectionlabel";
            this.selectionlabel.Size = new System.Drawing.Size(89, 13);
            this.selectionlabel.TabIndex = 6;
            this.selectionlabel.Text = "Selection Header";
            // 
            // Form_SelectionLogicDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(374, 197);
            this.Controls.Add(this.selectionlabel);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.rbOtherOption);
            this.Controls.Add(this.rbCurrentOption);
            this.Icon = global::Idera.SQLcompliance.Application.GUI.Properties.Resources.SQLcompliance_product_ico;
            this.Name = "Form_SelectionLogicDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Selection Title";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton rbCurrentOption;
        private System.Windows.Forms.RadioButton rbOtherOption;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label selectionlabel;
    }
}