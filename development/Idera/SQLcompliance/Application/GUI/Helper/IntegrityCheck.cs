using System;
using System.Windows.Forms;
using System.Data.SqlClient;

using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Application.GUI.Forms;

namespace Idera.SQLcompliance.Application.GUI.Helper
{
	/// <summary>
	/// Summary description for IntegrityCheck.
	/// </summary>
	public class IntegrityCheck
	{
		private IntegrityCheck()
		{
		}

      //-----------------------------------------------------------------
      // CheckAndRepair
      //-----------------------------------------------------------------		
      static public bool
         CheckAndRepair(
            string   caption,
            string   operation,
            string   instance,
            out bool hasIntegrity
         )
      {
         bool succeeded ;
         Repository rep    = new Repository();
         
         try
         {
            rep.OpenConnection();
            
            string database ;
            
            string sql =String.Format( "SELECT eventDatabase FROM {0} WHERE instance = {1}",
                                       CoreConstants.RepositoryServerTable,
                                       SQLHelpers.CreateSafeString( instance ) );
            using ( SqlCommand cmd = new SqlCommand( sql, Globals.Repository.Connection ) )
            {
               object obj = cmd.ExecuteScalar();
               if ( obj is System.DBNull )
                  throw new Exception( String.Format( "Events database for SQL Server {0} not found.", instance ) ); 
               else
                  database = (string)obj;
            }                                       
            
            succeeded = CheckAndRepair( caption,
                                           operation,
                                           instance,
                                           database,
                                           false,
                                           out hasIntegrity );
         }
         catch (Exception ex )
         {
            succeeded    = false;
            hasIntegrity = false;
            
            ErrorMessage.Show( caption,
                               String.Format( "Error retrieving information about SQL Server instance: {0} necessary to perform an integrity check. The operation has been aborted.",
                                              instance ),
                               ex.Message );
         }
         finally
         {
            rep.CloseConnection();
         }
         
         return succeeded;
      }

      //-----------------------------------------------------------------
      // CheckAndRepair
      //-----------------------------------------------------------------		
      static public bool
         CheckAndRepair(
            string   caption,
            string   operation,
            string   instance,
            string   databaseName,
            bool     isArchive,
            out bool hasIntegrity
         )
      {
         bool   succeeded   ;
         
         Form_IntegrityProgress frmCheck
            = new Form_IntegrityProgress( instance,
                                          databaseName,
                                          isArchive,
                                          false );
         frmCheck.ShowDialog();
         if ( frmCheck.callFailed )
         {
            // Call failed for some reason - server down or something
            ErrorMessage.Show( caption,
                               frmCheck.errMsg1,
                               frmCheck.errMsg2,
                               MessageBoxIcon.Error );
            hasIntegrity = false;                               
            return false;
         }
         
         if ( frmCheck.checkResult.intact ) 
         {
            //------------------------
            // Integrity Check Passed
            //------------------------
            if ( operation == "" )
            {
               MessageBox.Show( String.Format( CoreConstants.Info_IntegrityCheckPassed, databaseName),
                              caption );
            }
            
            hasIntegrity = true;
            return true;
         }

         //------------------------------------------------------------------------------------
         // Integrity Check Failed - since it failed in no fix mode ask if they want to fix it
         //------------------------------------------------------------------------------------
         if ( frmCheck.checkResult.integrityCheckError != "")
         {
            // an error occurred checking integrity
            ErrorMessage.Show( caption,
                               CoreConstants.Error_IntegrityCheckError,
                               frmCheck.checkResult.integrityCheckError );
            hasIntegrity = false;                               
            return false;
         }
         
         //------------------------
         // Integrity Failure Form
         //------------------------
         string failureCaption;
         if ( operation == "" )
         {
            failureCaption = String.Format( "(Database: {0})", databaseName );
         }
         else
         {
            failureCaption = String.Format( "({0})", instance );
         }
         
         Form_IntegrityFailure frm = new Form_IntegrityFailure( frmCheck.checkResult,
                                                                frmCheck.badEvents,
                                                                frmCheck.badEventTypes,
                                                                operation,
                                                                failureCaption );
         DialogResult choice = frm.ShowDialog();
         if ( choice == DialogResult.Yes )
         {
            //--------------------------------
            // Integrity Repair Progress Form
            //--------------------------------
            Form_IntegrityProgress frmRepair
               = new Form_IntegrityProgress( instance,
                                             databaseName,
                                             isArchive,
                                             true );
            frmRepair.ShowDialog();
            
            if ( frmRepair.callFailed )
            {
               ErrorMessage.Show( caption,
                                  frmRepair.errMsg1,
                                  frmRepair.errMsg2,
                                  MessageBoxIcon.Error );
               succeeded    = false;
               hasIntegrity = false;
            }
            else
            {
               if ( operation == "" )
               {
                  MessageBox.Show( CoreConstants.Info_IntegrityRepaired, caption );
               }

               // problems fixed - we are back to a state of integrity               
               succeeded    = true;
               hasIntegrity = true;
            }
         }
         else
         {
            succeeded    = true;
            hasIntegrity = false;
         }
         return succeeded;
      }
	}
}
