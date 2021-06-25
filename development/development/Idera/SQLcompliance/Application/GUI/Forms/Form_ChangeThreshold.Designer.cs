namespace Idera.SQLcompliance.Application.GUI.Forms
{
   partial class Form_ChangeThreshold
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_ChangeThreshold));
         this.thresholdNumeric = new System.Windows.Forms.NumericUpDown();
         this.thresholdDescLabel = new System.Windows.Forms.Label();
         this.ID_OK = new System.Windows.Forms.Button();
         this.ID_CANCEL = new System.Windows.Forms.Button();
         ((System.ComponentModel.ISupportInitialize)(this.thresholdNumeric)).BeginInit();
         this.SuspendLayout();
         // 
         // thresholdNumeric
         // 
         this.thresholdNumeric.Location = new System.Drawing.Point(163, 12);
         this.thresholdNumeric.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
         this.thresholdNumeric.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
         this.thresholdNumeric.Name = "thresholdNumeric";
         this.thresholdNumeric.Size = new System.Drawing.Size(53, 20);
         this.thresholdNumeric.TabIndex = 1;
         this.thresholdNumeric.Value = new decimal(new int[] {
            21,
            0,
            0,
            0});
         // 
         // thresholdDescLabel
         // 
         this.thresholdDescLabel.AutoSize = true;
         this.thresholdDescLabel.Location = new System.Drawing.Point(8, 14);
         this.thresholdDescLabel.Name = "thresholdDescLabel";
         this.thresholdDescLabel.Size = new System.Drawing.Size(149, 13);
         this.thresholdDescLabel.TabIndex = 2;
         this.thresholdDescLabel.Text = "Trace File Directory Size (GB):";
         // 
         // ID_OK
         // 
         this.ID_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
         this.ID_OK.Location = new System.Drawing.Point(60, 38);
         this.ID_OK.Name = "ID_OK";
         this.ID_OK.Size = new System.Drawing.Size(75, 23);
         this.ID_OK.TabIndex = 3;
         this.ID_OK.Text = "OK";
         this.ID_OK.UseVisualStyleBackColor = true;
         // 
         // ID_CANCEL
         // 
         this.ID_CANCEL.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.ID_CANCEL.Location = new System.Drawing.Point(141, 38);
         this.ID_CANCEL.Name = "ID_CANCEL";
         this.ID_CANCEL.Size = new System.Drawing.Size(75, 23);
         this.ID_CANCEL.TabIndex = 4;
         this.ID_CANCEL.Text = "Cancel";
         this.ID_CANCEL.UseVisualStyleBackColor = true;
         // 
         // Form_ChangeThreshold
         // 
         this.AcceptButton = this.ID_OK;
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.CancelButton = this.ID_CANCEL;
         this.ClientSize = new System.Drawing.Size(226, 73);
         this.Controls.Add(this.ID_CANCEL);
         this.Controls.Add(this.ID_OK);
         this.Controls.Add(this.thresholdNumeric);
         this.Controls.Add(this.thresholdDescLabel);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.MaximizeBox = false;
         this.Name = "Form_ChangeThreshold";
         this.ShowInTaskbar = false;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "Specify Size Limit";
         ((System.ComponentModel.ISupportInitialize)(this.thresholdNumeric)).EndInit();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.NumericUpDown thresholdNumeric;
      private System.Windows.Forms.Label thresholdDescLabel;
      private System.Windows.Forms.Button ID_OK;
      private System.Windows.Forms.Button ID_CANCEL;
   }
}