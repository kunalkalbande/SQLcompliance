namespace Idera.SQLcompliance.Application.GUI.Controls
{
   partial class ParameterComboBox
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

      #region Component Designer generated code

      /// <summary> 
      /// Required method for Designer support - do not modify 
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this._comboEditor = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
         this._lbl = new Infragistics.Win.Misc.UltraLabel();
         ((System.ComponentModel.ISupportInitialize)(this._comboEditor)).BeginInit();
         this.SuspendLayout();
         // 
         // _comboEditor
         // 
         this._comboEditor.DisplayStyle = Infragistics.Win.EmbeddableElementDisplayStyle.Office2007;
         this._comboEditor.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
         this._comboEditor.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this._comboEditor.Location = new System.Drawing.Point(82, 9);
         this._comboEditor.Name = "_comboEditor";
         this._comboEditor.ReadOnly = true;
         this._comboEditor.Size = new System.Drawing.Size(152, 22);
         this._comboEditor.TabIndex = 1;
         // 
         // _lbl
         // 
         this._lbl.Location = new System.Drawing.Point(11, 12);
         this._lbl.Name = "_lbl";
         this._lbl.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
         this._lbl.Size = new System.Drawing.Size(65, 21);
         this._lbl.TabIndex = 2;
         this._lbl.Text = "Parameter";
         // 
         // ParameterComboBox
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this._lbl);
         this.Controls.Add(this._comboEditor);
         this.Name = "ParameterComboBox";
         this.Size = new System.Drawing.Size(250, 40);
         ((System.ComponentModel.ISupportInitialize)(this._comboEditor)).EndInit();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private Infragistics.Win.Misc.UltraLabel _lbl;
      internal Infragistics.Win.UltraWinEditors.UltraComboEditor _comboEditor;

   }
}
