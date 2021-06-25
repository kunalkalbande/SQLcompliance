using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace Idera.SQLcompliance.Cluster
{
   public partial class Form_ProgressBar : Form
   {
      private BackgroundWorker triggerUpdateThread = new BackgroundWorker();
      private bool cancelled = false;

      public string ProgressMessage
      {
         get { return progressMessage.Text; }
         set { progressMessage.Text = value; }
      }

      public bool IsCancelled
      {
         get { return cancelled; }
      }

      public Form_ProgressBar()
      {
         InitializeComponent();

         // Setup the progress bar timer
         progressTimer = new Timer();
         progressTimer.Tick += new EventHandler(TimerUpdateProgressBar);
         progressTimer.Interval = 500;
         progressTimer.Enabled = true;

         // Start the trigger update thread
         triggerUpdateThread.WorkerSupportsCancellation = true;
         triggerUpdateThread.DoWork += new DoWorkEventHandler(UpdateTriggers);
         triggerUpdateThread.RunWorkerAsync();
      }

      private void TimerUpdateProgressBar(Object myObject, EventArgs myEventArgs)
      {
         progressBar.Value = progressBar.Value == 100 ? 0 : progressBar.Value + 5;

         if (!triggerUpdateThread.IsBusy)
            Close();
      }

      private void UpdateTriggers(object sender, DoWorkEventArgs e)
      {
         ProgressBarHelper.MoveTriggerAssemblies();
      }

      private void buttonCancel_Click(object sender, EventArgs e)
      {
         cancelled = true;

         if (triggerUpdateThread.IsBusy)
            triggerUpdateThread.CancelAsync();
      }
   }
}