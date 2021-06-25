using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Reflection;

namespace Idera.SQLcompliance.Utility.TraceRegister
{
   /// <summary>
   /// Summary description for Form_About.
   /// </summary>
   public class Form_About : System.Windows.Forms.Form
   {
      private System.Windows.Forms.Button btnClose;
      private System.Windows.Forms.Label labelDesc;
      private System.Windows.Forms.Label labelSQLsecure;
      private System.Windows.Forms.PictureBox picSQLsecure;
      /// <summary>
      /// Required designer variable.
      /// </summary>
      private System.ComponentModel.Container components = null;

      public Form_About()
      {
         //
         // Required for Windows Form Designer support
         //
         InitializeComponent();

         //
         // TODO: Add any constructor code after InitializeComponent call
         //
         Assembly assembly = Assembly.GetExecutingAssembly();

         string version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
         if (version != null && version.Equals("0.0.0.0")) 
         {
            version = "1.2";
         }

         if (version != null && version.StartsWith("v"))
            version = version.Substring(1, version.Length -1);

         this.labelSQLsecure.Text = "TraceRegister Utility " + version;
      }

      /// <summary>
      /// Clean up any resources being used.
      /// </summary>
      protected override void Dispose( bool disposing )
      {
         if( disposing )
         {
            if(components != null)
            {
               components.Dispose();
            }
         }
         base.Dispose( disposing );
      }

      #region Windows Form Designer generated code
      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_About));
         this.btnClose = new System.Windows.Forms.Button();
         this.labelDesc = new System.Windows.Forms.Label();
         this.labelSQLsecure = new System.Windows.Forms.Label();
         this.picSQLsecure = new System.Windows.Forms.PictureBox();
         ((System.ComponentModel.ISupportInitialize)(this.picSQLsecure)).BeginInit();
         this.SuspendLayout();
         // 
         // btnClose
         // 
         this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.btnClose.Location = new System.Drawing.Point(472, 112);
         this.btnClose.Name = "btnClose";
         this.btnClose.Size = new System.Drawing.Size(75, 23);
         this.btnClose.TabIndex = 0;
         this.btnClose.Text = "&Close";
         // 
         // labelDesc
         // 
         this.labelDesc.Location = new System.Drawing.Point(208, 36);
         this.labelDesc.Name = "labelDesc";
         this.labelDesc.Size = new System.Drawing.Size(344, 64);
         this.labelDesc.TabIndex = 7;
         this.labelDesc.Text = resources.GetString("labelDesc.Text");
         this.labelDesc.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // labelSQLsecure
         // 
         this.labelSQLsecure.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.labelSQLsecure.Location = new System.Drawing.Point(200, 12);
         this.labelSQLsecure.Name = "labelSQLsecure";
         this.labelSQLsecure.Size = new System.Drawing.Size(352, 16);
         this.labelSQLsecure.TabIndex = 6;
         this.labelSQLsecure.Text = "Trace Register 1.0.300.1234";
         this.labelSQLsecure.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
         // 
         // picSQLsecure
         // 
         this.picSQLsecure.BackColor = System.Drawing.Color.White;
         this.picSQLsecure.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
         this.picSQLsecure.Image = ((System.Drawing.Image)(resources.GetObject("picSQLsecure.Image")));
         this.picSQLsecure.Location = new System.Drawing.Point(8, 12);
         this.picSQLsecure.Name = "picSQLsecure";
         this.picSQLsecure.Size = new System.Drawing.Size(184, 80);
         this.picSQLsecure.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
         this.picSQLsecure.TabIndex = 5;
         this.picSQLsecure.TabStop = false;
         // 
         // Form_About
         // 
         this.AcceptButton = this.btnClose;
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.CancelButton = this.btnClose;
         this.ClientSize = new System.Drawing.Size(558, 144);
         this.Controls.Add(this.labelDesc);
         this.Controls.Add(this.labelSQLsecure);
         this.Controls.Add(this.picSQLsecure);
         this.Controls.Add(this.btnClose);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "Form_About";
         this.ShowInTaskbar = false;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "About TraceRegister";
         ((System.ComponentModel.ISupportInitialize)(this.picSQLsecure)).EndInit();
         this.ResumeLayout(false);

      }
      #endregion

      private void btnClose_Click(object sender, System.EventArgs e)
      {
         Close();
      }
   }
}
