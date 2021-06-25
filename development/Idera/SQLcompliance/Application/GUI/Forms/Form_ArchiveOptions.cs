using System ;
using System.IO;
using System.Text ;
using System.Windows.Forms ;
using Idera.SQLcompliance.Application.GUI.Helper ;
using Idera.SQLcompliance.Application.GUI.Properties ;
using Idera.SQLcompliance.Core ;
using Idera.SQLcompliance.Core.Collector;
using Idera.SQLcompliance.Core.TimeZoneHelper ;
using TimeZoneInfo = Idera.SQLcompliance.Core.TimeZoneHelper.TimeZoneInfo;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
	/// <summary>
	/// Summary description for Form_ArchiveOptions.
	/// </summary>
	public partial class Form_ArchiveOptions : Form
	{
		string oldSnapshot;
		string newSnapshot;

		public Form_ArchiveOptions()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
         this.Icon = Resources.SQLcompliance_product_ico;

			Cursor = Cursors.WaitCursor;

            // select defaults
		    cboMonthWeek.SelectedIndex = 0;
		    cboWeekday.SelectedIndex = 0;
		    radSchedule_DateWise.Checked = true;

			// Reload globals from repository
			Globals.SQLcomplianceConfig.Read( Globals.Repository.Connection );
			
         // Archive Preferences
         textAge.Text      = Globals.SQLcomplianceConfig.ArchiveAge.ToString();
         textPrefix.Text   = Globals.SQLcomplianceConfig.ArchivePrefix;
         if(Globals.SQLcomplianceConfig.ArchiveCheckIntegrity)
            _comboSkipIntegrity.SelectedIndex = 0 ;
         else
            _comboSkipIntegrity.SelectedIndex = 1 ;
         
         if (Globals.SQLcomplianceConfig.ArchivePeriod == 12 )
            comboPeriod.Text = "Month";
         else if (Globals.SQLcomplianceConfig.ArchivePeriod == 1 )
            comboPeriod.Text = "Year";
         else   
            comboPeriod.Text = "Quarter";

            // Archive Schedule
		    switch (Globals.SQLcomplianceConfig.ArchiveSchedule)
		    {
		        case SQLcomplianceConfiguration.ArchiveScheduleType.NoSchedule:
		            radNoSchedule.Checked = true;
                    break;

                case SQLcomplianceConfiguration.ArchiveScheduleType.Daily:
		            radDailySchedule.Checked = true;
                    break;

                case SQLcomplianceConfiguration.ArchiveScheduleType.Weekly:
		            radWeeklySchedule.Checked = true;
		            chkSunday.Checked = Globals.SQLcomplianceConfig.ArchiveScheduleWeekDays[0];
                    chkMonday.Checked = Globals.SQLcomplianceConfig.ArchiveScheduleWeekDays[1];
                    chkTuesday.Checked = Globals.SQLcomplianceConfig.ArchiveScheduleWeekDays[2];
                    chkWednesday.Checked = Globals.SQLcomplianceConfig.ArchiveScheduleWeekDays[3];
                    chkThursday.Checked = Globals.SQLcomplianceConfig.ArchiveScheduleWeekDays[4];
                    chkFriday.Checked = Globals.SQLcomplianceConfig.ArchiveScheduleWeekDays[5];
                    chkSaturday.Checked = Globals.SQLcomplianceConfig.ArchiveScheduleWeekDays[6];
		            updnWeekRepetition.Value = Globals.SQLcomplianceConfig.ArchiveScheduleRepetition;
                    break;

                case SQLcomplianceConfiguration.ArchiveScheduleType.MonthlyDateWise:
		            radMonthlySchedule.Checked = true;
		            radSchedule_DateWise.Checked = true;
		            updnMonthDay.Value = Globals.SQLcomplianceConfig.ArchiveScheduleDayOrWeekOfMonth;
		            updnMonthRepetition1.Value = Globals.SQLcomplianceConfig.ArchiveScheduleRepetition;
                    break;

                case SQLcomplianceConfiguration.ArchiveScheduleType.MonthlyWeekdayWise:
		            radMonthlySchedule.Checked = true;
		            radSchedule_WeekdayWise.Checked = true;
		            cboMonthWeek.SelectedIndex = Globals.SQLcomplianceConfig.ArchiveScheduleDayOrWeekOfMonth - 1;
		            for (int index = 0; index < Globals.SQLcomplianceConfig.ArchiveScheduleWeekDays.Length; index+=1)
		            {
                        if (Globals.SQLcomplianceConfig.ArchiveScheduleWeekDays[index])
		                {
		                    cboWeekday.SelectedIndex = index;
                            break;
		                }
                    }
                    updnMonthRepetition2.Value = Globals.SQLcomplianceConfig.ArchiveScheduleRepetition;
                    break;
		    }

		    dttmDailyScheduleTime.Value = Globals.SQLcomplianceConfig.ArchiveScheduleTime;
            dttmWeeklyScheduleTime.Value = Globals.SQLcomplianceConfig.ArchiveScheduleTime;
            dttmMonthlyScheduleTime.Value = Globals.SQLcomplianceConfig.ArchiveScheduleTime;

         // Load TimeZone Combo box
         TimeZoneInfo[] tza = TimeZoneInfo.GetSystemTimeZones();
         foreach (TimeZoneInfo tz in tza )
         {
            comboTimeZone.Items.Add( tz );
         }

		    txtArchiveDBFilesDirectory.Text = Globals.SQLcomplianceConfig.ArchiveDatabaseFilesLocation;
         
         //comboTimeZone.Items.Add( CoreConstants.TimeZone_UTC );
         
         // look for current setting
         int gmt = -1;
         bool found = false;
         for (int i=0; i<comboTimeZone.Items.Count; i++ )
         {
            if ( gmt == -1 )
            {
               string tz = comboTimeZone.Items[i].ToString();
               if ( (tz.IndexOf("(GMT)") != -1) && (tz.IndexOf("Greenwich") != -1) )
               {
                  gmt = i;
               }
            }
            
            if ( comboTimeZone.Items[i].ToString() == Globals.SQLcomplianceConfig.ArchiveTimeZoneName )
            {
               comboTimeZone.SelectedItem = comboTimeZone.Items[i];
               found = true;
               break;
            }
         }
         
         if ( ! found )
         {
            if ( gmt != -1 )
               comboTimeZone.SelectedIndex = gmt;
            else
               comboTimeZone.SelectedIndex = 0;
         }
         
			oldSnapshot = CreateSnapshot();

			Cursor = Cursors.Default;

         //------------------------------------------------------
         // Make controls read only unless user has admin access
         //------------------------------------------------------
         if ( ! Globals.isAdmin  )
         {
            foreach ( Control ctrl in this.Controls )
            {
               ctrl.Enabled = false;
            }

            // change buttons
            btnOK.Visible = false;
            btnCancel.Text = "Close";
            btnCancel.Enabled = true;
            this.AcceptButton = btnCancel;
         }
		}



      private void btnCancel_Click(object sender, EventArgs e)
      {
         this.DialogResult = DialogResult.Cancel;
         this.Close();
      }

      private void btnOK_Click(object sender, EventArgs e)
      {
         // validate values
         int archiveAge = UIUtils.TextToInt( textAge.Text); 
         if ( archiveAge < 1 || archiveAge > 999 )
         {
            ErrorMessage.Show( this.Text, UIConstants.Error_IllegalArchiveAge );
            return;
         }
         
         // check for illegal chars in filename prefix
         string prefix = textPrefix.Text.Trim();
         if ( prefix.Length == 0 )
         {
            ErrorMessage.Show( this.Text, UIConstants.Error_IllegalPrefix );
            return;
         }

         string illegalChars = @"*/:<>?\|";
         char[] anyOf        = illegalChars.ToCharArray();
         int pos = prefix.IndexOfAny(anyOf);
         
         if ( pos != -1 )
         {
            ErrorMessage.Show( this.Text, UIConstants.Error_IllegalPrefix );
            return;
         }

          // in case of weekly schedule, atleast one weekday must be selected
          if (radWeeklySchedule.Checked)
          {
              bool isWeekdaySelected = false;
              foreach (Control cntl in pnlWeekDays.Controls)
              {
                  CheckBox checkBox = cntl as CheckBox;
                  if (checkBox != null && checkBox.Checked)
                  {
                      isWeekdaySelected = true;
                      break;
                  }
              }

              if (!isWeekdaySelected)
              {
                  ErrorMessage.Show(Text, UIConstants.Error_ArchiveWeekdayNotSelected);
                  chkSunday.Focus();
                  return;
              }
          }

          if (string.IsNullOrEmpty(txtArchiveDBFilesDirectory.Text))
          {
              ErrorMessage.Show(Text, UIConstants.Error_ArchiveDBDirectoryNotProvided);
              txtArchiveDBFilesDirectory.Focus();
              return;
          }

          txtArchiveDBFilesDirectory.Text = txtArchiveDBFilesDirectory.Text.Trim();
          if (!Directory.Exists(txtArchiveDBFilesDirectory.Text))
          {
              ErrorMessage.Show(Text, string.Format(UIConstants.Error_ArchiveDBDirectoryNotExists, txtArchiveDBFilesDirectory.Text));
              txtArchiveDBFilesDirectory.SelectAll();
              txtArchiveDBFilesDirectory.Focus();
              return;
          }
         
         
         // save values
         Globals.SQLcomplianceConfig.ArchiveCheckIntegrity = (_comboSkipIntegrity.SelectedIndex == 0) ;
         Globals.SQLcomplianceConfig.ArchiveAge    = archiveAge;
         Globals.SQLcomplianceConfig.ArchivePrefix = textPrefix.Text.Trim();
         Globals.SQLcomplianceConfig.ArchiveDatabaseFilesLocation = txtArchiveDBFilesDirectory.Text;

         #region archive schedule settings

         // defaults
         Globals.SQLcomplianceConfig.ArchiveSchedule = SQLcomplianceConfiguration.ArchiveScheduleType.NoSchedule;
         Globals.SQLcomplianceConfig.ArchiveScheduleTime = DateTime.MinValue;
         Globals.SQLcomplianceConfig.ArchiveScheduleRepetition = 1;

         if (radDailySchedule.Checked)
         {
             Globals.SQLcomplianceConfig.ArchiveSchedule = SQLcomplianceConfiguration.ArchiveScheduleType.Daily;
             Globals.SQLcomplianceConfig.ArchiveScheduleTime = dttmDailyScheduleTime.Value;
         }
         else if (radWeeklySchedule.Checked)
         {
             Globals.SQLcomplianceConfig.ArchiveSchedule = SQLcomplianceConfiguration.ArchiveScheduleType.Weekly;
             Globals.SQLcomplianceConfig.ArchiveScheduleTime = dttmWeeklyScheduleTime.Value;
             Globals.SQLcomplianceConfig.ArchiveScheduleRepetition = (int)updnWeekRepetition.Value;

             Globals.SQLcomplianceConfig.ArchiveScheduleWeekDays[0] = chkSunday.Checked;
             Globals.SQLcomplianceConfig.ArchiveScheduleWeekDays[1] = chkMonday.Checked;
             Globals.SQLcomplianceConfig.ArchiveScheduleWeekDays[2] = chkTuesday.Checked;
             Globals.SQLcomplianceConfig.ArchiveScheduleWeekDays[3] = chkWednesday.Checked;
             Globals.SQLcomplianceConfig.ArchiveScheduleWeekDays[4] = chkThursday.Checked;
             Globals.SQLcomplianceConfig.ArchiveScheduleWeekDays[5] = chkFriday.Checked;
             Globals.SQLcomplianceConfig.ArchiveScheduleWeekDays[6] = chkSaturday.Checked;
         }
         else if (radMonthlySchedule.Checked)
         {
             if (radSchedule_DateWise.Checked)
             {
                 Globals.SQLcomplianceConfig.ArchiveSchedule = SQLcomplianceConfiguration.ArchiveScheduleType.MonthlyDateWise;
                 Globals.SQLcomplianceConfig.ArchiveScheduleRepetition = (int) updnMonthRepetition1.Value;
                 Globals.SQLcomplianceConfig.ArchiveScheduleDayOrWeekOfMonth = (int) updnMonthDay.Value;
             }
             else
             {
                 Globals.SQLcomplianceConfig.ArchiveSchedule = SQLcomplianceConfiguration.ArchiveScheduleType.MonthlyWeekdayWise;
                 Globals.SQLcomplianceConfig.ArchiveScheduleRepetition = (int)updnMonthRepetition2.Value;
                 Globals.SQLcomplianceConfig.ArchiveScheduleDayOrWeekOfMonth = cboMonthWeek.SelectedIndex + 1;

                 for (int index = 0; index < Globals.SQLcomplianceConfig.ArchiveScheduleWeekDays.Length; index += 1)
                 {
                     if (index == cboWeekday.SelectedIndex)
                         Globals.SQLcomplianceConfig.ArchiveScheduleWeekDays[index] = true;
                     else
                         Globals.SQLcomplianceConfig.ArchiveScheduleWeekDays[index] = false;
                 }
             }
             Globals.SQLcomplianceConfig.ArchiveScheduleTime = dttmMonthlyScheduleTime.Value;
         }

        #endregion

         if ( comboPeriod.Text == "Month" )
            Globals.SQLcomplianceConfig.ArchivePeriod = 12;
         else if ( comboPeriod.Text == "Year" )
            Globals.SQLcomplianceConfig.ArchivePeriod = 1;
         else
            Globals.SQLcomplianceConfig.ArchivePeriod = 3;
            
         if ( comboTimeZone.SelectedItem.ToString() == CoreConstants.TimeZone_UTC )
         {
            // since we show UTC in default case, dont override here
            // there is some special handling in server for this case
            // to set to local time for server
            if ( Globals.SQLcomplianceConfig.ArchiveTimeZoneName != "" )
            {
               Globals.SQLcomplianceConfig.ArchiveTimeZoneName = CoreConstants.TimeZone_UTC;

               Globals.SQLcomplianceConfig.ArchiveBias         = 0;
               Globals.SQLcomplianceConfig.ArchiveStandardBias = 0;
               Globals.SQLcomplianceConfig.ArchiveStandardDate = new DateTime( 2000,10,5,2,0,0,0);
               Globals.SQLcomplianceConfig.ArchiveDaylightBias = 0;
               Globals.SQLcomplianceConfig.ArchiveDaylightDate = new DateTime( 2000,4,1,2,0,0,0);
            }            
         }
         else
         {
            TimeZoneInfo tzi = (TimeZoneInfo)comboTimeZone.SelectedItem;
            Globals.SQLcomplianceConfig.ArchiveTimeZoneName = tzi.Name;
            Globals.SQLcomplianceConfig.ArchiveBias         = tzi.TimeZoneStruct.Bias;
            Globals.SQLcomplianceConfig.ArchiveStandardBias = tzi.TimeZoneStruct.StandardBias;
            Globals.SQLcomplianceConfig.ArchiveStandardDate = SystemTime.ToTimeZoneDateTime(tzi.TimeZoneStruct.StandardDate);
            Globals.SQLcomplianceConfig.ArchiveDaylightBias = tzi.TimeZoneStruct.DaylightBias;
            Globals.SQLcomplianceConfig.ArchiveDaylightDate = SystemTime.ToTimeZoneDateTime(tzi.TimeZoneStruct.DaylightDate);
         }
         
   	   newSnapshot = CreateSnapshot();
   	   if ( oldSnapshot != newSnapshot )
   	   {
   	      // only save and write snapshot if something changed
   	      
            if ( SaveValues() )
            {
   			   
			      StringBuilder log = new StringBuilder(1024);
   			   
   			   log.Append("Archive Preferences Changed\r\n\r\n");
			      log.Append("Old Settings\r\n");
			      log.Append(oldSnapshot);
			      log.Append("\r\n");
			      log.Append("New Settings\r\n");
			      log.Append(newSnapshot);
            
               LogRecord.WriteLog( Globals.Repository.Connection,
                                 LogType.ConfigureAutoArchive,
                                 log.ToString() );

               // restart automatic archive scheduler
               // force generation of ececution plan based on new archive preferences
               ArchiveScheduler.Instance.Reatart(true);

               this.DialogResult = DialogResult.OK;
               this.Close();
            }
         }
         else
         {
            this.DialogResult = DialogResult.OK;
            this.Close();
         }
      }
      
      private bool SaveValues()
      {
         Globals.SQLcomplianceConfig.Write( Globals.Repository.Connection );
         
         return true;
      }
      
      private string CreateSnapshot()
      {
         StringBuilder snap = new StringBuilder();

         SQLcomplianceConfiguration.ArchiveScheduleType scheduleType = SQLcomplianceConfiguration.ArchiveScheduleType.NoSchedule;

         DateTime scheduleTime = new DateTime(2014, 1, 1, 1, 30, 0); // default
         int scheduleRepetition = 1;
         string weekDays = "";
         int monthDayOrWeekOfMonth = 1;

          if (radDailySchedule.Checked)
          {
              scheduleType = SQLcomplianceConfiguration.ArchiveScheduleType.Daily;
              scheduleTime = dttmDailyScheduleTime.Value;
          }
         else if (radWeeklySchedule.Checked)
         {
             scheduleType = SQLcomplianceConfiguration.ArchiveScheduleType.Weekly;
             StringBuilder weekDaysBuilder = new StringBuilder();
             foreach (Control cntl in pnlWeekDays.Controls)
             {
                 CheckBox checkBox = cntl as CheckBox;
                 if (checkBox != null && checkBox.Checked)
                     weekDaysBuilder.AppendFormat("{0} ", checkBox.Name.Remove(0, 3)); // remove leading 'chk' from name to form weekday
             }

             scheduleRepetition = (int) updnWeekRepetition.Value;
             weekDays = weekDaysBuilder.ToString().Trim();
             scheduleTime = dttmWeeklyScheduleTime.Value;
         }
         else if (radMonthlySchedule.Checked)
         {
             if (radSchedule_DateWise.Checked)
             {
                 scheduleType = SQLcomplianceConfiguration.ArchiveScheduleType.MonthlyDateWise;
                 monthDayOrWeekOfMonth = (int) updnMonthDay.Value;
                 scheduleRepetition = (int) updnMonthRepetition1.Value;
             }
             else
             {
                 scheduleType = SQLcomplianceConfiguration.ArchiveScheduleType.MonthlyWeekdayWise;
                 monthDayOrWeekOfMonth = cboMonthWeek.SelectedIndex + 1;
                 weekDays = cboWeekday.SelectedItem.ToString();
                 scheduleRepetition = (int) updnMonthRepetition2.Value;
             }

             scheduleTime = dttmMonthlyScheduleTime.Value;
         }

         snap.Append( "\tArchive Options\r\n") ;
         snap.AppendFormat( "\t\tArchive events older then {0} days.\r\n", textAge.Text );
         snap.AppendFormat( "\t\tSkip Integrity Check:  {0}\r\n", _comboSkipIntegrity.SelectedIndex == 0 ? "False" : "True");
         snap.AppendFormat( "\t\tArchive time zone: {0}\r\n", comboTimeZone.Text );
         snap.Append( "\tArchive partitioning\r\n" );
         snap.AppendFormat( "\t\tArchive partition period: {0}\r\n", comboPeriod.Text );
         snap.AppendFormat( "\t\tArchive database prefix: {0}\r\n", textPrefix.Text );
         snap.AppendFormat( "\t\tArchive database files location: {0}\r\n", txtArchiveDBFilesDirectory.Text);
         snap.Append( "\tArchive Schedule Options\r\n");
         snap.AppendFormat( "\t\tArchive Schedule Type: {0}\r\n", scheduleType);
         snap.AppendFormat( "\t\tArchive Schedule Time: {0}\r\n", scheduleTime.ToString("hh:mm:ss tt"));
         snap.AppendFormat( "\t\tArchive Schedule Repetition: {0}\r\n", scheduleRepetition);
         snap.AppendFormat( "\t\tArchive Schedule Weekdays: {0}\r\n", weekDays);
         snap.AppendFormat("\t\tArchive Schedule Day Or Week Of Month: {0}\r\n", monthDayOrWeekOfMonth);

         return snap.ToString();
      }

      #region Help
      //--------------------------------------------------------------------
      // Form_Defaults_HelpRequested - Show Context Sensitive Help
      //--------------------------------------------------------------------
      private void Form_Defaults_HelpRequested(object sender, HelpEventArgs hlpevent)
      {
		   HelpAlias.ShowHelp(this,HelpAlias.SSHELP_Form_ArchiveOptions);
			hlpevent.Handled = true;
      }
      #endregion

      private void textPrefix_KeyPress(object sender, KeyPressEventArgs e)
      {
         if ( ( e.KeyChar == '"' )  ||
              ( e.KeyChar == '*' )  ||
              ( e.KeyChar == '|' )  ||
              ( e.KeyChar == '\\' ) ||
              ( e.KeyChar == ':' )  ||
              ( e.KeyChar == '/' )  ||
              ( e.KeyChar == '[' )  ||
              ( e.KeyChar == ']' )  ||
              ( e.KeyChar == '?' ) )
         {
            e.Handled=true; // input is not passed on to the control(TextBox) 
         }
      }

      private void linkLblHelpBestPractices_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
      {
         HelpAlias.ShowHelp(this, HelpAlias.SSHELP_AuditingBestPractices);
     }

     #region Archive DB Files Location Selection

        private void SelectArchiveDbFilesDirectory()
        {
            FolderBrowserDialog dlgBrowse = new FolderBrowserDialog();
            dlgBrowse.RootFolder = Environment.SpecialFolder.MyComputer;
            dlgBrowse.ShowNewFolderButton = true;
            dlgBrowse.Description = @"Select directory (folder) where you want archive database files to be created.";
            if (dlgBrowse.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(dlgBrowse.SelectedPath))
                txtArchiveDBFilesDirectory.Text = dlgBrowse.SelectedPath;
        }

        private void btnBrowseArchiveDBFilesDirectory_Click(object sender, EventArgs e)
        {
            SelectArchiveDbFilesDirectory();
        }

        private void txtArchiveDBFilesDirectory_DoubleClick(object sender, EventArgs e)
        {
            SelectArchiveDbFilesDirectory();
        }

     #endregion

        private void Schedule_CheckedChanged(object sender, EventArgs e)
        {
            dttmDailyScheduleTime.Enabled = false;
            dttmWeeklyScheduleTime.Enabled = false;
            dttmMonthlyScheduleTime.Enabled = false;

            pnlWeekDays.Enabled = false;
            pnlMonthlySchedule.Enabled = false;

            updnWeekRepetition.Enabled = false;

            if (radDailySchedule.Checked)
                dttmDailyScheduleTime.Enabled = true;
            else if (radWeeklySchedule.Checked)
            {
                updnWeekRepetition.Enabled = true;
                pnlWeekDays.Enabled = true;
                dttmWeeklyScheduleTime.Enabled = true;
            }
            else if (radMonthlySchedule.Checked)
            {
                pnlMonthlySchedule.Enabled = true;
                dttmMonthlyScheduleTime.Enabled = true;
            }
        }

	    private void MonthlySchedule_CheckChanged(object sender, EventArgs e)
	    {
	        updnMonthRepetition1.Enabled = false;
	        updnMonthRepetition2.Enabled = false;
	        updnMonthDay.Enabled = false;
	        cboMonthWeek.Enabled = false;
	        cboWeekday.Enabled = false;

	        if (radSchedule_DateWise.Checked)
	        {
	            updnMonthDay.Enabled = true;
	            updnMonthRepetition1.Enabled = true;
	        }
            else if (radSchedule_WeekdayWise.Checked)
            {
                cboMonthWeek.Enabled = true;
                cboWeekday.Enabled = true;
                updnMonthRepetition2.Enabled = true;
            }
	    }

    }
}
