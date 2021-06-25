using System;
using System.Collections;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;
using Idera.SQLcompliance.Application.GUI.Helper;
using Idera.SQLcompliance.Application.GUI.Properties;
using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Agent;
using Idera.SQLcompliance.Core.Collector;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
    /// <summary>
    /// Summary description for Form_AgentProperties.
    /// </summary>
    public partial class Form_AgentProperties : Form
    {
        #region Window Properties



        #endregion

        #region Properties

        private string oldSnapshot = "";
        private int isCompressedFile = 1;
        private bool isDirty = false;
        bool showRolloverWarningOnce = false;
        long oldRollover = 0;


        ServerRecord oldServer = null;
        public ServerRecord newServer = null;

        #endregion

        #region Constructor / Dispose

        public Form_AgentProperties(
              ServerRecord server
           )
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            this.Icon = Resources.SQLcompliance_product_ico;

            oldServer = server;
            oldSnapshot = CreateSnapshot(oldServer);

            // Tab: General
            txtComputer.Text = server.InstanceServer;

            // Agent Settings
            txtTimeLastHeartbeat.Text = "";

            if (!server.IsDeployed)
            {
                // Agent Tab
                textAgentStatus.Text = "Not Deployed";

                if (server.IsDeployedManually)
                    radioManuallyDeployed.Checked = true;
                else
                    radioAutoDeploy.Checked = true;
                radioManuallyDeployed.Enabled = true;
                radioAutoDeploy.Enabled = true;

                // Audit Settings
                txtTimeLastUpdate.Text = "";
                txtAuditSettingsStatus.Text = "";
                textAgentAuditLevel.Text = "";
                textCurrentAuditLevel.Text = "";

                txtTimeLastUpdate.Enabled = false;
                txtAuditSettingsStatus.Enabled = false;
                textAgentAuditLevel.Enabled = false;
                textCurrentAuditLevel.Enabled = false;
                btnUpdateAuditSettings.Enabled = false;
            }
            else
            {
                // Agent Tab
                textAgentStatus.Text = "Deployed";
                textAgentVersion.Text = server.AgentVersion;
                textServiceAccount.Text = server.AgentServiceAccount;

                if (server.IsDeployedManually)
                    radioManuallyDeployed.Checked = true;
                else
                    radioAutoDeploy.Checked = true;

                radioManuallyDeployed.Enabled = false;
                radioAutoDeploy.Enabled = false;

                txtTimeLastHeartbeat.Text = GetDateString(server.TimeLastHeartbeat);

                // Audit Settings
                txtTimeLastUpdate.Text = GetDateString(server.LastConfigUpdate);
                txtAuditSettingsStatus.Text = ((server.ConfigVersion > server.LastKnownConfigVersion)) ? UIConstants.Status_Pending
                                                             : UIConstants.Status_Current;
                btnUpdateAuditSettings.Enabled = (server.ConfigVersion > server.LastKnownConfigVersion);
                textAgentAuditLevel.Text = server.LastKnownConfigVersion.ToString();
                textCurrentAuditLevel.Text = server.ConfigVersion.ToString();
                radioFilesCompression.Checked = server.IsCompressedFile;
                radioFilesUncompression.Checked = !server.IsCompressedFile;
            }

            textAgentPort.Text = server.AgentPort.ToString();
            comboLogLevel.Text = UIUtils.GetLogLevelString(server.AgentLogLevel);
            textHeartbeatFrequency.Text = server.AgentHeartbeatInterval.ToString();

            // Trace Directory Tab
            textAgentTraceDirectory.Enabled = server.IsDeployed;
            textTraceFileRolloverSize.Enabled = true;
            textCollectionInterval.Enabled = true;
            textForcedCollectionInterval.Enabled = true;
            groupSizeLimit.Enabled = true;
            groupTimeLimit.Enabled = true;

            textAgentTraceDirectory.Text = server.AgentTraceDirectory;

            textTraceFileRolloverSize.Text = server.AgentMaxTraceSize.ToString();
            oldRollover = server.AgentMaxTraceSize;


            textCollectionInterval.Text = server.AgentCollectionInterval.ToString();
            textForcedCollectionInterval.Text = server.AgentForceCollectionInterval.ToString();

            if (server.AgentMaxFolderSize == -1)
            {
                radioDirSizeUnlimited.Checked = true;
                textMaxFolderSize.Enabled = false;
            }
            else
            {
                radioDirSizeLimit.Checked = true;
                textMaxFolderSize.Text = server.AgentMaxFolderSize.ToString();
                textMaxFolderSize.Enabled = true;
            }

            if (server.AgentMaxUnattendedTime == -1)
            {
                radioUnlimitedTime.Checked = true;
                textMaxUnattendedTime.Enabled = false;
            }
            else
            {
                radioTimeLimit.Checked = true;
                textMaxUnattendedTime.Text = server.AgentMaxUnattendedTime.ToString();
                textMaxUnattendedTime.Enabled = true;
            }

            if (server.AgentVersion == null || server.AgentVersion == "")
            {
                _tbTamperInterval.Text = CoreConstants.Agent_Default_TamperingDetectionInterval.ToString();
            }
            else if (server.AgentVersion.StartsWith("1.1") ||
                    server.AgentVersion.StartsWith("1.2") ||
                    server.AgentVersion.StartsWith("2.0"))
            {
                // Old agents don't support this option
                _gbTamper.Enabled = false;
                _tbTamperInterval.Text = CoreConstants.Agent_Default_TamperingDetectionInterval.ToString();
            }
            else
            {
                _tbTamperInterval.Text = server.DetectionInterval.ToString();

                if (ServerRecord.CompareVersions(server.AgentVersion, "3.3") >= 0)
                {
                    textTraceStartTimeout.Enabled = true;
                    textTraceStartTimeout.Text = server.AgentTraceStartTimeout.ToString();
                }
                else
                {
                    textTraceStartTimeout.Enabled = false;
                    textTraceStartTimeout.Text = CoreConstants.Agent_Default_TraceStartTimeout.ToString();
                }
            }

            // shared instances
            LoadServers();

            //------------------------------------------------------
            // Make controls read only unless user has admin access
            //------------------------------------------------------
            if (!Globals.isAdmin)
            {
                // other tabs
                for (int i = 0; i < tabProperties.TabPages.Count; i++)
                {
                    foreach (Control ctrl in tabProperties.TabPages[i].Controls)
                    {
                        ctrl.Enabled = false;
                    }
                }

                // change buttons
                btnOK.Visible = false;
                btnCancel.Text = "Close";
                btnCancel.Enabled = true;
                this.AcceptButton = btnCancel;
            }

            // reorder the tabs since it's a CF bug
            // by setting the index to "1" it sets it to the top
            // and the existing tabs get "pushed" down
            // this means the tab order on-screen will be in the reverse
            // order that we set here.              |
            tabProperties.Controls.SetChildIndex(this.tabPageTrace, 1);
            tabProperties.Controls.SetChildIndex(this.tabPageDeployment, 1);
            tabProperties.Controls.SetChildIndex(this.tabPageServers, 1);
            tabProperties.Controls.SetChildIndex(this.tabPageGeneral, 1);

            // make sure we start on general tab
            tabProperties.SelectedTab = tabPageGeneral;
        }

        //---------------------------------------------------------------------------
        // GetDateString
        //---------------------------------------------------------------------------
        private string
           GetDateString(
              DateTime time
         )
        {
            string retStr;

            if (time == DateTime.MinValue)
            {
                retStr = UIConstants.Status_Never;
            }
            else
            {
                DateTime local = time.ToLocalTime();
                retStr = String.Format("{0} {1}",
                                        local.ToShortDateString(),
                                        local.ToShortTimeString());
            }

            return retStr;
        }

        #endregion


        #region Private Methods

        //-------------------------------------------------------------
        // ValidateProperties
        //-------------------------------------------------------------
        private bool ValidateProperties()
        {
            // agent settings
            if (oldServer.AgentTraceDirectory != textAgentTraceDirectory.Text)
            {
                if (!UIUtils.ValidatePath(textAgentTraceDirectory.Text))
                {
                    ErrorMessage.Show(this.Text, UIConstants.Error_InvalidTraceDirectory);
                    tabProperties.SelectedTab = tabPageTrace;
                    textAgentTraceDirectory.Focus();
                    return false;
                }
            }

            try
            {
                ValidateRange(textTraceFileRolloverSize.Text,
                               (int)oldServer.AgentMaxTraceSize,
                               2, 50,
                               tabPageTrace, textTraceFileRolloverSize,
                               UIConstants.Error_BadTraceFileRollover);
                ValidateRange(textCollectionInterval.Text,
                               oldServer.AgentCollectionInterval,
                               1, 9999,
                               tabPageTrace, textCollectionInterval,
                               UIConstants.Error_BadCollectionInterval);
                ValidateRange(textForcedCollectionInterval.Text,
                               oldServer.AgentForceCollectionInterval,
                               1, 9999,
                               tabPageTrace, textForcedCollectionInterval,
                               UIConstants.Error_BadForcedCollectionInterval);

                //Always validate the timeout value since it is dependent on other values.
                ValidateRange(textTraceStartTimeout.Text,
                               1, UIUtils.TextToInt(textCollectionInterval.Text) * 60,  //the collection interval is in minutes and the trace timeout is in seconds
                               tabPageTrace, textTraceStartTimeout,
                               UIConstants.Error_BadTraceStartTimeout);
                ValidateRange(_tbTamperInterval.Text,
                               oldServer.DetectionInterval,
                               1, 9999,
                               tabPageTrace, _tbTamperInterval,
                               UIConstants.Error_BadTamperDetectionInterval);
                ValidateRange(textHeartbeatFrequency.Text,
                               oldServer.AgentHeartbeatInterval,
                               2, 9999,
                               tabPageGeneral, textHeartbeatFrequency,
                               UIConstants.Error_BadHeartbeatFrequency);

                if (!radioDirSizeUnlimited.Checked)
                {
                    ValidateRange(textMaxFolderSize.Text,
                                   oldServer.AgentMaxFolderSize,
                                   1, 9999,
                                   tabPageTrace, textMaxFolderSize,
                                   UIConstants.Error_BadMaxFolderSize);
                }

                if (!radioUnlimitedTime.Checked)
                {
                    ValidateRange(textMaxUnattendedTime.Text,
                                   oldServer.AgentMaxUnattendedTime,
                                   1, 999,
                                   tabPageTrace, textMaxUnattendedTime,
                                   UIConstants.Error_BadMaxUnattendedTime);
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private void
           ValidateRange(
              string strVal,
              int oldValue,
              int minValue,
              int maxValue,
              TabPage tab,
              Control cntrl,
              string errMsg
           )
        {
            int val = UIUtils.TextToInt(strVal);

            if (val == oldValue) return;

            if (val < minValue || val > maxValue)
            {
                ErrorMessage.Show(this.Text, errMsg);
                tabProperties.SelectedTab = tab;
                cntrl.Focus();
                throw new Exception("out of range");
            }
        }


        private void ValidateRange(string strVal, int minValue,
                                   int maxValue, TabPage tab,
                                   Control cntrl, string errMsg)
        {
            int val = UIUtils.TextToInt(strVal);

            if (val < minValue || val > maxValue)
            {
                ErrorMessage.Show(this.Text, errMsg);
                tabProperties.SelectedTab = tab;
                cntrl.Focus();
                throw new Exception("out of range");
            }
        }

        //-------------------------------------------------------------
        // SaveServerRecord
        //-------------------------------------------------------------
        private bool SaveServerRecord()
        {
            bool retval = false;
            string errorMsg = "";

            isDirty = false;

            if (ValidateProperties())
            {
                newServer = CreateServerRecord();
                newServer.Connection = Globals.Repository.Connection;

                try
                {
                    //---------------------------------------
                    // Write Server Properties if necessary
                    //---------------------------------------
                    if (!ServerRecord.Match(newServer, oldServer))
                    {
                        if (!newServer.Write(oldServer))
                        {
                            errorMsg = ServerRecord.GetLastError();
                            throw (new Exception(errorMsg));
                        }
                        else
                        {
                            string newSnapshot = CreateSnapshot(newServer);
                            StringBuilder log = new StringBuilder(1024);
                            log.AppendFormat("Agent Properties Change: Server {0}\r\n",
                                              newServer.AgentServer);
                            log.Append("New Settings\r\n");
                            log.Append(newSnapshot);
                            log.Append("\r\nOld Settings\r\n");
                            log.Append(oldSnapshot);
                            isDirty = true;

                            LogRecord.WriteLog(Globals.Repository.Connection,
                                                LogType.ChangeAgentProperties,
                                                newServer.AgentServer,
                                                log.ToString());
                        }
                    }
                    retval = true;
                }
                catch (Exception ex)
                {
                    errorMsg = ex.Message;
                }
                finally
                {
                    //-----------------------------------------------------------
                    // Cleanup - Close transaction, update server, display error
                    //-----------------------------------------------------------
                    if (retval && isDirty)
                    {
                        // in case agent properties were updated, update all instances
                        newServer.CopyAgentSettingsToAll(oldServer);


                        // bump server version number so that agent will synch up	      
                        newServer.ConfigVersion++;
                        ServerRecord.IncrementServerConfigVersion(Globals.Repository.Connection,
                                                                   newServer.InstanceServer);
                    }
                    if (!retval)
                    {
                        ErrorMessage.Show(this.Text,
                                           UIConstants.Error_ErrorSavingServer,
                                           errorMsg);
                    }
                }
            }
            return retval;
        }

        //--------------------------------------------------------------------
        // CreateServerRecord
        //--------------------------------------------------------------------
        private ServerRecord
           CreateServerRecord()
        {
            ServerRecord srv = oldServer.Clone();

            // Agent settings
            srv.AgentTraceDirectory = textAgentTraceDirectory.Text;
            srv.AgentMaxTraceSize = UIUtils.TextToInt(textTraceFileRolloverSize.Text);
            srv.AgentCollectionInterval = UIUtils.TextToInt(textCollectionInterval.Text);
            srv.AgentForceCollectionInterval = UIUtils.TextToInt(textForcedCollectionInterval.Text);
            srv.AgentTraceStartTimeout = UIUtils.TextToInt(textTraceStartTimeout.Text);
            srv.DetectionInterval = UIUtils.TextToInt(_tbTamperInterval.Text);
            srv.AgentHeartbeatInterval = UIUtils.TextToInt(textHeartbeatFrequency.Text);
            srv.IsCompressedFile = Convert.ToBoolean(isCompressedFile);

            if (!oldServer.IsDeployed)
            {
                srv.IsDeployedManually = radioManuallyDeployed.Checked;
            }

            if (radioDirSizeUnlimited.Checked)
                srv.AgentMaxFolderSize = -1;
            else
                srv.AgentMaxFolderSize = UIUtils.TextToInt(textMaxFolderSize.Text);

            if (radioUnlimitedTime.Checked)
                srv.AgentMaxUnattendedTime = -1;
            else
                srv.AgentMaxUnattendedTime = UIUtils.TextToInt(textMaxUnattendedTime.Text);

            if (comboLogLevel.Text == UIConstants.LogLevel_Debug)
                srv.AgentLogLevel = 3;
            else if (comboLogLevel.Text == UIConstants.LogLevel_Verbose)
                srv.AgentLogLevel = 2;
            else if (comboLogLevel.Text == UIConstants.LogLevel_Silent)
                srv.AgentLogLevel = 0;
            else
                srv.AgentLogLevel = 1;

            return srv;
        }


        //--------------------------------------------------------------------
        // CreateSnapshot
        //--------------------------------------------------------------------
        private string
           CreateSnapshot(
              ServerRecord srv
           )
        {
            StringBuilder snap = new StringBuilder(1024);

            snap.AppendFormat("\tHeartbeat interval:\t{0}\r\n", srv.AgentHeartbeatInterval);
            string lvl;
            if (srv.AgentLogLevel == 3)
                lvl = "Debug";
            else if (srv.AgentLogLevel == 2)
                lvl = "Verbose";
            else if (srv.AgentLogLevel == 1)
                lvl = "Normal";
            else
                lvl = "Silent";
            snap.AppendFormat("\tLogging level:\t{0}\r\n", lvl);


            snap.AppendFormat("\tTrace rollover size:\t{0}\r\n", srv.AgentMaxTraceSize);
            snap.AppendFormat("\tCollection interval\t{0}\r\n", srv.AgentCollectionInterval);
            snap.AppendFormat("\tForced collection interval\t{0}\r\n", srv.AgentForceCollectionInterval);

            //add the try catch to hide an error if the property doesn't exists.
            try
            {
                snap.AppendFormat("\tTrace Start Timeout\t\t{0}\r\n", srv.AgentTraceStartTimeout);
            }
            catch { }


            if (srv.AgentVersion == null || srv.AgentVersion == "" ||
               (!srv.AgentVersion.StartsWith("1.1") &&
               !srv.AgentVersion.StartsWith("1.2") &&
               !srv.AgentVersion.StartsWith("2.0")))
            {
                snap.AppendFormat("\tTamper detection interval\t{0}\r\n", srv.DetectionInterval);
            }

            string lim;
            if (srv.AgentMaxFolderSize == -1)
                lim = "Unlimited";
            else
                lim = String.Format("{0} GB", srv.AgentMaxFolderSize);
            snap.AppendFormat("\tTrace directory size limit\t{0}\r\n", lim);

            if (srv.AgentMaxUnattendedTime == -1)
                lim = "Unlimited";
            else
                lim = String.Format("{0} days", srv.AgentMaxUnattendedTime);
            snap.AppendFormat("\tUnattended auditing time limit\t{0}\r\n", lim);

            return snap.ToString();
        }


        #endregion

        #region OK / Cancel / Apply

        //-------------------------------------------------------------
        // btnOK_Click
        //-------------------------------------------------------------
        private void btnOK_Click(object sender, EventArgs e)
        {
            int newRollover = UIUtils.TextToInt(textTraceFileRolloverSize.Text);
            if (oldRollover <= 10 && newRollover > 10)
            {
                if (!showRolloverWarningOnce)
                {
                    showRolloverWarningOnce = true;

                    // warn about memory size
                    MessageBox.Show(UIConstants.Info_RolloverSizeWarning,
                       this.Text,
                       MessageBoxButtons.OK,
                       MessageBoxIcon.Warning);
                }
            }
            if (SaveServerRecord())
            {
                if (isDirty)
                    this.DialogResult = DialogResult.OK;
                else
                    this.DialogResult = DialogResult.Cancel;

                this.Close();
            }
        }

        //-------------------------------------------------------------
        // btnCancel_Click
        //-------------------------------------------------------------
        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        #endregion

        #region Form Event Handlers

        //-------------------------------------------------------------
        // btnUpdateAuditSettings_Click
        //-------------------------------------------------------------
        private void btnUpdateAuditSettings_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                AgentManager manager = GUIRemoteObjectsProvider.AgentManager();
                manager.UpdateAuditConfiguration(oldServer.Instance);

                txtAuditSettingsStatus.Text = UIConstants.Status_Requested;
            }
            catch (Exception ex)
            {
                ErrorMessage.Show(this.Text,
                                   UIConstants.Error_UpdateNowFailed,
                                   UIUtils.TranslateRemotingException(Globals.SQLcomplianceConfig.Server,
                                                                       UIConstants.CollectionServiceName,
                                                                       ex),
                                   MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        #endregion

        #region Help
        //--------------------------------------------------------------------
        // Form_AgentProperties_HelpRequested - Show Context Sensitive Help
        //--------------------------------------------------------------------
        private void Form_AgentProperties_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string helpTopic;

            if (tabProperties.SelectedTab == tabPageTrace)
                helpTopic = HelpAlias.SSHELP_Form_AgentProperties_Trace;
            else if (tabProperties.SelectedTab == tabPageDeployment)
                helpTopic = HelpAlias.SSHELP_Form_AgentProperties_Deploy;
            else if (tabProperties.SelectedTab == tabPageServers)
                helpTopic = HelpAlias.SSHELP_Form_AgentProperties_Servers;
            else
                helpTopic = HelpAlias.SSHELP_Form_AgentProperties_General;

            HelpAlias.ShowHelp(this, helpTopic);
            hlpevent.Handled = true;
        }
        #endregion


        private void radioDirSizeUnlimited_CheckedChanged(object sender, EventArgs e)
        {
            textMaxFolderSize.Enabled = radioDirSizeLimit.Checked;
        }

        private void radioUnlimitedTime_CheckedChanged(object sender, EventArgs e)
        {
            textMaxUnattendedTime.Enabled = radioTimeLimit.Checked;
        }

        private void LoadServers()
        {
            ICollection serverList = ServerRecord.GetServers(Globals.Repository.Connection, false);
            foreach (ServerRecord s in serverList)
            {
                if (s.InstanceServer.ToUpper() == oldServer.InstanceServer.ToUpper())
                {
                    ListViewItem si = listServers.Items.Add(s.Instance);
                    si.SubItems.Add(s.Description);
                }
            }
        }

        private void textTraceFileRolloverSize_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != (char)8)
            {
                e.Handled = true; // input is not passed on to the control(TextBox) 
            }
        }

        private void radioFilesCompression_CheckedChanged(object sender, EventArgs e)
        {
            isCompressedFile = 1;
        }

        private void radioFilesUncompression_CheckedChanged(object sender, EventArgs e)
        {
            if (radioFilesCompression.Checked || !((RadioButton)sender).Checked)
                return;
            if (String.Compare(oldServer.AgentVersion, "5.8.1.0") < 0)
            {
                ErrorMessage.Show(UIConstants.Caption_NonCompressedFileTransfer, UIConstants.Error_NonCompressedFileTransfer);
                radioFilesCompression.Checked = true;
                return;
            }
            else
            isCompressedFile = 0;            
        }
    }
}
