using System ;
using System.Collections ;
using System.Diagnostics ;
using System.Windows.Forms ;
using Idera.SQLcompliance.Application.GUI.Controls ;
using Idera.SQLcompliance.Application.GUI.Forms ;
using Idera.SQLcompliance.Core ;

namespace Idera.SQLcompliance.Application.GUI.Helper
{
	/// <summary>
	/// Summary description for DatabaseRecordGUI.
	/// </summary>
	public class DatabaseRecordGUI
	{
	   #region Constructor
	   
		public DatabaseRecordGUI()
		{
		}
		
		#endregion
		
      //-------------------------------------------------------------------
      // ShowNewWizard - Display New Database Wizard
      //                 Writes database to repository for first time
      //-------------------------------------------------------------------
      public static bool
         ShowCreateWizard(
            int               serverId,
            out string        instance
         )
      {
         return ShowCreateWizard( serverId,
                                  out instance,
                                  false);
      }

      public static bool
         ShowCreateWizard(
            int               serverId,
            out string        instance,
            bool              allowOverride
         )
      {
         bool retval = false;
         instance = null;
         /*
         Form_DatabaseNew wiz = new Form_DatabaseNew( serverId );
         
         wiz.allowOverride = allowOverride;
         
         if ( wiz.ShowDialog() == DialogResult.OK )
         {
            instance = wiz.currServer;
            retval = true;
         }
         else
         {
            instance = "";
         }*/
         
         return retval;
      }

      //-------------------------------------------------------------
      // LoadDatabaseList
      //--------------------------------------------------------------
//      static public void
//         LoadDatabaseList(
//            ListView         listDatabases,
//            string           whereClause,
//            bool             statusColumn,
//            bool             includeServer,
//            bool             serverEnabled
//         )
//      {
//         listDatabases.Items.Clear();
//         
//         if ( Globals.Repository.Connection == null )
//         {
//            Debug.Write( "Assertion - Failure to initialize database connection before loading view" );
//            return;
//         }
//
//         ICollection dbList = null;
//         try
//         {         
//            dbList = DatabaseRecord.GetDatabases( Globals.Repository.Connection,
//                                                  whereClause );
//         }
//#if DEBUG
//         catch ( Exception ex )
//         {
//            MessageBox.Show ( String.Format( "Error loading audited databases for server: {0}", ex.Message ) );
//#else
//         catch ( Exception )
//         {
//#endif         
//         }
//
//			if ((dbList != null) && (dbList.Count != 0)) 
//			{
//				foreach (DatabaseRecord db in dbList) 
//				{
//				   string status;
//				   int statusImage;
//				   
//				   if ( db.IsEnabled && serverEnabled )
//				   {
//				      status      = UIConstants.Status_Enabled;
//				      statusImage = UIConstants.Icon_DBEnabled;
//				   }
//				   else
//				   {
//				      status      = UIConstants.Status_Disabled;
//				      statusImage = UIConstants.Icon_DBDisabled;
//				   }
//				   
//				   string name;
//				   if ( includeServer )
//				   {
//				      name = String.Format( @"{0}\{1}", db.SrvInstance, db.Name);
//				   }
//				   else
//				   {
//				      name = db.Name;
//				   }
//				   
//               ListViewItem item = new ListViewItem( name,
//                                                                     statusImage );
//               item.Tag = db;
//               
//               listDatabases.Items.Add(item );
//               
//               if ( statusColumn ) item.SubItems.Add( status );
//               
//               item.SubItems.Add( db.Description );
//            }
//            
//            // select first object
//            if ( listDatabases.Items.Count != 0 )
//            {
//               listDatabases.Items[0].EnsureVisible();
//               listDatabases.Items[0].Selected = true;
//            }
//         }
//      }
	   
      //-------------------------------------------------------------------
      // LoadDatabaseTreeNode - Workhorse routine to load DBs under a node
      //                        in the tree. Input is WHERE clause for
      //                        different parts of tree
      //-------------------------------------------------------------------
//      static public void
//         LoadDatabaseTreeNode( 
//            SQLcomplianceTreeNode parentNode,
//            string            whereClause,
//            bool              includeServer
//         )
//      {
//         SQLcomplianceTreeNode node;
//         
//         if ( Globals.Repository.Connection == null )
//         {
//            Debug.Write( "Assertion - Failure to initialize database connection before loading view" );
//            return;
//         }
//
//         ICollection dbList = null;
//         try
//         {         
//            dbList = DatabaseRecord.GetDatabases( Globals.Repository.Connection,
//                                                  whereClause );
//         }
//#if DEBUG
//         catch ( Exception ex )
//         {
//            MessageBox.Show ( String.Format( "Error loading audited databases: {0}", ex.Message ) );
//#else
//         catch ( Exception )
//         {
//#endif         
//         }
//
//			if ((dbList != null) && (dbList.Count != 0)) 
//			{
//   		   int statusImage;
//  		      string name;
//   		   
//				foreach (DatabaseRecord db in dbList) 
//				{
//				   if ( includeServer )
//				   {
//				      name = String.Format( @"{0}\{1}", db.SrvInstance, db.Name);
//				   }
//				   else
//				   {
//				      name = db.Name;
//				   }
//				   
//				   
//				   if ( db.IsEnabled && parentNode.IsEnabled )
//				   {
//				      statusImage = UIConstants.Icon_DBEnabled;
//				   }
//				   else
//				   {
//				      statusImage = UIConstants.Icon_DBDisabled;
//				   }
//
//               node = new SQLcomplianceTreeNode( name,
//                                             statusImage,
//                                             statusImage,
//                                             CMNodeType.Database );
//               node.Tag       = db.Name;
//               node.Instance  = db.SrvInstance;
//               node.IsEnabled = db.IsEnabled;
//               node.DatabaseId = db.SqlDatabaseId ;
//               
//               node.SetMenuFlag( CMMenuItem.Refresh );
//               node.SetMenuFlag( CMMenuItem.Properties );
//               node.SetMenuFlag( CMMenuItem.Delete );
//               node.SetMenuFlag( CMMenuItem.Enable );
//               node.SetMenuFlag( CMMenuItem.Disable );
//               node.SetMenuFlag( CMMenuItem.NewDatabase );
//               
//               parentNode.Nodes.Add( node );                                             
//            }
//         }
//      }
      
      //-------------------------------------------------------------------
      // ResetDatabaseTreeIcon
      //-------------------------------------------------------------------
//      static public void
//         ResetDatabaseTreeIcon( 
//            SQLcomplianceTreeNode node,
//            bool              enable
//         )
//      {
//         SQLcomplianceTreeNode parent = (SQLcomplianceTreeNode)node.Parent;
// 		   int imageIndex;
// 		   
//	      if ( enable && parent.IsEnabled )
//			{
//				imageIndex = UIConstants.Icon_DBEnabled;
//			}
//			else
//			{
//				imageIndex = UIConstants.Icon_DBDisabled;
//			}
//			
//			node.ImageIndex         = imageIndex;
//			node.SelectedImageIndex = imageIndex;
//			node.IsEnabled          = enable;
//      }
	}
}
