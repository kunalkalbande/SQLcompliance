using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Idera.SQLcompliance.Application.GUI.Helper;
using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Agent;
using Idera.SQLcompliance.Core.Collector;
using Idera.SQLcompliance.Core.Rules;

namespace Idera.SQLcompliance.Application.GUI.SQL
{
   public enum ServerStatus
   {
      OK = 0,
      Warning = 1,
      Alert = 2,
      Archive = 3,
      Disabled = 4 
   };

   /// <summary>
   /// Summary description for SQLRepository.
   /// </summary>
   public class SQLRepository
   {
      private SqlConnection _connection ;
      private string _errMsg = "" ;
      private string _instance = "" ;
      private Dictionary<int, CMEventCategory> _sqlServerEventCategories ;
      private Dictionary<int, CMEventType> _sqlServerEventTypes ;
      private Dictionary<int, string> _changeLogEventTypes ;
      private Dictionary<int, string> _agentLogEventTypes;

      #region Properties

      public SqlConnection Connection
      {
         get { return _connection ; }
      }

      public string Instance
      {
         get { return _instance ; }
      }

      public bool Connected
      {
         get
         {
            if (_connection == null ||
               (_connection.State & ConnectionState.Open) != ConnectionState.Open)
               return false;
            else
               return true;
         }
      }

      #endregion

      public string GetLastError()
      {
         return _errMsg ;
      }

      #region Connection Management

      //-----------------------------------------------------------------------------
      // OpenConnection - open a connection to the SQLsecure configuration database
      //-----------------------------------------------------------------------------
      public bool
         OpenConnection(
         string serverName
         )
      {
         bool retval ;

         try
         {
            string strConn = String.Format("server={0};" +
                                           "database={1};" +
                                           "integrated security=SSPI;" +
                                           "Connect Timeout=30;" +
                                           "MultipleActiveResultSets=true;" +
                                           "Application Name='{2}';",
                                           serverName,
                                           CoreConstants.RepositoryDatabase,
                                           CoreConstants.ManagementConsoleName) ;
            _connection = new SqlConnection(strConn) ;
            _connection.Open() ;

            GetEventTypes() ;
            _instance = serverName ;

            _errMsg = "" ;
            retval = true ;
         }
         catch(Exception ex)
         {
            _errMsg = ex.Message ;
            retval = false ;
         }

         return retval ;
      }

        public SqlConnection GetPooledConnection()
        {
            try
            {
                string strConn = String.Format("server={0};" +
                                               "database={1};" +
                                               "integrated security=SSPI;" +
                                               "Connection Timeout=30;" +
                                                "Connection Lifetime=0;" + 
                                                "Min Pool Size=5;" + 
                                                "Max Pool Size=100;" +
                                                "Pooling=true;" +
                                                "Application Name='{2}';",
                                               _instance,
                                               CoreConstants.RepositoryDatabase,
                                               CoreConstants.ManagementConsoleName);
                SqlConnection connection = new SqlConnection(strConn);
                connection.Open();
                return connection;
            }
            catch (Exception ex)
            {
                _errMsg = ex.Message;
            }

            return null;
        }

      public bool
         ResetConnection()
      {
         if(_instance == "") return false ;

         if(_connection != null)
         {
            _connection.Close() ;
            _connection.Dispose() ;
            _connection = null ;
         }

         return OpenConnection(_instance) ;
      }


      //-----------------------------------------------------------------------------
      // CloseConnection - close the connection to the SQLsecure configuration database
      //-----------------------------------------------------------------------------
      public void
         CloseConnection()
      {
         if(_connection != null)
         {
            _connection.Close() ;
            _connection.Dispose() ;
            _connection = null ;
         }
         _instance = "" ;
      }

      public void
         ResetUse()
      {
         Use("") ;
      }

      public void
         Use(
         string databaseName
         )
      {
         if(databaseName == "") databaseName = CoreConstants.RepositoryDatabase ;

         string str = String.Format("USE {0};",
                                    SQLHelpers.CreateSafeDatabaseName(databaseName)) ;
         SqlCommand cmd = new SqlCommand(str, _connection) ;
         cmd.ExecuteNonQuery() ;
      }

      #endregion

      #region Server Logic

      //----------------------------------------------------------------
      // GetOverallStatus - For DBA LaunchPad
      //----------------------------------------------------------------
      public int
         GetOverallStatus(
         out string overallStatusMsg,
         out int serverCount,
         out int auditedServerCount,
         out int dbCount,
         out int totalEvents
         )
      {
         int overallStatus ;

         bool errorsFound = false ;
         bool warningsFound = false ;
         serverCount = 0 ;
         auditedServerCount = 0 ;
         dbCount = 0 ;
         totalEvents = 0 ;

         ICollection serverList ;
         try
         {
            serverList = ServerRecord.GetServers(Globals.Repository.Connection, true, true) ;
         }
         catch(Exception ex)
         {
            overallStatus = UIConstants.ServerImage_Warning ;
            overallStatusMsg = UIConstants.OverallStatus_NoConnection ;

            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                    "GetServers in GetStatus",
                                    ex,
                                    ErrorLog.Severity.Informational) ;
            return overallStatus ;
         }

