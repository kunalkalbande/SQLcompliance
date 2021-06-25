namespace Idera.SQLcompliance.Application.GUI.Controls
{
   partial class DateSelector
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
         this._label = new System.Windows.Forms.Label();
         this._dateEditor = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
         ((System.ComponentModel.ISupportInitialize)(this._dateEditor)).BeginInit();
         this.SuspendLayout();
         // 
         // _label
         // 
         this._label.AutoSize = true;
         this._label.Location = new System.Drawing.Point(21, 10);
         this._label.Name = "_label";
         this._label.Size = new System.Drawing.Size(55, 13);
         this._label.TabIndex = 0;
         this._label.Text = "Paremeter";
         // 
         // _dateEditor
         // 
         this._dateEditor.FormatString = "mm/dd/yyyy";
         this._dateEditor.Location = new System.Drawing.Point(82, 10);
         this._dateEditor.Name = "_dateEditor";
         this._dateEditor.Nullable = false;
         this._dateEditor.Size = new System.Drawing.Size(152, 21);
         this._dateEditor.TabIndex = 1;
         // 
         // DateSelector
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.Controls.Add(this._dateEditor);
         this.Controls.Add(this._label);
         this.Name = "DateSelector";
         this.Size = new System.Drawing.Size(265, 40);
         ((System.ComponentModel.ISupportInitialize)(this._dateEditor)).EndInit();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.Label _label;
      private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor _dateEditor;
   }
}
