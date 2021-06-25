using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Idera.SQLcompliance.Test.CollectionService
{
   /// <summary>
   /// Summary description for MaxEventsForm.
   /// </summary>
   public class MaxEventsForm : Form
   {
      private Label label1;
      private TextBox textMaxEvents;
      private Button btnOK;
      private Button btnClose;

      /// <summary>
      /// Required designer variable.
      /// </summary>
      private Container components = null;

      public
         MaxEventsForm(
         int inMaxEvents
         )
      {
         //
         // Required for Windows Form Designer support
         //
         InitializeComponent();

         MaxEvents = inMaxEvents;
         textMaxEvents.Text = MaxEvents.ToString();
      }

      /// <summary>
      /// Clean up any resources being used.
      /// </summary>
      protected override void Dispose(bool disposing)
      {
         if (disposing)
         {
            if (components != null)
            {
               components.Dispose();
            }
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
         System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof (MaxEventsForm));
         this.label1 = new System.Windows.Forms.Label();
         this.textMaxEvents = new System.Windows.Forms.TextBox();
         this.btnOK = new System.Windows.Forms.Button();
         this.btnClose = new System.Windows.Forms.Button();
         this.SuspendLayout();
         // 
         // label1
         // 
         this.label1.Location = new System.Drawing.Point(31, 20);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(204, 16);
         this.label1.TabIndex = 0;
         this.label1.Text = "Maximum number of events to display:";
         // 
         // textMaxEvents
         // 
         this.textMaxEvents.Location = new System.Drawing.Point(239, 16);
         this.textMaxEvents.MaxLength = 5;
         this.textMaxEvents.Name = "textMaxEvents";
         this.textMaxEvents.Size = new System.Drawing.Size(60, 20);
         this.textMaxEvents.TabIndex = 1;
         this.textMaxEvents.Text = "100";
         // 
         // btnOK
         // 
         this.btnOK.Location = new System.Drawing.Point(86, 56);
         this.btnOK.Name = "btnOK";
         this.btnOK.TabIndex = 2;
         this.btnOK.Text = "&OK";
         this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
         // 
         // btnClose
         // 
         this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.btnClose.Location = new System.Drawing.Point(170, 56);
         this.btnClose.Name = "btnClose";
         this.btnClose.TabIndex = 3;
         this.btnClose.Text = "&Close";
         this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
         // 
         // MaxEventsForm
         // 
         this.AcceptButton = this.btnOK;
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.CancelButton = this.btnClose;
         this.ClientSize = new System.Drawing.Size(330, 88);
         this.Controls.Add(this.btnClose);
         this.Controls.Add(this.btnOK);
         this.Controls.Add(this.textMaxEvents);
         this.Controls.Add(this.label1);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.Icon = ((System.Drawing.Icon) (resources.GetObject("$this.Icon")));
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "MaxEventsForm";
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "Set maximum number of events";
         this.ResumeLayout(false);
      }

      #endregion

      private void btnClose_Click(object sender, EventArgs e)
      {
         DialogResult = DialogResult.Cancel;
         Close();
      }

      public int MaxEvents;

      private void btnOK_Click(object sender, EventArgs e)
      {
         MaxEvents = Convert.ToInt32(textMaxEvents.Text);
         DialogResult = DialogResult.OK;
         Close();
      }
   }
}