         if((serverList != null) && (serverList.Count != 0))
         {
            foreach(ServerRecord config in serverList)
            {
               if(config.IsAuditedServer)
               {
                  serverCount ++ ;
                  if(config.IsEnabled)
                  {
                     auditedServerCount ++ ;
                  }
               }

               string opStatus ;
               string auditStatus ;
               ServerStatus status = GetStatus(config, out opStatus, out auditStatus);

               if (status == ServerStatus.Alert) errorsFound = true;
               if (status == ServerStatus.Warning) warningsFound = true;

               int serverEvents = GetEventCount(config) ;
               totalEvents += serverEvents ;
            }

            // convert individual into overall				
            if(errorsFound)
            {
               overallStatus = UIConstants.ServerImage_Alert ;
               overallStatusMsg = UIConstants.OverallStatus_Alert ;
            }
            else if(warningsFound)
            {
               overallStatus = UIConstants.ServerImage_Warning ;
               overallStatusMsg = UIConstants.OverallStatus_Warning ;
            }
            else
            {
               overallStatus = UIConstants.ServerImage_OK ;
               overallStatusMsg = UIConstants.OverallStatus_OK ;
            }
         }
         else
         {
            overallStatus = UIConstants.ServerImage_Warning ;
            overallStatusMsg = UIConstants.OverallStatus_NoServers ;
         }

         if(serverCount != 0)
         {
            dbCount = DatabaseRecord.GetAuditedDatabaseCount(Globals.Repository.Connection) ;
         }

         return overallStatus ;
      }

