namespace Idera.SQLcompliance.Application.GUI
{
   partial class ParameterTextBox
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
         this._lbl = new Infragistics.Win.Misc.UltraLabel();
         this._txtBox = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
         ((System.ComponentModel.ISupportInitialize)(this._txtBox)).BeginInit();
         this.SuspendLayout();
         // 
         // _lbl
         // 
         this._lbl.Location = new System.Drawing.Point(14, 12);
         this._lbl.Name = "_lbl";
         this._lbl.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
         this._lbl.Size = new System.Drawing.Size(62, 18);
         this._lbl.TabIndex = 0;
         this._lbl.Text = "Parameter";
         // 
         // _txtBox
         // 
         this._txtBox.Location = new System.Drawing.Point(82, 9);
         this._txtBox.Name = "_txtBox";
         this._txtBox.Size = new System.Drawing.Size(152, 21);
         this._txtBox.TabIndex = 1;
         // 
         // ParameterTextBox
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this._txtBox);
         this.Controls.Add(this._lbl);
         this.Name = "ParameterTextBox";
         this.Size = new System.Drawing.Size(250, 35);
         ((System.ComponentModel.ISupportInitialize)(this._txtBox)).EndInit();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private Infragistics.Win.Misc.UltraLabel _lbl;
      private Infragistics.Win.UltraWinEditors.UltraTextEditor _txtBox;
   }
}
