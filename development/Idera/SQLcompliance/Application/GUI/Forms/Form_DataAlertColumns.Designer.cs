namespace Idera.SQLcompliance.Application.GUI.Forms
{
   partial class Form_DataAlertColumns
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_DataAlertColumns));
            this.descriptionText = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.availableColumns = new System.Windows.Forms.TreeView();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.notConfiguredPanel = new System.Windows.Forms.Panel();
            this.notConfiguredText = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.notConfiguredPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // descriptionText
            // 
            this.descriptionText.Location = new System.Drawing.Point(6, 16);
            this.descriptionText.Name = "descriptionText";
            this.descriptionText.Size = new System.Drawing.Size(283, 59);
            this.descriptionText.TabIndex = 0;
            this.descriptionText.Text = "Select a column for the really nice alert.";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.descriptionText);
            this.groupBox1.Location = new System.Drawing.Point(12, 7);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(295, 78);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            // 
            // availableColumns
            // 
            this.availableColumns.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.availableColumns.Location = new System.Drawing.Point(12, 91);
            this.availableColumns.Name = "availableColumns";
            this.availableColumns.Size = new System.Drawing.Size(295, 321);
            this.availableColumns.TabIndex = 2;
            this.availableColumns.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.availableColumns_AfterSelect);
            this.availableColumns.BeforeSelect += availableColumns_BeforeSelect;  //SQLCM -5470 v5.6
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(232, 418);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 3;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.Location = new System.Drawing.Point(151, 418);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 4;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // notConfiguredPanel
            // 
            this.notConfiguredPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.notConfiguredPanel.Controls.Add(this.notConfiguredText);
            this.notConfiguredPanel.Location = new System.Drawing.Point(0, 0);
            this.notConfiguredPanel.Name = "notConfiguredPanel";
            this.notConfiguredPanel.Size = new System.Drawing.Size(307, 412);
            this.notConfiguredPanel.TabIndex = 5;
            this.notConfiguredPanel.Visible = false;
            // 
            // notConfiguredText
            // 
            this.notConfiguredText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.notConfiguredText.Location = new System.Drawing.Point(6, 3);
            this.notConfiguredText.Name = "notConfiguredText";
            this.notConfiguredText.Size = new System.Drawing.Size(283, 409);
            this.notConfiguredText.TabIndex = 0;
            this.notConfiguredText.Text = "label1";
            // 
            // Form_DataAlertColumns
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(319, 444);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.availableColumns);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.notConfiguredPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form_DataAlertColumns";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select Column";
            this.groupBox1.ResumeLayout(false);
            this.notConfiguredPanel.ResumeLayout(false);
            this.ResumeLayout(false);

      }

      

      #endregion

      private System.Windows.Forms.Label descriptionText;
      private System.Windows.Forms.GroupBox groupBox1;
      private System.Windows.Forms.TreeView availableColumns;
      private System.Windows.Forms.Button buttonCancel;
      private System.Windows.Forms.Button buttonOK;
      private System.Windows.Forms.Panel notConfiguredPanel;
      private System.Windows.Forms.Label notConfiguredText;
   }
}