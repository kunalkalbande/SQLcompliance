using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using Idera.SQLcompliance.Core.Agent;

namespace Idera.SQLcompliance.Test.AgentService
{
   /// <summary>
   /// Summary description for Form1.
   /// </summary>
   public class Form1 : Form
   {
      private SQLcomplianceAgent agent;
      private ListBox lstEvents;
      private EventLog eventLog1;
      private MainMenu mainMenu1;
      private MenuItem menuStart;
      private MenuItem menuStop;
      private MenuItem menuPause;
      private MenuItem menuResume;
      private IContainer components;


      public Form1()
      {
         //
         // Required for Windows Form Designer support
         //
         InitializeComponent();
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

            if (agent != null)
            {
               agent.Stop();
               agent = null;
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
         this.components = new System.ComponentModel.Container();
         System.ComponentModel.ComponentResourceManager resources =
            new System.ComponentModel.ComponentResourceManager(typeof (Form1));
         this.lstEvents = new System.Windows.Forms.ListBox();
         this.eventLog1 = new System.Diagnostics.EventLog();
         this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
         this.menuStart = new System.Windows.Forms.MenuItem();
         this.menuStop = new System.Windows.Forms.MenuItem();
         this.menuPause = new System.Windows.Forms.MenuItem();
         this.menuResume = new System.Windows.Forms.MenuItem();
         ((System.ComponentModel.ISupportInitialize) (this.eventLog1)).BeginInit();
         this.SuspendLayout();
         // 
         // lstEvents
         // 
         this.lstEvents.Dock = System.Windows.Forms.DockStyle.Fill;
         this.lstEvents.HorizontalExtent = 1200;
         this.lstEvents.HorizontalScrollbar = true;
         this.lstEvents.Location = new System.Drawing.Point(0, 0);
         this.lstEvents.Name = "lstEvents";
         this.lstEvents.ScrollAlwaysVisible = true;
         this.lstEvents.Size = new System.Drawing.Size(496, 355);
         this.lstEvents.TabIndex = 9;
         this.lstEvents.TabStop = false;
         // 
         // eventLog1
         // 
         this.eventLog1.EnableRaisingEvents = true;
         this.eventLog1.Log = "Application";
         this.eventLog1.Source = "TestAgent";
         this.eventLog1.SynchronizingObject = this.lstEvents;
         this.eventLog1.EntryWritten += new System.Diagnostics.EntryWrittenEventHandler(this.NewLogEntry);
         // 
         // mainMenu1
         // 
         this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[]
                                              {
                                                 this.menuStart,
                                                 this.menuStop,
                                                 this.menuPause,
                                                 this.menuResume
                                              });
         // 
         // menuStart
         // 
         this.menuStart.Index = 0;
         this.menuStart.Text = "Start";
         this.menuStart.Click += new System.EventHandler(this.menuStart_Click);
         // 
         // menuStop
         // 
         this.menuStop.Enabled = false;
         this.menuStop.Index = 1;
         this.menuStop.Text = "Stop";
         this.menuStop.Click += new System.EventHandler(this.menuStop_Click);
         // 
         // menuPause
         // 
         this.menuPause.Enabled = false;
         this.menuPause.Index = 2;
         this.menuPause.Text = "Pause";
         this.menuPause.Click += new System.EventHandler(this.menuPause_Click);
         // 
         // menuResume
         // 
         this.menuResume.Enabled = false;
         this.menuResume.Index = 3;
         this.menuResume.Text = "Resume";
         this.menuResume.Click += new System.EventHandler(this.menuResume_Click);
         // 
         // Form1
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(496, 364);
         this.Controls.Add(this.lstEvents);
         this.Icon = ((System.Drawing.Icon) (resources.GetObject("$this.Icon")));
         this.Menu = this.mainMenu1;
         this.Name = "Form1";
         this.Text = "Agent Tester UI";
         ((System.ComponentModel.ISupportInitialize) (this.eventLog1)).EndInit();
         this.ResumeLayout(false);
      }

      #endregion

      /// <summary>
      /// The main entry point for the application.
      /// </summary>
      [STAThread]
      private static void Main()
      {
         Application.Run(new Form1());
      }


      private void NewLogEntry(object sender, EntryWrittenEventArgs e)
      {
         if (e.Entry.Source.StartsWith("SQLsecure"))
         {
            if (lstEvents.Items.Count >= 500)
               lstEvents.Items.RemoveAt(0);

            string source;

            if (e.Entry.Source.EndsWith("Agent"))
               source = "Agent";
            else
               source = "Collector";

            if (lstEvents.Items.Count > 500)
               lstEvents.Items.RemoveAt(500);
            lstEvents.Items.Insert(0, String.Format("{0}: {1} : {2} : {3 } ",
                                                    e.Entry.TimeGenerated,
                                                    source,
                                                    e.Entry.EntryType,
                                                    e.Entry.Message));
         }
      }

      private void menuStart_Click(object sender, EventArgs e)
      {
         if (agent == null) agent = SQLcomplianceAgent.Instance;
         if (agent != null)
         {
            agent.Start();
            menuStop.Enabled = true;
            menuPause.Enabled = true;
            menuStart.Enabled = false;
            lstEvents.Items.Insert(0, "A new agent started");
         }
         else
         {
            lstEvents.Items.Insert(0, "Failed to start a new agent");
         }
      }

      private void menuStop_Click(object sender, EventArgs e)
      {
         if (agent != null)
         {
            agent.Stop();
            menuStop.Enabled = false;
            menuPause.Enabled = false;
            menuResume.Enabled = false;
            menuStart.Enabled = true;
            lstEvents.Items.Insert(0, "Agent instance stopped");
            agent = null;
         }
         else
         {
            lstEvents.Items.Insert(0, "No agent instance to stop");
         }
      }

      private void menuPause_Click(object sender, EventArgs e)
      {
         if (agent != null)
         {
            menuResume.Enabled = true;
            menuPause.Enabled = false;
            lstEvents.Items.Insert(0, "Agent instance paused");
         }
         else
         {
            lstEvents.Items.Insert(0, "No agent instance to pause");
         }
      }

      private void menuResume_Click(object sender, EventArgs e)
      {
         if (agent != null)
         {
            menuResume.Enabled = false;
            menuPause.Enabled = true;
            lstEvents.Items.Insert(0, "Agent instance resumed");
         }
         else
         {
            lstEvents.Items.Insert(0, "No agent instance to resume");
         }
      }
   }
}