      //----------------------------------------------------------------
      // GetStatus
      //----------------------------------------------------------------
      static public ServerStatus GetStatus(ServerRecord config,out string opStatus,out string auditStatus)
      {
         opStatus = UIConstants.ServerStatus_OK ;
         ServerStatus statusImage = ServerStatus.OK;

         if(! config.IsAuditedServer)
         {
            //-----------------------
            // Archive host
            //-----------------------
            statusImage = ServerStatus.Archive;
            opStatus = UIConstants.ServerStatus_NotAudited ;
            auditStatus = "" ;
         }
         else
         {
            //----------------
            // Audited Server
            //----------------

            if(! config.IsDeployed)
            {
               //--------------
               // Not deployed
               //--------------
               statusImage = ServerStatus.Warning;
               if(config.IsDeployedManually)
                  opStatus = UIConstants.ServerStatus_AwaitingManual ;
               else
                  opStatus = UIConstants.ServerStatus_NotDeployed ;
            }
            else
            {
               //-------------------------------------
               // Deployed - so check for other stuff
               //-------------------------------------
               if(config.TimeLastAgentContact == DateTime.MinValue)
               {
                  // Agent has never reported in
                  statusImage = ServerStatus.Warning;
                  opStatus = UIConstants.ServerStatus_Unknown ;
               }
               else
               {
                  // check for supported SQL Server Version 
                  // we cant support auditing 2005 from a repository hosted by 2000
                  if(config.SqlVersion > Globals.repositorySqlVersion)
                  {
                     statusImage = ServerStatus.Alert;
                     if (config.SqlVersion == 9)
                        opStatus = UIConstants.ServerStatus_2005NotSupported;
                     else if (config.SqlVersion == 10)
                        opStatus = UIConstants.ServerStatus_2008NotSupported;
                     else if (config.SqlVersion == 11)
                        opStatus = UIConstants.ServerStatus_2012NotSupported;
                     else if (config.SqlVersion == 12)
                         opStatus = UIConstants.ServerStatus_2014NotSupported;
                     else if (config.SqlVersion == 13)
                         opStatus = UIConstants.ServerStatus_2016NotSupported;
					 else if (config.SqlVersion == 14)
                         opStatus = UIConstants.ServerStatus_2017NotSupported;
                  }
                  else if(config.SqlVersion == 9 && Globals.repositorySqlVersion == 9
                          && config.AgentVersion.StartsWith("1."))
                  {
                     // We also can't support auditing 2005 with a 1.x agent
                     statusImage = ServerStatus.Alert;
                     opStatus = UIConstants.ServerStatus_AgentUpgradeRequired ;
                  }
                  else if (config.SqlVersion == 10 && Globals.repositorySqlVersion == 10
                     && (config.AgentVersion.StartsWith("1.") || config.AgentVersion.StartsWith("2.")
                     || config.AgentVersion.StartsWith("3.0")))
                  {
                     // We also can't support auditing 2008 with a 1.x, 2.x, or 3.0 agent
                     statusImage = ServerStatus.Alert;
                     opStatus = UIConstants.ServerStatus_AgentUpgradeRequired;
                  }
                  else if (config.SqlVersion == 11 &&
                      Globals.repositorySqlVersion == 11 &&
                      (config.AgentVersion.StartsWith("1.") ||
                       config.AgentVersion.StartsWith("2.") ||
                       config.AgentVersion.StartsWith("3.0") ||
                       config.AgentVersion.StartsWith("3.1") ||
                       config.AgentVersion.StartsWith("3.2") ||
                       config.AgentVersion.StartsWith("3.3") ||
                       config.AgentVersion.StartsWith("3.5") ||
                       config.AgentVersion.StartsWith("3.6")))
                  {
                     //we can't support 2012 with anything less than 3.7
                     statusImage = ServerStatus.Alert;
                     opStatus = UIConstants.ServerStatus_AgentUpgradeRequired;
                  }
                  else
                  {
                     if (!config.IsRunning)
                     {
                        // Agent has reported in
                        statusImage = ServerStatus.Alert;
                        opStatus = UIConstants.ServerStatus_NotRunning;
                     }
                     else if (config.IsCrippled)
                     {
                        statusImage = ServerStatus.Alert;
                        opStatus = UIConstants.ServerStatus_Crippled;
                     }
                     else
                     {
                        // Running but possible stuff to report

                        // states
                        // Agent hasnt called home in a day - red
                        // Agent has missed two heartbeats  - yellow
                        // Disabled
                        // Audit Pending

                        DateTime noContactWarning = DateTime.UtcNow;
                        DateTime noContactError = DateTime.UtcNow;

                        noContactWarning = noContactWarning.AddMinutes(-2 * config.AgentHeartbeatInterval);
                        noContactError = noContactError.AddDays(-1);

                        if (DateTime.Compare(config.TimeLastAgentContact,
                                            noContactError) < 0)
                        {
                           opStatus = UIConstants.ServerStatus_VeryStale;
                           statusImage = ServerStatus.Alert;
                        }
                        else if (DateTime.Compare(config.TimeLastAgentContact,
                                                 noContactWarning) < 0)
                        {
                           opStatus = UIConstants.ServerStatus_Stale;
                           statusImage = ServerStatus.Warning;
                        }
                     }
                  }
               }
            }

            // Audit Status
            if(config.IsEnabled)
               auditStatus = UIConstants.ServerStatus_Enabled ;
            else
            {
               statusImage = ServerStatus.Disabled;
               auditStatus = UIConstants.ServerStatus_Disabled ;
            }

            if(config.IsDeployed)
            {
               if(config.ConfigUpdateRequested)
               {
                  if(config.ConfigVersion == config.LastKnownConfigVersion)
                  {
                     ServerRecord oldServerState = config.Clone() ;
                     config.ConfigUpdateRequested = false ;
                     config.Connection = Globals.Repository.Connection;
                     config.Write(oldServerState) ;
                  }
                  else
                  {
                     auditStatus += UIConstants.ServerStatus_Requested ;
                  }
               }
               else if(config.ConfigVersion != config.LastKnownConfigVersion)
               {
                  auditStatus += UIConstants.ServerStatus_Pending ;
               }
            }
            else
            {
               auditStatus = "None until deployed" ;
            }
         }
         // System Alerts
         if (config.AgentHealth != 0)
         {
            List<SystemAlertType> alerts = SystemAlert.GetAgentHealthDetails(config.AgentHealth);
            if (alerts.Count > 1)
            {
               opStatus = String.Format("{0} Unresolved System Alerts", alerts.Count);
            }
            else if (alerts.Count == 1)
            {
               switch (alerts[0])
               {
                  case SystemAlertType.AgentWarning:
                     opStatus = "Agent Warning";
                     break;
                  case SystemAlertType.AgentConfigurationError:
                     opStatus = "Agent Configuration Error";
                     break;
                  case SystemAlertType.TraceDirectoryError:
                     opStatus = "Trace Directory Error";
                     break;
                  case SystemAlertType.SqlTraceError:
                     opStatus = "SQL Trace Error";
                     break;
                  case SystemAlertType.ServerConnectionError:
                     opStatus = "Server Connection Error";
                     break;
                  case SystemAlertType.CollectionServiceConnectionError:
                     opStatus = "Collection Service Connection Error";
                     break;
                  case SystemAlertType.ClrError:
                     opStatus = "CLR Error";
                     break;
               }
            }
            else
            {
               opStatus = "Unresolved System Alert";
            }
            auditStatus = UIConstants.AuditStatus_ViewActivityLog;
            statusImage = ServerStatus.Alert;
         }
//         // Before After issues
//         bool hasBA = false ;
//         foreach(DatabaseRecord db in DatabaseRecord.GetDatabases(Globals.Repository.Connection, config.SrvId))
//         {
//            if(db.IsEnabled && db.AuditDataChanges)
//               hasBA = true ;
//         }
//         if (hasBA)
//         {
//            if (!IsClrEnabled(config))
//            {
//               opStatus = UIConstants.ServerStatus_CLRDisabled;
//               auditStatus = UIConstants.ServerStatus_CLRDisabled;
//               statusImage = ServerStatus.Alert;
//            }
//         }

         return statusImage ;
      }

