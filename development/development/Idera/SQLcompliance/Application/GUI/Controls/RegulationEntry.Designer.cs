namespace Idera.SQLcompliance.Application.GUI.Controls
{
   partial class RegulationEntry
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
         this.panelSection = new Idera.SQLcompliance.Application.GUI.Controls.RoundedPanel();
         this.labelSection = new System.Windows.Forms.Label();
         this.panelServerEvents = new Idera.SQLcompliance.Application.GUI.Controls.RoundedPanel();
         this.label1 = new System.Windows.Forms.Label();
         this.labelServerEvents = new System.Windows.Forms.Label();
         this.panelDatabaseEvents = new Idera.SQLcompliance.Application.GUI.Controls.RoundedPanel();
         this.label2 = new System.Windows.Forms.Label();
         this.labelDatabaseEvents = new System.Windows.Forms.Label();
         this.panelSection.SuspendLayout();
         this.panelServerEvents.SuspendLayout();
         this.panelDatabaseEvents.SuspendLayout();
         this.SuspendLayout();
         // 
         // panelSection
         // 
         this.panelSection.BorderColor = System.Drawing.Color.Gray;
         this.panelSection.Controls.Add(this.labelSection);
         this.panelSection.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
         this.panelSection.FillColor2 = System.Drawing.Color.Empty;
         this.panelSection.Location = new System.Drawing.Point(3, 3);
         this.panelSection.Name = "panelSection";
         this.panelSection.Radius = 1F;
         this.panelSection.Size = new System.Drawing.Size(777, 27);
         this.panelSection.TabIndex = 1;
         // 
         // labelSection
         // 
         this.labelSection.BackColor = System.Drawing.Color.Transparent;
         this.labelSection.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.labelSection.Location = new System.Drawing.Point(7, 4);
         this.labelSection.Name = "labelSection";
         this.labelSection.Size = new System.Drawing.Size(735, 18);
         this.labelSection.TabIndex = 1;
         this.labelSection.Text = "Section";
         // 
         // panelServerEvents
         // 
         this.panelServerEvents.BorderColor = System.Drawing.Color.Gray;
         this.panelServerEvents.Controls.Add(this.label1);
         this.panelServerEvents.Controls.Add(this.labelServerEvents);
         this.panelServerEvents.FillColor = System.Drawing.Color.White;
         this.panelServerEvents.FillColor2 = System.Drawing.Color.Empty;
         this.panelServerEvents.Location = new System.Drawing.Point(73, 30);
         this.panelServerEvents.Name = "panelServerEvents";
         this.panelServerEvents.Radius = 1F;
         this.panelServerEvents.Size = new System.Drawing.Size(356, 78);
         this.panelServerEvents.TabIndex = 2;
         // 
         // label1
         // 
         this.label1.AutoSize = true;
         this.label1.BackColor = System.Drawing.Color.Transparent;
         this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.label1.Location = new System.Drawing.Point(3, 6);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(98, 15);
         this.label1.TabIndex = 3;
         this.label1.Text = "Server Events:";
         // 
         // labelServerEvents
         // 
         this.labelServerEvents.BackColor = System.Drawing.Color.Transparent;
         this.labelServerEvents.Location = new System.Drawing.Point(10, 24);
         this.labelServerEvents.Name = "labelServerEvents";
         this.labelServerEvents.Size = new System.Drawing.Size(342, 49);
         this.labelServerEvents.TabIndex = 0;
         this.labelServerEvents.Text = "Server Events";
         // 
         // panelDatabaseEvents
         // 
         this.panelDatabaseEvents.BorderColor = System.Drawing.Color.Gray;
         this.panelDatabaseEvents.Controls.Add(this.label2);
         this.panelDatabaseEvents.Controls.Add(this.labelDatabaseEvents);
         this.panelDatabaseEvents.FillColor = System.Drawing.Color.White;
         this.panelDatabaseEvents.FillColor2 = System.Drawing.Color.Empty;
         this.panelDatabaseEvents.Location = new System.Drawing.Point(431, 30);
         this.panelDatabaseEvents.Name = "panelDatabaseEvents";
         this.panelDatabaseEvents.Radius = 1F;
         this.panelDatabaseEvents.Size = new System.Drawing.Size(349, 78);
         this.panelDatabaseEvents.TabIndex = 2;
         // 
         // label2
         // 
         this.label2.AutoSize = true;
         this.label2.BackColor = System.Drawing.Color.Transparent;
         this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.label2.Location = new System.Drawing.Point(3, 6);
         this.label2.Name = "label2";
         this.label2.Size = new System.Drawing.Size(118, 15);
         this.label2.TabIndex = 4;
         this.label2.Text = "Database Events:";
         // 
         // labelDatabaseEvents
         // 
         this.labelDatabaseEvents.BackColor = System.Drawing.Color.Transparent;
         this.labelDatabaseEvents.Location = new System.Drawing.Point(10, 24);
         this.labelDatabaseEvents.Name = "labelDatabaseEvents";
         this.labelDatabaseEvents.Size = new System.Drawing.Size(324, 49);
         this.labelDatabaseEvents.TabIndex = 1;
         this.labelDatabaseEvents.Text = "DatabaseEvents";
         // 
         // RegulationEntry
         // 
         this.Controls.Add(this.panelDatabaseEvents);
         this.Controls.Add(this.panelServerEvents);
         this.Controls.Add(this.panelSection);
         this.Name = "RegulationEntry";
         this.Size = new System.Drawing.Size(788, 109);
         this.panelSection.ResumeLayout(false);
         this.panelServerEvents.ResumeLayout(false);
         this.panelServerEvents.PerformLayout();
         this.panelDatabaseEvents.ResumeLayout(false);
         this.panelDatabaseEvents.PerformLayout();
         this.ResumeLayout(false);

      }

      #endregion

      private RoundedPanel panelSection;
      public RoundedPanel panelServerEvents;
      public RoundedPanel panelDatabaseEvents;
      private System.Windows.Forms.Label labelSection;
      public System.Windows.Forms.Label labelServerEvents;
      public System.Windows.Forms.Label labelDatabaseEvents;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.Label label2;


   }
}
