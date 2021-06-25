using System ;
using System.Collections.Generic ;
using System.Windows.Forms ;
using Idera.SQLcompliance.Application.GUI.Controls ;
using Idera.SQLcompliance.Application.GUI.Helper ;
using Idera.SQLcompliance.Application.GUI.SQL ;
using Idera.SQLcompliance.Core ;
using Idera.SQLcompliance.Core.Agent ;
using Idera.SQLcompliance.Core.Collector ;
using Idera.SQLcompliance.Core.Stats ;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
   partial class Form_Main
   {
      public void ShowAgentProperties(ServerRecord server)
      {
         // read current server record
         server.Connection = Globals.Repository.Connection;
         if (!server.Read(server.Instance))
         {
            MessageBox.Show(String.Format(UIConstants.Error_LoadingServerProperties,
                                          ServerRecord.GetLastError()),
                            Text,
                            MessageBoxButtons.OKCancel,
                            MessageBoxIcon.Error);
            return;
         }

         Form_AgentProperties frm = new Form_AgentProperties(server);
         if (frm.ShowDialog() == DialogResult.OK)
         {
         }
      }

      public void ShowServerProperties(ServerRecord server)
      {
         ShowServerProperties(server, Form_ServerProperties.Context.General);
      }

      public void ShowServerProperties(ServerRecord server, Form_ServerProperties.Context context)
      {
         // read current server record
         server.Connection = Globals.Repository.Connection;
         if (!server.Read(server.Instance))
         {
            MessageBox.Show(String.Format(UIConstants.Error_LoadingServerProperties,
                                          ServerRecord.GetLastError()),
                            Text,
                            MessageBoxButtons.OKCancel,
                            MessageBoxIcon.Error);
         }
         else
         {
            Form_ServerProperties frm = new Form_ServerProperties(server);
            frm.SetContext(context);
            if (frm.ShowDialog(this) == DialogResult.OK)
            {
               RaiseServerModified(new ServerEventArgs(frm.newServer));
            }
            // These are different because they are stored in different tables
            //  and have different ramifications.  Thresholds are not used anywhere
            //  but the GUI currently, so are not considered part of an audit snapshot
            if (frm.ThresholdsDirty)
            {
               List<ReportCardRecord> reportCards = ReportCardRecord.GetReportCardEntries(Globals.Repository.Connection);
               foreach (ReportCardRecord reportCard in reportCards)
               {
                  if (_statistics.ContainsKey((StatsCategory)reportCard.StatisticId))
                  {
                     EnterpriseStatistics entStats = _statistics[(StatsCategory)reportCard.StatisticId];
                     ServerStatistics serverStats = entStats.GetServerStatistics(reportCard.SrvId);
                     serverStats.Threshold = reportCard;
                  }
               }
            }

         }
      }

      public void ShowDatabaseProperties(DatabaseRecord database)
      {
         ShowDatabaseProperties(database, Form_DatabaseProperties.Context.General);
      }

      public void ShowDatabaseProperties(DatabaseRecord database, Form_DatabaseProperties.Context context)
      {
         // read current database record
         database.Connection = Globals.Repository.Connection;
         if (!database.Read(database.DbId))
         {
            MessageBox.Show(String.Format(UIConstants.Error_LoadingDatabaseProperties,
                                          DatabaseRecord.GetLastError()),
                            Text,
                            MessageBoxButtons.OKCancel,
                            MessageBoxIcon.Error);
         }
         else
         {
            Form_DatabaseProperties frm = new Form_DatabaseProperties(database);
            frm.SetContext(context);
            if (frm.ShowDialog(this) == DialogResult.OK)
            {
               RaiseDatabaseModified(new DatabaseEventArgs(frm.db)) ;
            }
         }
      }

      public void CheckAgent(ServerRecord server)
      {
         Cursor = Cursors.WaitCursor;

         try
         {
            // ping agent
            string url = String.Format("tcp://{0}:{1}/{2}",
                                        server.AgentServer,
                                        server.AgentPort,
                                        typeof(AgentCommand).Name);
            AgentCommand agentCmd = (AgentCommand)Activator.GetObject(typeof(AgentCommand), url);
            agentCmd.Ping();

            ServerRecord.UpdateLastAgentContact(server.Instance);

            MessageBox.Show("A SQLcompliance Agent is active on computer '" +
                             server.AgentServer +
                             "'.",
                             "Check SQLcompliance Agent");
         }
         catch (Exception)
         {
            if (server.isClustered)
            {
               MessageBox.Show("The SQLcompliance Agent on computer '" +
                               server.AgentServer +
                               "' failed to respond. The SQLcompliance Agent service may not be running or may be inaccessible.");
            }
            else
            {
               DialogResult choice = MessageBox.Show("The SQLcompliance Agent on computer '" +
                                                     server.AgentServer +
                                                     "' failed to respond. The SQLcompliance Agent service may not be running.\n\nDo you want to attempt to start the SQLcompliance Agent service?",
                                                     "Check SQLcompliance Agent",
                                                     MessageBoxButtons.YesNo);
               if (choice == DialogResult.Yes)
               {
                  StartAgentService(server.AgentServer);
               }
            }
         }
         finally
         {
            Cursor = Cursors.Default;
         }
      }

      public void StartAgentService(string agentServer)
      {
         Cursor = Cursors.WaitCursor;

         Form_ServiceProgress frm = new Form_ServiceProgress(agentServer, true);
         frm.ShowDialog();

         bool started = frm.Success;

         if (started)
         {
            MessageBox.Show(String.Format(UIConstants.Info_AgentStarted,
                                          agentServer),
                            UIConstants.Title_StartAgent);
         }
         else
         {
            MessageBox.Show(String.Format(UIConstants.Error_CantStartAgent,
                                          agentServer,
                                          frm.ErrorMessage),
                            UIConstants.Title_StartAgent);
         }
         Cursor = Cursors.Default;
      }

      public void ChangeAgentTraceDirectory(ServerRecord server)
      {
         // read current server record
         server.Connection = Globals.Repository.Connection;
         if (!server.Read(server.Instance))
         {
            MessageBox.Show(String.Format(UIConstants.Error_LoadingServerProperties,
                                          ServerRecord.GetLastError()),
                            Text,
                            MessageBoxButtons.OKCancel,
                            MessageBoxIcon.Error);
            return;
         }

         Form_AgentTraceDirectory frm = new Form_AgentTraceDirectory(server);
         if (frm.ShowDialog() == DialogResult.OK)
         {
         }
      }

      public void DoSnapshot(string instance)
      {
         Form_Snapshot frm = new Form_Snapshot(instance);
         frm.ShowDialog();
      }

      public void UpdateNow(string instance)
      {
         this.Cursor = Cursors.WaitCursor;

         try
         {
            string url = String.Format("tcp://{0}:{1}/{2}",
                                          Globals.SQLcomplianceConfig.Server,
                                          Globals.SQLcomplianceConfig.ServerPort,
                                          typeof(AgentManager).Name);

            AgentManager manager = (AgentManager)Activator.GetObject(
               typeof(AgentManager),
               url);

            manager.UpdateAuditConfiguration(instance);
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

      public void ChangeTraceDirectory(ServerRecord record)
      {
         Form_AgentTraceDirectory frm = new Form_AgentTraceDirectory(record);
         frm.ShowDialog();
      }

      public void CheckIntegrity(string targetInstance)
      {
         Form_IntegrityCheck frm = new Form_IntegrityCheck(targetInstance);
         frm.ShowDialog();
      }

      public void NavigateToView(ConsoleViews view)
      {
         switch (view)
         {
            case ConsoleViews.EnterpriseSummary:
               _explorerBar.SelectedGroup = _explorerBar.Groups[0];
               _mainTabView.ShowTab(MainTabView.Tabs.ChangeLog);
               break;
            case ConsoleViews.EnterpriseAlerts:
               _explorerBar.SelectedGroup = _explorerBar.Groups[0];
               _mainTabView.ShowTab(MainTabView.Tabs.ChangeLog);
               break;
            case ConsoleViews.EnterpriseChangeLog:
               _explorerBar.SelectedGroup = _explorerBar.Groups[0];
               _mainTabView.ShowTab(MainTabView.Tabs.ChangeLog);
               break ;
            case ConsoleViews.EnterpriseActivityLog:
               _explorerBar.SelectedGroup = _explorerBar.Groups[0];
               _mainTabView.ShowTab(MainTabView.Tabs.ChangeLog);
               break;
            case ConsoleViews.ServerSummary:
               _explorerBar.SelectedGroup = _explorerBar.Groups[0];
               _mainTabView.ShowTab(MainTabView.Tabs.ChangeLog);
               break;
            case ConsoleViews.ServerAlerts:
               _explorerBar.SelectedGroup = _explorerBar.Groups[0];
               _mainTabView.ShowTab(MainTabView.Tabs.ChangeLog);
               break;
            case ConsoleViews.ServerEvents:
               _explorerBar.SelectedGroup = _explorerBar.Groups[0];
               _mainTabView.ShowTab(MainTabView.Tabs.ChangeLog);
               break;
            case ConsoleViews.ServerArchive:
               _explorerBar.SelectedGroup = _explorerBar.Groups[0];
               _mainTabView.ShowTab(MainTabView.Tabs.ChangeLog);
               break;
            case ConsoleViews.ServerChangeLog:
               _explorerBar.SelectedGroup = _explorerBar.Groups[0];
               _mainTabView.ShowTab(MainTabView.Tabs.ChangeLog);
               break;
            case ConsoleViews.DatabaseSummary:
               _explorerBar.SelectedGroup = _explorerBar.Groups[0];
               _mainTabView.ShowTab(MainTabView.Tabs.ChangeLog);
               break;
            case ConsoleViews.DatabaseEvents:
               _explorerBar.SelectedGroup = _explorerBar.Groups[0];
               _mainTabView.ShowTab(MainTabView.Tabs.ChangeLog);
               break;
            case ConsoleViews.DatabaseArchive:
               _explorerBar.SelectedGroup = _explorerBar.Groups[0];
               _mainTabView.ShowTab(MainTabView.Tabs.ChangeLog);
               break;
            case ConsoleViews.AdminRegisteredServers:
               _explorerBar.SelectedGroup = _explorerBar.Groups[2];
               _treeAdmin.SelectedNode = _treeAdmin.Nodes[0];
               break;
            case ConsoleViews.AdminAlertRules:
               _explorerBar.SelectedGroup = _explorerBar.Groups[2];
               _treeAdmin.SelectedNode = _treeAdmin.Nodes[1];
               break;
            case ConsoleViews.AdminEventFilters:
               _explorerBar.SelectedGroup = _explorerBar.Groups[2];
               _treeAdmin.SelectedNode = _treeAdmin.Nodes[2];
               break;
            case ConsoleViews.AdminLogins:
               _explorerBar.SelectedGroup = _explorerBar.Groups[2];
               _treeAdmin.SelectedNode = _treeAdmin.Nodes[3];
               break;
            case ConsoleViews.AdminActivityLog:
               _explorerBar.SelectedGroup = _explorerBar.Groups[2];
               _treeAdmin.SelectedNode = _treeAdmin.Nodes[4];
               break;
            case ConsoleViews.AdminChangeLog:
               _explorerBar.SelectedGroup = _explorerBar.Groups[2];
               _treeAdmin.SelectedNode = _treeAdmin.Nodes[5];
               break;
         }
      }

      #region Server Actions

      internal bool RemoveServerAction(ServerRecord srv)
      {
         if (Server.RemoveServer(srv))
         {
            RaiseServerRemoved(new ServerEventArgs(srv)) ;
            return true;
         }
         return false;
      }

      public void AddServerAction()
      {
         Form_ServerNew dlg = new Form_ServerNew();
         if (DialogResult.OK == dlg.ShowDialog())
         {
            RaiseServerAdded(new ServerEventArgs(dlg.srv));
            if (dlg.DesiredDeploymentSuccess)
            {
               DialogResult choice = MessageBox.Show(UIConstants.Prompt_AddDatabasesNow,
                  UIConstants.Title_AddDatabasesNow,
                  MessageBoxButtons.YesNo);
               if (choice == DialogResult.Yes)
               {
                  AddDatabaseAction(dlg.srv);
               }
            }
         }
      }

      public void EnableServersAction(List<ServerRecord> servers)
      {
         foreach (ServerRecord server in servers)
            EnableServerAction(server);
      }

      //-------------------------------------------------------------------------
      // EnableServerAction
      //-------------------------------------------------------------------------
      public void EnableServerAction(ServerRecord srv)
      {
         srv.Connection = Globals.Repository.Connection;
         if (srv.EnableServer(true))
         {
            // Register change to server and perform audit log				      
            ServerUpdate.RegisterChange(srv.SrvId, LogType.EnableServer, srv.Instance);
            RaiseServerModified(new ServerEventArgs(srv));
         }
         else
         {
            ErrorMessage.Show(this.Text,
                               UIConstants.Error_CantChangeAuditingStatus,
                               ServerRecord.GetLastError());
         }
      }

      public void DisableServersAction(List<ServerRecord> servers)
      {
         if (DialogResult.OK != MessageBox.Show(UIConstants.Warning_DisableAuditedServers,
                                                UIConstants.Title_DisableAuditedServers,
                                                MessageBoxButtons.OKCancel,
                                                MessageBoxIcon.Warning))
            return;

         foreach (ServerRecord srv in servers)
            DisableServerAction(srv, false);
      }

      public void DisableServerAction(ServerRecord srv)
      {
         DisableServerAction(srv, true);
      }

      private void DisableServerAction(ServerRecord srv, bool promptUser)
      {
         if (promptUser)
         {
            if (DialogResult.OK != MessageBox.Show(UIConstants.Warning_DisableAuditedServers,
                                                   UIConstants.Title_DisableAuditedServers,
                                                   MessageBoxButtons.OKCancel,
                                                   MessageBoxIcon.Warning))
               return;
         }
         srv.Connection = Globals.Repository.Connection;
         if (srv.EnableServer(false))
         {
            // Register change to server and perform audit log				      
            ServerUpdate.RegisterChange(srv.SrvId, LogType.DisableServer, srv.Instance);
            RaiseServerModified(new ServerEventArgs(srv));
         }
         else
         {
            ErrorMessage.Show(this.Text,
                               UIConstants.Error_CantChangeAuditingStatus,
                               ServerRecord.GetLastError());
         }
      }

      #endregion Server Actions

      #region Database Actions

      public void AddDatabaseAction(ServerRecord server)
      {
         Form_DatabaseNew frm;
         frm = new Form_DatabaseNew(server);
         if (frm.ShowDialog(this) == DialogResult.OK)
         {
            foreach (DatabaseRecord db in frm.DatabaseList)
               RaiseDatabaseAdded(new DatabaseEventArgs(db));
         }
      }

      internal bool RemoveDatabasesAction(List<DatabaseRecord> databases)
      {
         if (DialogResult.OK != MessageBox.Show(UIConstants.Warning_RemoveAuditedDatabases,
                                                  UIConstants.Title_RemoveAuditedDatabases,
                                                  MessageBoxButtons.OKCancel,
                                                  MessageBoxIcon.Warning))
            return false;
         foreach (DatabaseRecord record in databases)
            RemoveDatabaseAction(record, false);
         return true;
      }

      internal bool RemoveDatabaseAction(DatabaseRecord dbItem)
      {
         return RemoveDatabaseAction(dbItem, true);
      }

      private bool RemoveDatabaseAction(DatabaseRecord dbItem, bool promptUser)
      {
         // warning
         if (promptUser)
         {
            if (DialogResult.OK != MessageBox.Show(UIConstants.Warning_RemoveAuditedDatabases,
                                                     UIConstants.Title_RemoveAuditedDatabases,
                                                     MessageBoxButtons.OKCancel,
                                                     MessageBoxIcon.Warning))
               return false;

         }
         this.Cursor = Cursors.WaitCursor;

         try
         {
            dbItem.Connection = Globals.Repository.Connection;
            if (Server.RemoveDatabase(dbItem))
            {
               RaiseDatabaseRemoved(new DatabaseEventArgs(dbItem));
               return true;
            }
         }
         finally
         {
            this.Cursor = Cursors.Default;
         }
         return false;
      }

      public void EnableDatabasesAction(List<DatabaseRecord> databases)
      {
         foreach (DatabaseRecord database in databases)
            EnableDatabaseAction(database);
      }

      public void EnableDatabaseAction(DatabaseRecord database)
      {
         database.Connection = Globals.Repository.Connection;
         if (database.Enable(true))
         {
            // Register change to server and perform audit log				      
            ServerUpdate.RegisterChange(database.SrvId, LogType.EnableDatabase, database.SrvInstance, database.Name);
            RaiseDatabaseModified(new DatabaseEventArgs(database));
         }
         else
         {
            ErrorMessage.Show(this.Text,
                              UIConstants.Error_CantChangeAuditingStatus,
                              DatabaseRecord.GetLastError());
         }
      }

      public void DisableDatabasesAction(List<DatabaseRecord> databases)
      {
         if (DialogResult.OK != MessageBox.Show(UIConstants.Warning_DisableAuditedDatabases,
                                            UIConstants.Title_DisableAuditedDatabases,
                                            MessageBoxButtons.OKCancel,
                                            MessageBoxIcon.Warning))
            return;

         foreach (DatabaseRecord database in databases)
            DisableDatabaseAction(database, false);
      }

      public void DisableDatabaseAction(DatabaseRecord database)
      {
         DisableDatabaseAction(database, true);
      }

      private void DisableDatabaseAction(DatabaseRecord database, bool promptUser)
      {
         if (promptUser)
         {
            if (DialogResult.OK != MessageBox.Show(UIConstants.Warning_DisableAuditedDatabases,
                                               UIConstants.Title_DisableAuditedDatabases,
                                               MessageBoxButtons.OKCancel,
                                               MessageBoxIcon.Warning))
               return;
         }
         database.Connection = Globals.Repository.Connection;
         if (database.Enable(false))
         {
            // Register change to server and perform audit log				      
            ServerUpdate.RegisterChange(database.SrvId, LogType.DisableDatabase, database.SrvInstance, database.Name);
            RaiseDatabaseModified(new DatabaseEventArgs(database));
         }
         else
         {
            ErrorMessage.Show(this.Text,
                              UIConstants.Error_CantChangeAuditingStatus,
                              DatabaseRecord.GetLastError());
         }
      }

      #endregion Database Actions

      //-------------------------------------------------------------------------
      // RemoveArchiveAction
      //-------------------------------------------------------------------------
      public void DetachArchiveAction(ArchiveRecord archive)
      {
         // warning
         DialogResult choice;

         choice = MessageBox.Show(UIConstants.Warning_RemoveArchive,
                                   UIConstants.Title_RemoveArchive,
                                   MessageBoxButtons.YesNo,
                                   MessageBoxIcon.Warning);
         if (choice != DialogResult.Yes)
         {
            return;
         }

         if (SQLRepository.RemoveArchiveDatabase(archive.DatabaseName))
         {
            // remove or refresh instance node as appropriate
            ServerRecord server = new ServerRecord();
            server.Connection = Globals.Repository.Connection;
            server.Read(archive.Instance);
            int archiveCount = server.CountLoadedArchives(Globals.Repository.Connection);

            if ((!server.IsAuditedServer) && (0 == archiveCount))
            {
               // kill server record
               ServerRecord.DeleteServerRecord(Globals.Repository.Connection, server.Instance);
               RaiseServerRemoved(new ServerEventArgs(server));
            }

            // Build snapshot
            string snapshot = String.Format("Detach archive database: {0}\r\n\r\n", archive.DatabaseName);

            LogRecord.WriteLog(Globals.Repository.Connection,
                                LogType.DetachArchive,
                                archive.Instance,
                                snapshot);
            RaiseArchiveDetached(new ArchiveEventArgs(archive));
         }
      }



      public void AttachArchiveAction()
      {
         Form_ArchiveImport frm = new Form_ArchiveImport();
         if (DialogResult.OK == frm.ShowDialog())
         {
            if (frm.serverAdded)
               RaiseServerAdded(new ServerEventArgs(frm.serverRecord));
            RaiseArchiveAttached(new ArchiveEventArgs(frm.m_archive));
         }
      }
   }
}