using System ;
using System.Collections ;
using System.Data.SqlClient ;
using System.Windows.Forms ;
using Idera.SQLcompliance.Core ;
using Idera.SQLcompliance.Core.Agent ;
using Idera.SQLcompliance.Core.Collector ;
using Idera.SQLcompliance.Core.Status ;

namespace Idera.SQLcompliance.Application.GUI.Helper
{
	/// <summary>
	/// Summary description for Server.
	/// </summary>
	public static class Server
	{
      #region Server - Remove
      
      //-------------------------------------------------------------
      // RemoveServer
      //--------------------------------------------------------------
      static public bool RemoveServer(ServerRecord         srv)
      {
         // warning
         DialogResult choice;
         choice = MessageBox.Show( (srv.isClustered) ? UIConstants.Warning_RemoveClusteredServer
                                                     : UIConstants.Warning_RemoveServer,
                                   UIConstants.Title_RemoveServer, 
                                   MessageBoxButtons.YesNo );
         if ( choice == DialogResult.No ) return false;
         
         bool removeEventsDatabase = false;
         choice = MessageBox.Show( UIConstants.Warning_RemoveEventsDatabase,
                                   UIConstants.Title_RemoveEventsDatabase, 
                                   MessageBoxButtons.YesNoCancel,
                                   MessageBoxIcon.Warning );
         if ( choice == DialogResult.Cancel ) return false;
         if ( choice == DialogResult.No )    removeEventsDatabase = true;

         
         
         //---------------------
         // Deactivate instance
         //---------------------
         if ( ! InternalDeactivateAgent( srv, removeEventsDatabase ) )
         {
            return false;
         }
         
         //-------------------------
         // Kill server & databases            
         //-------------------------
                  
         // reset active control (otherwise we get busy database message)
         if (removeEventsDatabase)
         {
            /*
            if ( mainForm.activeControl == mainForm.eventView )
            {
               mainForm.activeControl.Reset();
            }*/
         }
         
         bool bRemoved = InternalRemoveServer( srv, removeEventsDatabase );
         
         if ( bRemoved )
         {
            // Register change to server and perform audit log				      
            AgentStatusMsg.LogStatus( srv.AgentServer,
                                      srv.Instance,
                                      AgentStatusMsg.MsgType.Unregistered,
                                      Globals.Repository.Connection );

            //-----------------
            // Uninstall agent
            //-----------------
            InternalUninstallAgent( srv );
         }
         else
         {
            // server remove failed - errmsg already shown - exit
            return false;
         }
        
         MessageBox.Show( String.Format( "SQL Server {0} has been removed.", srv.Instance ),
                          "Remove SQL Server" );
         
         return true;
      }
      
      //-------------------------------------------------------------
      // InternalDeactivateAgent - deactivate instance at agent
      //--------------------------------------------------------------
      static internal bool
         InternalDeactivateAgent(
            ServerRecord  srv,
            bool          removeEventsDatabase
         )
      {
         srv.Connection = Globals.Repository.Connection;
         
         bool           bRemoved     = false;
         bool           serverUp     = false;
         string         url          ;
         ServerRecord   oldSrv;
         
         try
         {
            if ( srv.IsAuditedServer && srv.IsDeployed )
            {
               ServerManager srvManager = null;
               
               // check for collection service - cant uninstall if it is down or unreachable
               try
               {
                   srvManager = GUIRemoteObjectsProvider.ServerManager();
                  srvManager.Ping();
                  serverUp = true;
               }
               catch ( Exception ex )
               {
                  // allow them to continue even if agent cant be contacted and stopped
                  DialogResult choice = MessageBox.Show(
                     String.Format( UIConstants.Error_CouldntContactCollectionService,
                                    UIUtils.TranslateRemotingException( Globals.SQLcomplianceConfig.Server,
                                                                        UIConstants.CollectionServiceName,
                                                                        ex ) ),
                                                         UIConstants.Title_RemoveServer,
                                                         MessageBoxButtons.YesNo,
                                                         MessageBoxIcon.Error );
                  if ( choice == DialogResult.No )
                  {
                     return false;                  
                  }
               }

               try
               {
                  // stop agent
                  if ( serverUp )
                  {
                     // server up - get it to stop agent (gets us through firewalls)
                      AgentManager manager = GUIRemoteObjectsProvider.AgentManager();
                     manager.Deactivate(srv.Instance, removeEventsDatabase);
                  }
                  else
                  {
                     // server down - try direct to agent stop request
                     AgentCommand agentCmd = GUIRemoteObjectsProvider.AgentCommand(srv.AgentServer, srv.AgentPort);
                     agentCmd.Deactivate(srv.Instance, removeEventsDatabase);
                  }
               }
               catch ( Exception ex )
               {
                  // allow them to continue even if agent cant be contacted and stopped
                  string msg;
                  if ( serverUp )
                  {
                     msg = UIUtils.TranslateRemotingException( Globals.SQLcomplianceConfig.Server,
                                                               UIConstants.CollectionServiceName,
                                                               ex );
                  }
                  else
                  {
                     msg = UIUtils.TranslateRemotingException( srv.AgentServer,
                                                               UIConstants.AgentServiceName,
                                                               ex );

                  }
                  
                  DialogResult choice = MessageBox.Show( String.Format( UIConstants.Info_CouldntStopAgent,
                                                                        srv.Instance,
                                                                        msg ),
                                                         UIConstants.Title_RemoveServer,
                                                         MessageBoxButtons.YesNo,
                                                         MessageBoxIcon.Error );
                  if ( choice == DialogResult.No )
                  {
                     return false;                  
                  }
               }
            
               // stop trace jobs
               if ( serverUp )
               {
                  srvManager.StopTraceJobs( srv.Instance );
               }
            }
            
            // mark server as inactive
            oldSrv = srv.Clone();
            srv.IsRunning         = false;
            srv.IsEnabled         = false;
            srv.Write( oldSrv );
            
            bRemoved = true;
         }
         catch (Exception ex )
         {
            // TODO: Better error message for error deleting server
            ErrorMessage.Show( UIConstants.Title_RemoveServer,
                               String.Format( UIConstants.Error_DeleteServerProblem,
                                              srv.Instance,
                                              ex.Message ) );
         }
         
         return bRemoved;
      }         
      