      //-------------------------------------------------------------
      // LoadServerList
      //--------------------------------------------------------------
//      static public void
//         LoadServerList(
//         ListView listServers,
//         bool auditedServersOnly,
//         bool extras
//         )
//      {
//         try
//         {
//            ServerRecord selectedItemConfig = null ;
//            ListViewItem finalSelected = null ;
//
//            // We want to save the currently selected item
//            //  so we can reselect it at the end of refresh
//            if(listServers.SelectedItems.Count > 0)
//            {
//               selectedItemConfig = listServers.SelectedItems[0].Tag as ServerRecord ;
//            }
//
//            listServers.BeginUpdate() ;
//            listServers.Items.Clear() ;
//
//            if(Globals.Repository.Connection == null)
//            {
//               Debug.Write("Assertion - Failure to initialize database connection before loading view") ;
//               return ;
//            }
//
//            ICollection serverList ;
//            serverList = ServerRecord.GetServers(Globals.Repository.Connection, auditedServersOnly, true) ;
//
//            if((serverList != null) && (serverList.Count != 0))
//            {
//               foreach(ServerRecord config in serverList)
//               {
//                  int statusImage ;
//                  string opStatus = "" ;
//                  string auditStatus = "" ;
//
//                  if(extras)
//                  {
//                     statusImage = 0;// GetStatus(config, out opStatus, out auditStatus);
//                  }
//                  else
//                  {
//                     if(config.IsAuditedServer)
//                     {
//                        if(config.IsEnabled)
//                           statusImage = UIConstants.Icon_Server ;
//                        else
//                           statusImage = UIConstants.Icon_ServerDisabled ;
//                     }
//                     else
//                        statusImage = UIConstants.Icon_ArchiveServer ;
//                  }
//
//                  ListViewItem item = new ListViewItem(config.Instance, statusImage) ;
//
//                  item.Tag = config ;
//
//                  listServers.Items.Add(item) ;
//                  // Is this the item we should select at the end?
//                  if(selectedItemConfig != null &&
//                     String.Compare(config.Instance, selectedItemConfig.Instance, true) == 0)
//                     finalSelected = item ;
//
//                  if(extras)
//                  {
//                     item.SubItems.Add(opStatus) ;
//                     item.SubItems.Add(auditStatus) ;
//                     item.SubItems.Add(GetLastAgentContactTime(config)) ;
//                  }
//                  else
//                  {
//                     item.SubItems.Add(config.Description) ;
//                  }
//               }
//
//               // Maintain our selection; otherwise select the
//               //  first item in the list
//               if(listServers.Items.Count != 0)
//               {
//                  if(finalSelected != null)
//                  {
//                     try
//                     {
//                        finalSelected.EnsureVisible() ;
//                        finalSelected.Selected = true ;
//                     }
//                     catch
//                     {
//                     }
//                  }
//                  else
//                  {
//                     listServers.Items[0].EnsureVisible() ;
//                     listServers.Items[0].Selected = true ;
//                  }
//               }
//            }
//         }
//         finally
//         {
//            listServers.EndUpdate() ;
//         }
//      }

      static public string GetLastAgentContactTime(ServerRecord config)
      {
         if(config != null && config.IsAuditedServer)
         {
            if(config.TimeLastAgentContact == DateTime.MinValue)
               return UIConstants.Status_Never ;
            else
            {
               DateTime local = config.TimeLastAgentContact.ToLocalTime() ;
               return String.Format("{0} {1}",
                                              local.ToShortDateString(),
                                              local.ToShortTimeString()) ;
            }
         }
         else
         {
            return "" ;
         }
      }

      //-------------------------------------------------------------------
      // GetEventCount
      //  Return the approximate number of events in the Events table for the
      //  specified monitored Server
      //-------------------------------------------------------------------
      static public int GetEventCount(ServerRecord srv)
      {
         if(srv == null)
            return 0 ;
         else
            return GetTableRowCount(srv.EventDatabase, CoreConstants.RepositoryEventsTable) ;
      }

      //-------------------------------------------------------------------
      // GetServerEvents
      //-------------------------------------------------------------------
      static private int GetTableRowCount(string sDatabase, string sTable)
      {
         int count = 0 ;

         if(sTable == null || sTable.Length == 0 ||
            sDatabase == null || sDatabase.Length == 0)
         {
            return 0 ;
         }

         try
         {
            string sql =
               String.Format(
                  "select c.rowcnt from {0}..[sysobjects] a inner join {0}..sysindexes c on a.id=c.id AND (c.indid=1 or c.indid=0) where a.name='{1}' and a.xtype='U'",
                  SQLHelpers.CreateSafeDatabaseName(sDatabase), sTable) ;
            SqlCommand cmd = new SqlCommand(sql, Globals.Repository.Connection) ;

            object obj = cmd.ExecuteScalar() ;
            if(obj is DBNull)
               count = 0 ;
            else if(obj is Int32)
               count = (int)obj ;
            else if(obj is Int64)
               count = Convert.ToInt32(obj) ;
         }
         catch(Exception ex)
         {
            ErrorLog.Instance.Write(ErrorLog.Level.Debug, "GetTableRowCount()",
                                    sDatabase + ".." + sTable, ex) ;
         }
         return count ;
      }

