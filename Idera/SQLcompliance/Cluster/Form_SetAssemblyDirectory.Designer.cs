namespace Idera.SQLcompliance.Cluster
{
   partial class Form_SetAssemblyDirectory
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
         this.acceptButton = new System.Windows.Forms.Button();
         this.cancelButton = new System.Windows.Forms.Button();
         this.label1 = new System.Windows.Forms.Label();
         this.assemblyPathGrid = new System.Windows.Forms.DataGridView();
         this.instanceName = new System.Windows.Forms.DataGridViewTextBoxColumn();
         this.path = new System.Windows.Forms.DataGridViewTextBoxColumn();
         ((System.ComponentModel.ISupportInitialize)(this.assemblyPathGrid)).BeginInit();
         this.SuspendLayout();
         // 
         // acceptButton
         // 
         this.acceptButton.Location = new System.Drawing.Point(367, 198);
         this.acceptButton.Name = "acceptButton";
         this.acceptButton.Size = new System.Drawing.Size(75, 23);
         this.acceptButton.TabIndex = 1;
         this.acceptButton.Text = "OK";
         this.acceptButton.UseVisualStyleBackColor = true;
         this.acceptButton.Click += new System.EventHandler(this.acceptButton_Click);
         // 
         // cancelButton
         // 
         this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.cancelButton.Location = new System.Drawing.Point(448, 198);
         this.cancelButton.Name = "cancelButton";
         this.cancelButton.Size = new System.Drawing.Size(75, 23);
         this.cancelButton.TabIndex = 2;
         this.cancelButton.Text = "Cancel";
         this.cancelButton.UseVisualStyleBackColor = true;
         this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
         // 
         // label1
         // 
         this.label1.Location = new System.Drawing.Point(12, 9);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(481, 31);
         this.label1.TabIndex = 0;
         this.label1.Text = "For each instance you audit on this cluster, specify a directory on the cluster r" +
             "esource group shared disk where SQL compliance should store the CLR trigger asse" +
             "mblies.";
         // 
         // assemblyPathGrid
         // 
         this.assemblyPathGrid.AllowUserToAddRows = false;
         this.assemblyPathGrid.AllowUserToDeleteRows = false;
         this.assemblyPathGrid.AllowUserToResizeRows = false;
         this.assemblyPathGrid.BackgroundColor = System.Drawing.SystemColors.Control;
         this.assemblyPathGrid.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
         this.assemblyPathGrid.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
         this.assemblyPathGrid.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
         this.assemblyPathGrid.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
         this.assemblyPathGrid.ColumnHeadersHeight = 21;
         this.assemblyPathGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
         this.assemblyPathGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.instanceName,
            this.path});
         this.assemblyPathGrid.Location = new System.Drawing.Point(15, 50);
         this.assemblyPathGrid.MultiSelect = false;
         this.assemblyPathGrid.Name = "assemblyPathGrid";
         this.assemblyPathGrid.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
         this.assemblyPathGrid.RowHeadersVisible = false;
         this.assemblyPathGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
         this.assemblyPathGrid.Size = new System.Drawing.Size(508, 142);
         this.assemblyPathGrid.TabIndex = 3;
         this.assemblyPathGrid.VirtualMode = true;
         this.assemblyPathGrid.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.assemblyPathGrid_CellValueChanged);
         this.assemblyPathGrid.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.assemblyPathGrid_CellValueNeeded);
         this.assemblyPathGrid.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.assemblyPathGrid_CellPainting);
         // 
         // instanceName
         // 
         this.instanceName.HeaderText = "Audited Instance";
         this.instanceName.Name = "instanceName";
         this.instanceName.ReadOnly = true;
         this.instanceName.Width = 190;
         // 
         // path
         // 
         this.path.HeaderText = "Directory Path";
         this.path.Name = "path";
         this.path.Width = 313;
         // 
         // Form_SetAssemblyDirectory
         // 
         this.AcceptButton = this.acceptButton;
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.CancelButton = this.cancelButton;
         this.ClientSize = new System.Drawing.Size(536, 234);
         this.Controls.Add(this.assemblyPathGrid);
         this.Controls.Add(this.label1);
         this.Controls.Add(this.acceptButton);
         this.Controls.Add(this.cancelButton);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "Form_SetAssemblyDirectory";
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "Specify CLR Trigger Directory";
         this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.Form_SetAssemblyDirectory_HelpRequested);
         ((System.ComponentModel.ISupportInitialize)(this.assemblyPathGrid)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private System.Windows.Forms.Button acceptButton;
      private System.Windows.Forms.Button cancelButton;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.DataGridView assemblyPathGrid;
      private System.Windows.Forms.DataGridViewTextBoxColumn instanceName;
      private System.Windows.Forms.DataGridViewTextBoxColumn path;
   }
}