namespace Idera.SQLcompliance.Application.GUI.Forms
{
    partial class Form_RowCounts
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_RowCounts));
            this._tbRowcount = new System.Windows.Forms.TextBox();
            this._btnOk = new System.Windows.Forms.Button();
            this._btnCancel = new System.Windows.Forms.Button();
            this._gbRowcount = new System.Windows.Forms.GroupBox();
            this._textdetail = new System.Windows.Forms.TextBox();
            this._numtbRowcount = new Idera.SQLcompliance.Application.GUI.Controls.NumericTextBox();
            this._cbRowcount = new System.Windows.Forms.ComboBox();
            this._lblRowcount = new System.Windows.Forms.Label();
            this.lblRcTimeframe = new System.Windows.Forms.Label();
            this._gbTimeframe = new System.Windows.Forms.GroupBox();
            this._numtbTimeframe = new Idera.SQLcompliance.Application.GUI.Controls.FloatTextBox();
            this._lblTimeframe = new System.Windows.Forms.Label();
            this._gbRowcount.SuspendLayout();
            this._gbTimeframe.SuspendLayout();
            this.SuspendLayout();
            // 
            // _tbRowcount
            // 
            this._tbRowcount.Font = new System.Drawing.Font("Malgun Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._tbRowcount.Location = new System.Drawing.Point(5, 134);
            this._tbRowcount.Multiline = true;
            this._tbRowcount.Name = "_tbRowcount";
            this._tbRowcount.ReadOnly = true;
            this._tbRowcount.Size = new System.Drawing.Size(376, 85);
            this._tbRowcount.TabIndex = 3;
            this._tbRowcount.Text = resources.GetString("_tbRowcount.Text");
            this._tbRowcount.TextChanged += new System.EventHandler(this._tbRowcount_TextChanged);
            // 
            // _btnOk
            // 
            this._btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this._btnOk.Location = new System.Drawing.Point(208, 309);
            this._btnOk.Name = "_btnOk";
            this._btnOk.Size = new System.Drawing.Size(68, 23);
            this._btnOk.TabIndex = 6;
            this._btnOk.Text = "OK";
            this._btnOk.UseVisualStyleBackColor = true;
            this._btnOk.Click += new System.EventHandler(this._btnOk_Click);
            // 
            // _btnCancel
            // 
            this._btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._btnCancel.Location = new System.Drawing.Point(291, 309);
            this._btnCancel.Name = "_btnCancel";
            this._btnCancel.Size = new System.Drawing.Size(61, 23);
            this._btnCancel.TabIndex = 7;
            this._btnCancel.Text = "Cancel";
            this._btnCancel.UseVisualStyleBackColor = true;
            // 
            // _gbRowcount
            // 
            this._gbRowcount.Controls.Add(this._textdetail);
            this._gbRowcount.Controls.Add(this._numtbRowcount);
            this._gbRowcount.Controls.Add(this._cbRowcount);
            this._gbRowcount.Controls.Add(this._lblRowcount);
            this._gbRowcount.Location = new System.Drawing.Point(12, 12);
            this._gbRowcount.Name = "_gbRowcount";
            this._gbRowcount.Size = new System.Drawing.Size(351, 88);
            this._gbRowcount.TabIndex = 8;
            this._gbRowcount.TabStop = false;
            this._gbRowcount.Enter += new System.EventHandler(this._gbRowcount_Enter);
            // 
            // textBox1
            // 
            this._textdetail.Location = new System.Drawing.Point(330, 87);
            this._textdetail.Name = "_textdetail";
            this._textdetail.Size = new System.Drawing.Size(100, 20);
            this._textdetail.TabIndex = 6;
            // 
            // _numtbRowcount
            // 
            this._numtbRowcount.Location = new System.Drawing.Point(221, 17);
            this._numtbRowcount.Name = "_numtbRowcount";
            this._numtbRowcount.Size = new System.Drawing.Size(64, 20);
            this._numtbRowcount.TabIndex = 5;
            this._numtbRowcount.TextChanged += new System.EventHandler(this._numtbRowcount_TextChanged_1);
            // 
            // _cbRowcount
            // 
            this._cbRowcount.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbRowcount.FormattingEnabled = true;
            this._cbRowcount.Items.AddRange(new object[] {
            "<",
            "<=",
            "=",
            ">",
            ">="});
            this._cbRowcount.Location = new System.Drawing.Point(114, 16);
            this._cbRowcount.Name = "_cbRowcount";
            this._cbRowcount.Size = new System.Drawing.Size(78, 21);
            this._cbRowcount.TabIndex = 4;
            this._cbRowcount.SelectedIndexChanged += new System.EventHandler(this._cbRowcount_SelectedIndexChanged_1);
            // 
            // _lblRowcount
            // 
            this._lblRowcount.AutoSize = true;
            this._lblRowcount.Location = new System.Drawing.Point(36, 19);
            this._lblRowcount.Name = "_lblRowcount";
            this._lblRowcount.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._lblRowcount.Size = new System.Drawing.Size(60, 13);
            this._lblRowcount.TabIndex = 3;
            this._lblRowcount.Text = "Row Count";
            this._lblRowcount.Click += new System.EventHandler(this._lblRowcount_Click_1);
            // 
            // label3
            // 
            this.lblRcTimeframe.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblRcTimeframe.Location = new System.Drawing.Point(5, 120);
            this.lblRcTimeframe.Name = "lblRcTimeframe";
            this.lblRcTimeframe.Size = new System.Drawing.Size(375, 2);
            this.lblRcTimeframe.TabIndex = 9;
            // 
            // _gbTimeframe
            // 
            this._gbTimeframe.Controls.Add(this._numtbTimeframe);
            this._gbTimeframe.Controls.Add(this._lblTimeframe);
            this._gbTimeframe.Location = new System.Drawing.Point(12, 235);
            this._gbTimeframe.Name = "_gbTimeframe";
            this._gbTimeframe.Size = new System.Drawing.Size(351, 58);
            this._gbTimeframe.TabIndex = 10;
            this._gbTimeframe.TabStop = false;
            this._gbTimeframe.Enter += new System.EventHandler(this._gbTimeframe_Enter);
            // 
            // _numtbTimeframe
            // 
            this._numtbTimeframe.Location = new System.Drawing.Point(135, 22);
            this._numtbTimeframe.Name = "_numtbTimeframe";
            this._numtbTimeframe.Size = new System.Drawing.Size(100, 20);
            this._numtbTimeframe.TabIndex = 9;
            this._numtbTimeframe.TextChanged += new System.EventHandler(this._numtbTimeframe_TextChanged);
            // 
            // _lblTimeframe
            // 
            this._lblTimeframe.AutoSize = true;
            this._lblTimeframe.Location = new System.Drawing.Point(36, 25);
            this._lblTimeframe.Name = "_lblTimeframe";
            this._lblTimeframe.Size = new System.Drawing.Size(82, 13);
            this._lblTimeframe.TabIndex = 6;
            this._lblTimeframe.Text = "Time frame (hrs)";
            // 
            // Form_RowCounts
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.CancelButton = this._btnCancel;
            this.ClientSize = new System.Drawing.Size(386, 344);
            this.Controls.Add(this._gbTimeframe);
            this.Controls.Add(this.lblRcTimeframe);
            this.Controls.Add(this._gbRowcount);
            this.Controls.Add(this._btnCancel);
            this.Controls.Add(this._btnOk);
            this.Controls.Add(this._tbRowcount);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form_RowCounts";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Specify Row Count Threshold";
            this.Load += new System.EventHandler(this.Form_RowCounts_Load);
            this._gbRowcount.ResumeLayout(false);
            this._gbRowcount.PerformLayout();
            this._gbTimeframe.ResumeLayout(false);
            this._gbTimeframe.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        

        private System.Windows.Forms.TextBox _tbRowcount;
        private System.Windows.Forms.Button _btnOk;
        private System.Windows.Forms.Button _btnCancel;
        private System.Windows.Forms.GroupBox _gbRowcount;
        private Controls.NumericTextBox _numtbRowcount;
        private System.Windows.Forms.ComboBox _cbRowcount;
        private System.Windows.Forms.Label _lblRowcount;
        private System.Windows.Forms.Label lblRcTimeframe;
        private System.Windows.Forms.GroupBox _gbTimeframe;
        private System.Windows.Forms.Label _lblTimeframe;
        private System.Windows.Forms.TextBox _textdetail;
        private Controls.FloatTextBox _numtbTimeframe;
    }
}