      //-------------------------------------------------------------------
      // LoadServerTreeNode - Workhorse routine to load Servers under a 
      //                      node in the tree. Input is WHERE clause for
      //                      different parts of tree
      //-------------------------------------------------------------------
//      static public void
//         LoadServerTreeNode(
//         SQLcomplianceTreeNode parentNode
//         )
//      {
//         if(Globals.Repository.Connection == null)
//         {
//            Debug.Write("Assertion - Failure to initialize database connection before loading view") ;
//            return ;
//         }
//
//         SQLHelpers.CheckConnection(Globals.Repository.Connection) ;
//
//         ICollection serverList ;
//         serverList = ServerRecord.GetServers(Globals.Repository.Connection, false, true) ;
//
//         if((serverList != null) && (serverList.Count != 0))
//         {
//            foreach(ServerRecord config in serverList)
//            {
//               AddServerTreeNode(parentNode,
//                                 config) ;
//            }
//         }
//      }

      //-------------------------------------------------------------------
      // AddServerTreeNode
      //-------------------------------------------------------------------
//      static public void
//         AddServerTreeNode(
//         SQLcomplianceTreeNode parent,
//         ServerRecord server
//         )
//      {
//         SQLcomplianceTreeNode newNode ;
//
//         int imageIndex ;
//
//         if(server.IsAuditedServer)
//         {
//            if(server.IsEnabled)
//               imageIndex = UIConstants.Icon_Server ;
//            else
//               imageIndex = UIConstants.Icon_ServerDisabled ;
//         }
//         else
//         {
//            imageIndex = UIConstants.Icon_ArchiveServer ;
//         }
//
//
//         newNode = new SQLcomplianceTreeNode(server.Instance,
//                                             imageIndex,
//                                             imageIndex,
//                                             CMNodeType.AuditServer) ;
//
//         newNode.SetMenuFlag(CMMenuItem.Refresh) ;
//         newNode.SetMenuFlag(CMMenuItem.Properties) ;
//
//         newNode.SetMenuFlag(CMMenuItem.AttachArchive) ;
//         newNode.SetMenuFlag(CMMenuItem.NewServer) ;
//
//         if(server.IsAuditedServer)
//         {
//            newNode.SetMenuFlag(CMMenuItem.Delete) ;
//            newNode.SetMenuFlag(CMMenuItem.Enable) ;
//            newNode.SetMenuFlag(CMMenuItem.Disable) ;
//            newNode.SetMenuFlag(CMMenuItem.UpdateAuditSettings) ;
//            newNode.SetMenuFlag(CMMenuItem.ForceCollection) ;
//            newNode.SetMenuFlag(CMMenuItem.AgentProperties) ;
//            newNode.SetMenuFlag(CMMenuItem.NewDatabase) ;
//         }
//
//         newNode.IsAuditedServer = server.IsAuditedServer ;
//         newNode.IsEnabled = server.IsEnabled ;
//         newNode.ServerId = server.SrvId ;
//
//         newNode.Nodes.Add("placeholder") ;
//
//         parent.Nodes.Add(newNode) ;
//      }

      //-------------------------------------------------------------------
      // ResetServerTreeNode
      //-------------------------------------------------------------------
//      static public void
//         ResetServerTreeNode(
//         SQLcomplianceTreeNode parent,
//         string server
//         )
//      {
//         // regular server tree reset         
//         ResetWorkhorse(parent,
//                        server,
//                        false,
//                        false) ;
//      }

      //-------------------------------------------------------------------
      // ResetServerTreeNode
      //-------------------------------------------------------------------
//      static public void
//         ResetServerTreeNode(
//         SQLcomplianceTreeNode parent,
//         string server,
//         bool isAudited
//         )
//      {
//         ResetWorkhorse(parent,
//                        server,
//                        true,
//                        isAudited) ;
//      }

      //-------------------------------------------------------------------
      // ResetWorkhorse
      //-------------------------------------------------------------------
//      static private void
//         ResetWorkhorse(
//         SQLcomplianceTreeNode parent,
//         string server,
//         bool changeIsAudited,
//         bool isAudited
//         )
//      {
//         // collapse child nodes so we can reload later
//         foreach(TreeNode n in parent.Nodes)
//         {
//            if(n is SQLcomplianceTreeNode && n.Text == server)
//            {
//               SQLcomplianceTreeNode node = (SQLcomplianceTreeNode)n ;
//
//               bool expanded = node.IsExpanded ;
//
//               if(changeIsAudited)
//               {
//                  node.IsAuditedServer = isAudited ;
//
//                  int imageIndex ;
//                  if(isAudited)
//                  {
//                     if(node.IsEnabled)
//                        imageIndex = UIConstants.Icon_Server ;
//                     else
//                        imageIndex = UIConstants.Icon_ServerDisabled ;
//
//                     node.SetMenuFlag(CMMenuItem.Delete) ;
//                     node.SetMenuFlag(CMMenuItem.Enable) ;
//                     node.SetMenuFlag(CMMenuItem.Disable) ;
//                     node.SetMenuFlag(CMMenuItem.UpdateAuditSettings) ;
//                     node.SetMenuFlag(CMMenuItem.AgentProperties) ;
//                     node.SetMenuFlag(CMMenuItem.NewDatabase) ;
//                  }
//                  else
//                  {
//                     imageIndex = UIConstants.Icon_ArchiveServer ;
//
//                     node.SetMenuFlag(CMMenuItem.Delete, false) ;
//                     node.SetMenuFlag(CMMenuItem.Enable, false) ;
//                     node.SetMenuFlag(CMMenuItem.Disable, false) ;
//                     node.SetMenuFlag(CMMenuItem.UpdateAuditSettings, false) ;
//                     node.SetMenuFlag(CMMenuItem.AgentProperties, false) ;
//                     node.SetMenuFlag(CMMenuItem.NewDatabase, false) ;
//                  }
//                  node.ImageIndex = imageIndex ;
//                  node.SelectedImageIndex = imageIndex ;
//               }
//
//               node.Nodes.Clear() ;
//               node.Nodes.Add("placeholder") ;
//               node.Loaded = false ;
//
//               node.Collapse() ;
//
//               if(expanded) node.Expand() ;
//
//               break ;
//            }
//         }
//      }