      //------------------------
      // InternalUninstallAgent
      //------------------------
      static internal bool
         InternalUninstallAgent(
            ServerRecord  srv
         )
      {
         srv.Connection = Globals.Repository.Connection;
         
         bool           bRemoved     = false;
         ServerRecord   oldSrv;
         
         
         try
         {
            //-----------------------------------------
            // Check for cases where we skip uninstall
            //-----------------------------------------
            if ( 0 != srv.CountSharedInstances(Globals.Repository.Connection ) ) return true;
            if ( srv.IsDeployedManually )
            {
               MessageBox.Show( String.Format( UIConstants.Info_ManualUninstallRequired,
                                                srv.Instance ),
                                 UIConstants.Title_RemoveServer,
                                 MessageBoxButtons.OK,
                                 MessageBoxIcon.Information );
               return true;
            }
            
            if ( ! srv.IsDeployed )       return true;   // not deployed; dont undeploy
            if ( srv.IsOnRepositoryHost ) return true;   // leave agent on repository at all times
            
            //--------------
            // Do Uninstall
            //--------------
            try
            {
               AgentControl.RemoteUninstallAgentService( null, null,
                                                          srv.AgentServer,
                                                          null, null );
            }
            catch( Exception ex )
            {
			      ErrorMessage.Show( UIConstants.Title_UninstallAgent,
			                         UIConstants.Error_CantUninstallAgent,
			                         ex.Message );
            }
                     
            oldSrv = srv.Clone();
            srv.IsDeployed = false;
            srv.IsRunning = false;
            srv.Write( oldSrv );
            
            bRemoved = true;
         }
         catch (Exception ex )
         {
            ErrorMessage.Show( UIConstants.Title_RemoveServer,
                               String.Format( UIConstants.Error_UninstallAgent,
                                              srv.Instance,
                                              ex.Message ) );
         }
         finally
         {
            if ( bRemoved )
            {
               // agent deployed - update all instances on that computer
               ICollection servers = ServerRecord.GetServers( Globals.Repository.Connection,
                                                              true );
               foreach ( ServerRecord srvrec in servers )
               {
                  if ( srvrec.InstanceServer.ToUpper() == srv.InstanceServer.ToUpper() )
                  {
                     ServerRecord oldServerState = srvrec.Clone();
                     if ( oldServerState.IsDeployed )
                     {                     
                        srvrec.IsRunning            = false;
                        srvrec.IsDeployed           = false;
                        srvrec.Write( oldServerState );
                     
                        AgentStatusMsg.LogStatus( srvrec.AgentServer,
                                                  srvrec.Instance,
                                                  AgentStatusMsg.MsgType.Unregistered,
                                                  Globals.Repository.Connection );
                     }
                  }
               }
            }
         }
         return bRemoved;
      }         
      
