using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using Idera.SQLcompliance.Core.Collector;

namespace Idera.SQLcompliance.Test.CollectionService
{
   /// <summary>
   /// Summary description for Form1.
   /// </summary>
   public class Form1 : Form
   {
      private ListBox lstEvents;
      private EventLog eventLog1;
      private IContainer components;
      private MainMenu mainMenu1;
      private MenuItem menuItem1;
      private MenuItem menuStart;
      private MenuItem menuStop;
      private MenuItem menuExit;
      private MenuItem menuItem2;
      private MenuItem menuClear;
      private MenuItem menuNewestAtTop;
      private MenuItem menuSetMaxEvents;


      private CollectionServer server;
      private int maxEvents = 500;


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
         this.menuItem1 = new System.Windows.Forms.MenuItem();
         this.menuExit = new System.Windows.Forms.MenuItem();
         this.menuItem2 = new System.Windows.Forms.MenuItem();
         this.menuClear = new System.Windows.Forms.MenuItem();
         this.menuNewestAtTop = new System.Windows.Forms.MenuItem();
         this.menuSetMaxEvents = new System.Windows.Forms.MenuItem();
         this.menuStart = new System.Windows.Forms.MenuItem();
         this.menuStop = new System.Windows.Forms.MenuItem();
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
         this.lstEvents.Size = new System.Drawing.Size(752, 355);
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
                                                 this.menuItem1,
                                                 this.menuItem2,
                                                 this.menuStart,
                                                 this.menuStop
                                              });
         // 
         // menuItem1
         // 
         this.menuItem1.Index = 0;
         this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[]
                                              {
                                                 this.menuExit
                                              });
         this.menuItem1.Text = "&File";
         // 
         // menuExit
         // 
         this.menuExit.Index = 0;
         this.menuExit.Text = "E&xit";
         this.menuExit.Click += new System.EventHandler(this.menuExit_Click);
         // 
         // menuItem2
         // 
         this.menuItem2.Index = 1;
         this.menuItem2.MenuItems.AddRange(new System.Windows.Forms.MenuItem[]
                                              {
                                                 this.menuClear,
                                                 this.menuNewestAtTop,
                                                 this.menuSetMaxEvents
                                              });
         this.menuItem2.Text = "&View";
         // 
         // menuClear
         // 
         this.menuClear.Index = 0;
         this.menuClear.Text = "&Clear list";
         this.menuClear.Click += new System.EventHandler(this.menuClear_Click);
         // 
         // menuNewestAtTop
         // 
         this.menuNewestAtTop.Checked = true;
         this.menuNewestAtTop.Index = 1;
         this.menuNewestAtTop.Text = "Show newest events at top of list";
         this.menuNewestAtTop.Click += new System.EventHandler(this.menuNewestAtTop_Click);
         // 
         // menuSetMaxEvents
         // 
         this.menuSetMaxEvents.Index = 2;
         this.menuSetMaxEvents.Text = "Set maximum events to display";
         this.menuSetMaxEvents.Click += new System.EventHandler(this.menuSetMaxEvents_Click);
         // 
         // menuStart
         // 
         this.menuStart.Index = 2;
         this.menuStart.Text = "&Start Service";
         this.menuStart.Click += new System.EventHandler(this.menuStart_Click);
         // 
         // menuStop
         // 
         this.menuStop.Enabled = false;
         this.menuStop.Index = 3;
         this.menuStop.Text = "Sto&p Service";
         this.menuStop.Click += new System.EventHandler(this.menuStop_Click);
         // 
         // Form1
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(752, 358);
         this.Controls.Add(this.lstEvents);
         this.Icon = ((System.Drawing.Icon) (resources.GetObject("$this.Icon")));
         this.Menu = this.mainMenu1;
         this.Name = "Form1";
         this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
         this.Text = "Collection Server Test";
         this.Closing += new System.ComponentModel.CancelEventHandler(this.Form1_Closing);
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

      private void
         StopServer()
      {
         Cursor = Cursors.WaitCursor;

         if (server != null)
         {
            InsertEvent("Stopping server ...");

            server.Stop();
            server = null;

            menuStop.Enabled = false;
            menuStart.Enabled = true;
            InsertEvent("Server stopped");
         }
         else
         {
            InsertEvent("No server to stop");
         }

         Cursor = Cursors.Default;
      }


      private void NewLogEntry(object sender, EntryWrittenEventArgs e)
      {
         if (e.Entry.Source.StartsWith("SQLcompliance"))
         {
            string source;

            if (e.Entry.Source.EndsWith("Agent"))
               source = "Agent";
            else
               source = "Collector";

            try
            {
               InsertEvent(String.Format("{0}: {1} : {2} : {3 } ",
                                         e.Entry.TimeGenerated,
                                         source,
                                         e.Entry.EntryType,
                                         e.Entry.Message));
            }
            catch (Exception ex)
            {
               string s = ex.Message;
            }
         }
      }

      private void Form1_Closing(object sender, CancelEventArgs e)
      {
         StopServer();
      }

      private void menuStop_Click(object sender, EventArgs e)
      {
         StopServer();
      }

      private void menuStart_Click(object sender, EventArgs e)
      {
         if (server == null) server = CollectionServer.Instance;
         if (server != null)
         {
            InsertEvent("Starting server ...");
            Application.DoEvents();

            Cursor = Cursors.WaitCursor;
            server.Start();
            Cursor = Cursors.Default;

            menuStop.Enabled = true;
            menuStart.Enabled = false;

            InsertEvent("Server started");
         }
         else
         {
            InsertEvent("Error starting server");
         }
      }

      private void menuExit_Click(object sender, EventArgs e)
      {
         StopServer();
         Close();
      }

      private void menuClear_Click(object sender, EventArgs e)
      {
         lstEvents.Items.Clear();
      }

      private void
         InsertEvent(
         string s
         )
      {
         if (lstEvents.Items != null)
         {
            if (lstEvents.Items.Count >= maxEvents)
            {
               if (menuNewestAtTop.Checked)
               {
                  lstEvents.Items.RemoveAt(lstEvents.Items.Count - 1);
               }
               else
               {
                  lstEvents.Items.RemoveAt(0);
               }
            }
         }

         if (menuNewestAtTop.Checked)
         {
            lstEvents.Items.Insert(0, s);
            lstEvents.SelectedIndex = 0;
         }
         else
         {
            lstEvents.Items.Add(s);
            lstEvents.SelectedIndex = lstEvents.Items.Count - 1;
         }
      }

      private void menuNewestAtTop_Click(object sender, EventArgs e)
      {
         menuNewestAtTop.Checked = ! menuNewestAtTop.Checked;
      }

      private void menuSetMaxEvents_Click(object sender, EventArgs e)
      {
         MaxEventsForm frm = new MaxEventsForm(maxEvents);
         if (DialogResult.OK == frm.ShowDialog())
         {
            maxEvents = frm.MaxEvents;
         }
      }
   }
}