      //-------------------------------------------------------------------
      // ResetServerNodeIcon
      //-------------------------------------------------------------------
//      static public void
//         ResetServerNodeIcon(
//         SQLcomplianceTreeNode node,
//         bool isAudited,
//         bool isEnabled
//         )
//      {
//         int imageIndex ;
//         if(isAudited)
//         {
//            if(isEnabled)
//               imageIndex = UIConstants.Icon_Server ;
//            else
//               imageIndex = UIConstants.Icon_ServerDisabled ;
//         }
//         else
//         {
//            imageIndex = UIConstants.Icon_ArchiveServer ;
//         }
//         node.ImageIndex = imageIndex ;
//         node.SelectedImageIndex = imageIndex ;
//
//         node.IsAuditedServer = isAudited ;
//         node.IsEnabled = isEnabled ;
//      }

      #endregion

      #region Archives Logic

      //-------------------------------------------------------------
      // LoadArchiveList
      //--------------------------------------------------------------
//      static public void
//         LoadArchiveList(
//         ListView listArchives,
//         string instance
//         )
//      {
//         listArchives.Items.Clear() ;
//
//         if(Globals.Repository.Connection == null)
//         {
//            Debug.Write("Assertion - Failure to initialize database connection before loading view") ;
//            return ;
//         }
//
//         SQLHelpers.CheckConnection(Globals.Repository.Connection) ;
//
//
//         ICollection aList = null ;
//         try
//         {
//            aList = GetArchives(instance) ;
//         }
//#if DEBUG
//         catch(Exception ex)
//         {
//            MessageBox.Show(String.Format("Error loading archives: {0}", ex.Message)) ;
//#else
//         catch ( Exception )
//         {
//#endif         
//         }
//
//         if((aList != null) && (aList.Count != 0))
//         {
//            foreach(ArchiveRecord archive in aList)
//            {
//               ListViewItem item = new ListViewItem(archive.DisplayName,
//                                                    UIConstants.Icon_Archive) ;
//               item.Tag = archive ;
//
//               listArchives.Items.Add(item) ;
//
//               item.SubItems.Add(archive.Description) ;
//            }
//
//            // select first object
//            if(listArchives.Items.Count != 0)
//            {
//               listArchives.Items[0].EnsureVisible() ;
//               listArchives.Items[0].Selected = true ;
//            }
//         }
//      }

      //-----------------------------------------------------------------------------
      // GetArchives
      //-----------------------------------------------------------------------------
      static public List<ArchiveRecord> GetArchives(string instance)
      {
         List<ArchiveRecord> aList = new List<ArchiveRecord>();

         // Load Archives
         try
         {
            string cmdstr = String.Format("SELECT instance,displayName,description,databaseName " +
                                          "FROM {0} " +
                                          "WHERE databaseType='Archive' AND instance={1} " +
                                          "ORDER by displayName ASC",
                                          CoreConstants.RepositorySystemDatabaseTable,
                                          SQLHelpers.CreateSafeString(instance)) ;

            using(SqlCommand cmd = new SqlCommand(cmdstr,
                                                  Globals.Repository.Connection))
            {
               using(SqlDataReader reader = cmd.ExecuteReader())
               {
                  while(reader.Read())
                  {
                     ArchiveRecord archive = new ArchiveRecord() ;

                     archive.Instance = SQLHelpers.GetString(reader, 0) ;
                     archive.DisplayName = SQLHelpers.GetString(reader, 1) ;
                     archive.Description = SQLHelpers.GetString(reader, 2) ;
                     archive.DatabaseName = SQLHelpers.GetString(reader, 3) ;

                     // Add to list               
                     aList.Add(archive) ;
                  }
               }
            }
         }
         catch(Exception ex)
         {
            Debug.Write(String.Format("Error loading list: {0}", ex.Message)) ;
         }

         return aList ;
      }