      //-------------------------------------------------------------
      // InternalRemoveServer - Delete server and its events from 
      //                        repository
      //--------------------------------------------------------------
      static internal bool
         InternalRemoveServer(
            ServerRecord  srv,
            bool          deleteEventsDatabase
         )
      {
         srv.Connection = Globals.Repository.Connection;
         
         bool           bRemoved     = false;
         
         try
         {
            // remove server out of configuration tables (if no archives loaded!)
            if ( 0 == srv.CountLoadedArchives(Globals.Repository.Connection ) )
            {
                DeleteUserForServer(srv);
               // no archives; delete it and all dbs
               srv.Delete();
            }
            else
            {
               // mark as non-audited db since it is still hosting archives
               ServerRecord oldSrv = srv.Clone();
               srv.IsAuditedServer = false;
               srv.Write( oldSrv );
               
               // delete all audit dbs
               srv.DeleteAuditData( null );
            }
            
            // delete events database
            if ( deleteEventsDatabase )
            {
               try
               {
                  srv.DeleteEventsDatabase();
               }
               catch ( Exception ex )
               {
                  ErrorMessage.Show( UIConstants.Title_RemoveServer,
                                     UIConstants.Error_RemoveEventsDatabase,
                                     ex.Message );
               }
               
               //srv.DeleteChangeLogs();
               
               Globals.Repository.ResetConnection();
            }
            
            ServerRecord old = srv.Clone();
            srv.EventDatabase   = "";
            srv.Write( old );
            
            bRemoved = true;
            
         }
         catch (Exception ex )
         {
            ErrorMessage.Show( UIConstants.Title_RemoveServer,
                               String.Format( UIConstants.Error_RemoveEventsServerProblem,
                                              srv.Instance,
                                              ex.Message ) );
         }
         finally
         {
            if ( bRemoved )
            {
               // Register change to server and perform audit log				      
				   ServerUpdate.LogChange( LogType.RemoveServer,
                                       srv.Instance );
               if ( deleteEventsDatabase )
               {
				      ServerUpdate.LogChange( LogType.RemoveEventsDatabase,
                                          srv.Instance );
               }                                       
            }
         }
         return bRemoved;
      }
      
        static internal void DeleteUserForServer(ServerRecord srv)
        {
            string querySQL = srv.GetDeleteSQLForUser(srv.SrvId);
            querySQL += srv.GetDeleteSQLForUserDatabases(srv.SrvId);
            string errorMsg;
            try
            {
                if (!srv.DeleteUser(querySQL))
                {
                    errorMsg = ServerRecord.GetLastError();
                    throw (new Exception(errorMsg));
                }
            }
            catch(Exception ex)
            {
                errorMsg = ex.Message;
            }
        }
      #endregion
      
      #region RemoveDatabase
      
      //-------------------------------------------------------------
      // RemoveDatabase
      //--------------------------------------------------------------
      static public bool
         RemoveDatabase(
            DatabaseRecord    db
         )
      {
         bool           bremoved = false;
         SqlTransaction transaction ;
         
         
		  using(transaction = Globals.Repository.Connection.BeginTransaction())
		  {
			  try
			  {
				  if ( db.Delete(transaction ) )
				  {
					  if ( DatabaseObjectRecord.DeleteUserTables( db.DbId,
						  transaction) )
					  {
						  bremoved = true;
					  }
				  }
			  }
			  finally
			  {
				  if ( bremoved )
				  {
					  transaction.Commit();
               
					  // Register change to server and perform audit log				      
					  ServerUpdate.RegisterChange( db.SrvId,
						  LogType.RemoveDatabase,
						  db.SrvInstance,
						  db.Name );
				  }
				  else
				  {
					  transaction.Rollback();
					  ErrorMessage.Show( UIConstants.AppTitle,
						  UIConstants.Error_CantDeleteDatabase,
						  DatabaseRecord.GetLastError() );
				  }
			  }
		  }
         
         return bremoved;
      }
      
      #endregion
      
      
      //-------------------------------------------------------------
      // ForceCollection
      //-------------------------------------------------------------
      static public void
         ForceCollection(
            string   instance
         )
      {
         if  ( instance== "" ) return;
         
         DialogResult choice;         
         choice = MessageBox.Show( UIConstants.Warning_ForceCollection,
                                   UIConstants.Title_ForceCollection, 
                                   MessageBoxButtons.OKCancel );
         if ( choice == DialogResult.Cancel ) return;
         
         
         try
         {
            AgentManager manager = GUIRemoteObjectsProvider.AgentManager();
               
            manager.CollectTracesNow( instance );
         }
         catch (Exception ex )
         {
            ErrorMessage.Show( UIConstants.Title_ForceCollection,
                               UIConstants.Error_ForceCollectionFailed,
                               UIUtils.TranslateRemotingException( Globals.SQLcomplianceConfig.Server,
                                                                   UIConstants.CollectionServiceName,
                                                                   ex ),
                               MessageBoxIcon.Error );
         }
      }
	}
}
