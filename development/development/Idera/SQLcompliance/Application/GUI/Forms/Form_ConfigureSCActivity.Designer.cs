namespace Idera.SQLcompliance.Application.GUI.Forms
{
    using Idera.SQLcompliance.Application.GUI.Properties;

    partial class Form_ConfigureSCActivity
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_ConfigureSCActivity));
            this._lblTitle = new System.Windows.Forms.Label();
            this.rdSelectOnly = new System.Windows.Forms.RadioButton();
            this.rdSelectAndDML = new System.Windows.Forms.RadioButton();
            this.rdAllActivity = new System.Windows.Forms.RadioButton();
            this.btnSave = new System.Windows.Forms.Button();
            this.Cancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _lblTitle
            // 
            this._lblTitle.Location = new System.Drawing.Point(12, 20);
            this._lblTitle.Name = "_lblTitle";
            this._lblTitle.Size = new System.Drawing.Size(383, 59);
            this._lblTitle.TabIndex = 0;
            this._lblTitle.Text = resources.GetString("_lblTitle.Text");
            // 
            // rdSelectOnly
            // 
            this.rdSelectOnly.AutoSize = true;
            this.rdSelectOnly.Location = new System.Drawing.Point(47, 101);
            this.rdSelectOnly.Name = "rdSelectOnly";
            this.rdSelectOnly.Size = new System.Drawing.Size(79, 17);
            this.rdSelectOnly.TabIndex = 1;
            this.rdSelectOnly.TabStop = true;
            this.rdSelectOnly.Text = "Select Only";
            this.rdSelectOnly.UseVisualStyleBackColor = true;
            // 
            // rdSelectAndDML
            // 
            this.rdSelectAndDML.AutoSize = true;
            this.rdSelectAndDML.Location = new System.Drawing.Point(47, 162);
            this.rdSelectAndDML.Name = "rdSelectAndDML";
            this.rdSelectAndDML.Size = new System.Drawing.Size(113, 17);
            this.rdSelectAndDML.TabIndex = 3;
            this.rdSelectAndDML.TabStop = true;
            this.rdSelectAndDML.Text = "SELECT and DML";
            this.rdSelectAndDML.UseVisualStyleBackColor = true;
            // 
            // rdAllActivity
            // 
            this.rdAllActivity.AutoSize = true;
            this.rdAllActivity.Location = new System.Drawing.Point(47, 131);
            this.rdAllActivity.Name = "rdAllActivity";
            this.rdAllActivity.Size = new System.Drawing.Size(73, 17);
            this.rdAllActivity.TabIndex = 2;
            this.rdAllActivity.TabStop = true;
            this.rdAllActivity.Text = "All Activity";
            this.rdAllActivity.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(218, 217);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // Cancel
            // 
            this.Cancel.Location = new System.Drawing.Point(310, 217);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(75, 23);
            this.Cancel.TabIndex = 5;
            this.Cancel.Text = "Cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // Form_ConfigureSCActivity
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(407, 252);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.rdSelectAndDML);
            this.Controls.Add(this.rdAllActivity);
            this.Controls.Add(this.rdSelectOnly);
            this.Controls.Add(this._lblTitle);
            this.Icon = global::Idera.SQLcompliance.Application.GUI.Properties.Resources.SQLcompliance_product_ico;
            this.Name = "Form_ConfigureSCActivity";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select Sensitive Column Activity";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _lblTitle;
        private System.Windows.Forms.RadioButton rdSelectOnly;
        private System.Windows.Forms.RadioButton rdSelectAndDML;
        private System.Windows.Forms.RadioButton rdAllActivity;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button Cancel;

    }
}