      //-------------------------------------------------------------------
      // LoadArchiveTreeNode
      //-------------------------------------------------------------------
//      static public void
//         LoadArchiveTreeNode(
//         SQLcomplianceTreeNode parentNode
//         )
//      {
//         string instance = parentNode.Parent.Text ;
//
//         parentNode.Nodes.Clear() ;
//
//         if(Globals.Repository.Connection == null)
//         {
//            Debug.Write("Assertion - Failure to initialize database connection before loading view") ;
//            return ;
//         }
//
//         ICollection aList = null ;
//         try
//         {
//            aList = GetArchives(instance) ;
//         }
//         catch(Exception)
//         {
//         }
//
//         if((aList != null) && (aList.Count != 0))
//         {
//            foreach(ArchiveRecord archive in aList)
//            {
//               AddArchiveTreeNode(parentNode, archive) ;
//            }
//         }
//      }

//      static public void
//         AddArchiveTreeNode(
//         SQLcomplianceTreeNode parent,
//         ArchiveRecord archive
//         )
//      {
//         SQLcomplianceTreeNode node = new SQLcomplianceTreeNode(archive.DisplayName,
//                                                                UIConstants.Icon_Archive,
//                                                                UIConstants.Icon_Archive,
//                                                                CMNodeType.Archive) ;
//
//         node.Tag = archive ;
//
//         node.SetMenuFlag(CMMenuItem.Properties) ;
//         node.SetMenuFlag(CMMenuItem.Refresh) ;
//         node.SetMenuFlag(CMMenuItem.DetachArchive) ;
//
//         node.SetMenuFlag(CMMenuItem.AttachArchive) ;
//
//
//         parent.Nodes.Add(node) ;
//      }

      #endregion

      #region System Database Table

      //--------------------------------------------------------------
      // RemoveArchiveDatabase
      //--------------------------------------------------------------
      static public bool
         RemoveArchiveDatabase(
         string databaseName
         )
      {
         bool retval = false ;

         try
         {
            string selectQuery = String.Format("DELETE FROM {0} WHERE databaseName = {1} and databaseType='Archive'",
                                               CoreConstants.RepositorySystemDatabaseTable,
                                               SQLHelpers.CreateSafeString(databaseName)) ;
            SqlCommand cmd = new SqlCommand(selectQuery, Globals.Repository.Connection) ;
            cmd.ExecuteNonQuery() ;
            retval = true ;
         }
         catch(Exception ex)
         {
            ErrorMessage.Show(UIConstants.AppTitle,
                              UIConstants.Error_RemovingArchiveDatabase,
                              ex.Message) ;
         }

         return retval ;
      }

      //--------------------------------------------------------------
      // DropArchiveDatabase
      //--------------------------------------------------------------
      static public bool
         DropArchiveDatabase(
         string databaseName
         )
      {
         bool retval = false ;

         try
         {
            string sql = String.Format("DROP DATABASE {0}",
                                       SQLHelpers.CreateSafeDatabaseName(databaseName)) ;
            SqlCommand cmd = new SqlCommand(sql, Globals.Repository.Connection) ;
            cmd.ExecuteNonQuery() ;
            retval = true ;
         }
         catch(SqlException sqlEx)
         {
            if(sqlEx.Number == 3701)
            {
               // database no longer exists - suppress error message
               retval = true ;
            }
            else
            {
               ErrorMessage.Show(UIConstants.AppTitle,
                                 UIConstants.Error_RemovingArchiveDatabase,
                                 sqlEx.Message) ;
            }
         }
         catch(Exception ex)
         {
            ErrorMessage.Show(UIConstants.AppTitle,
                              UIConstants.Error_RemovingArchiveDatabase,
                              ex.Message) ;
         }

         return retval ;
      }

      //--------------------------------------------------------------
      // IsSQLsecureOwnedDB - Checks if a database is a SQLsecure owned db
      //--------------------------------------------------------------
      static public bool
         IsSQLsecureOwnedDB(
         string dbName
         )
      {
         bool retval = false ;

         try
         {
            string selectQuery = String.Format("SELECT count(*) FROM {0} WHERE databaseName = {1}",
                                               CoreConstants.RepositorySystemDatabaseTable,
                                               SQLHelpers.CreateSafeString(dbName)) ;

            SqlCommand cmd = new SqlCommand(selectQuery, Globals.Repository.Connection) ;
            int count ;

            object obj = cmd.ExecuteScalar() ;
            if(obj is DBNull)
               count = 0 ;
            else
               count = (int)obj ;


            if(count != 0)
               retval = true ;
         }
         catch(Exception ex)
         {
            ErrorMessage.Show(UIConstants.AppTitle,
                              UIConstants.Error_ReadingSQLcomplianceDatabases,
                              ex.Message) ;
         }

         return retval ;
      }

      #endregion

      #region Domains visible to SQL Server

      public ICollection
         GetDomains()
      {
         IList domainList ;

         try
         {
            string cmdstr = "exec master..xp_ntsec_enumdomains" ;
            using(SqlCommand cmd = new SqlCommand(cmdstr, Globals.Repository.Connection))
            {
               using(SqlDataReader reader = cmd.ExecuteReader())
               {
                  domainList = new ArrayList() ;

                  while(reader.Read())
                  {
                     string domain ;

                     domain = SQLHelpers.GetString(reader, 0) ;
                     domainList.Add(domain) ;
                  }
               }
            }
         }
         catch(Exception ex)
         {
            ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                    "GetDomains",
                                    ex) ;
            _errMsg = ex.Message ;
            domainList = new ArrayList() ;
         }

         return domainList ;
      }

      #endregion

      //--------------------------------------------------------------------
      // AddToRole
      //--------------------------------------------------------------------
      static public void
         AddToRole(
         string login,
         string rolename
         )
      {
         string sql = String.Format("exec master..sp_addsrvrolemember {0}, {1}",
                                    SQLHelpers.CreateSafeString(login),
                                    SQLHelpers.CreateSafeString(rolename)) ;
         SqlCommand cmd = new SqlCommand(sql, Globals.Repository.Connection) ;
         cmd.ExecuteNonQuery() ;
      }

      //--------------------------------------------------------------------
      // RemoveFromRole
      //--------------------------------------------------------------------
      static public void
         RemoveFromRole(
         string login,
         string rolename
         )
      {
         string sql = String.Format("exec master..sp_dropsrvrolemember {0}, {1}",
                                    SQLHelpers.CreateSafeString(login),
                                    SQLHelpers.CreateSafeString(rolename)) ;
         SqlCommand cmd = new SqlCommand(sql, Globals.Repository.Connection) ;
         cmd.ExecuteNonQuery() ;
      }

      //--------------------------------------------------------------------
      // AddLogin
      //--------------------------------------------------------------------
      static public void
         AddLogin(
         string storedProcName,
         string loginName
         )
      {
         string sql = String.Format("exec master..{0} {1}",
                                    storedProcName,
                                    SQLHelpers.CreateSafeString(loginName)) ;
         SqlCommand cmd = new SqlCommand(sql, Globals.Repository.Connection) ;
         cmd.ExecuteNonQuery() ;
      }

      //
      // GetEventTypes()
      //
      // This function retrieives the EventTypes table information for use throughout
      //  the GUI
      //
      private void GetEventTypes()
      {
         _sqlServerEventCategories = RulesDal.SelectAllEventCategories(_connection) ;
         _sqlServerEventTypes = new Dictionary<int,CMEventType>() ;
         foreach(CMEventCategory category in _sqlServerEventCategories.Values)
         {
            foreach(CMEventType evType in category.EventTypes)
            {
               _sqlServerEventTypes.Add(evType.TypeId, evType) ;
            }
         }

         _changeLogEventTypes = new Dictionary<int,string>() ;
         string sQuery = String.Format("SELECT eventId,Name from {0}..{1}",
                                       CoreConstants.RepositoryDatabase, CoreConstants.RepositoryChangeLogEventTypeTable) ;
         using(SqlCommand cmd = new SqlCommand(sQuery, _connection))
         {
            using(SqlDataReader reader = cmd.ExecuteReader())
            {
               while(reader.Read())
               {
                  int id ;
                  string name ;

                  id = reader.GetInt32(0) ;
                  name = reader.GetString(1) ;
                  _changeLogEventTypes[id] = name ;
               }
            }
         }

         _agentLogEventTypes = new Dictionary<int,string>() ;
         sQuery = String.Format("SELECT eventId,Name from {0}..{1}",
                                CoreConstants.RepositoryDatabase, CoreConstants.RepositoryAgentEventTypeTable) ;
         using(SqlCommand cmd = new SqlCommand(sQuery, _connection))
         {
            using(SqlDataReader reader = cmd.ExecuteReader())
            {
               while(reader.Read())
               {
                  int id ;
                  string name ;

                  id = reader.GetInt32(0) ;
                  name = reader.GetString(1) ;
                  _agentLogEventTypes[id] = name ;
               }
            }
         }
      }

      public CMEventCategory LookupEventCategory(int id)
      {
         if(_sqlServerEventCategories.ContainsKey(id))
            return _sqlServerEventCategories[id] ;
         else
            return _sqlServerEventCategories[-1] ;
      }

      public CMEventType LookupEventType(int id)
      {
         if(_sqlServerEventTypes.ContainsKey(id))
            return _sqlServerEventTypes[id] ;
         else
            return _sqlServerEventTypes[-1] ;
      }

      public string LookupChangeLogEventType(int id)
      {
         if(_changeLogEventTypes.ContainsKey(id))
            return _changeLogEventTypes[id] ;
         else
            return "Unknown" ;
      }

      public string LookupAgentLogEventType(int id)
      {
         if(_agentLogEventTypes.ContainsKey(id))
            return _agentLogEventTypes[id] ;
         else
            return "Unknown" ;
      }

      public List<CMEventCategory> GetEventCategories()
      {
         return new List<CMEventCategory>(_sqlServerEventCategories.Values) ;
      }

      public List<CMEventCategory> GetExcludableEventCategories()
      {
         List<CMEventCategory> retVal = new List<CMEventCategory>();
         foreach (CMEventCategory c in _sqlServerEventCategories.Values)
            if (c.Excludable)
               retVal.Add(c);
         return retVal ;
      }
   }
}