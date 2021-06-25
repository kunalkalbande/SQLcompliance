using System ;
using System.Collections ;
using System.Data.SqlClient ;
using System.Diagnostics ;
using System.Text ;
using System.Windows.Forms ;
using Idera.SQLcompliance.Core ;

namespace Idera.SQLcompliance.Application.GUI.Helper
{
	/// <summary>
	/// Summary description for DatabaseObjectRecord.
	/// </summary>
	public class DatabaseObjectRecord
	{
	   #region Constructor
	   
		public DatabaseObjectRecord()
		{
		  objectId    = -1;
		  dbId        = -1;
		  
		  name        = "";
		  id          = -1;
		  objectType  = -1;
		}

		public DatabaseObjectRecord( RawTableObject table )
		{
			objectId   = -1; // -1 means not in repository yet
		
 	      dbId       = -1;
			name       = table.TableName;
         schemaName = table.SchemaName;
			id         = table.id;
			objectType = Table;
      }
		
		#endregion
		
		public static int Table       = 0;
		public static int SystemTable = 1;
		public static int View        = 2;
		
		#region Private Properties
		
		private int      objectId;
		private int      dbId;
		
		private string   name;
		private int      id;         // id in SQL Server
		private int      objectType; // Type of object: Table, etc
      private string   schemaName;
		
		
		#endregion
		
		#region Public Properties
		
		public int     DbId
		{
		   get { return dbId;  }
		   set { dbId = value; }
		}
		
		public int     ObjectId
		{
		   get { return objectId;  }
		   set { objectId = value; }
		}
		
		public string  TableName
		{
		   get { return name;  }
		   set { name = value; }
		}

      public string SchemaName
      {
         get { return schemaName; }
         set { schemaName = value; }
      }

      public string FullTableName
      {
          get
          {
              return name.StartsWith(schemaName+".") ?
                  name : string.Format("{0}.{1}", schemaName, name);
          }
      }

		public int Id
		{
		   get { return id;  }
		   set { id = value; }
		}
		
		public int     ObjectType
		{
		   get { return objectType;  }
		   set { objectType = value; }
		}
		
		#endregion
		
		#region LastError
		
	   static string           errMsg   = "";
	   static public  string   GetLastError() { return errMsg; } 
	   
	   #endregion
		
	   #region static Public Routines

	   //-----------------------------------------------------------------------------
	   // GetUserTables
	   //-----------------------------------------------------------------------------
	   static public ICollection
	      GetUserTables(
	         int              dbId
         )
      {
         string where = String.Format( "dbId={0} AND objectType=0", dbId );
         return GetDatabaseObjects( where );
      }
      
	   //-----------------------------------------------------------------------------
	   // DeleteUserTables
	   //-----------------------------------------------------------------------------
      static public bool
         DeleteUserTables(
            int                      dbId,
            SqlTransaction           transaction
         )
      {
         bool retval       = false;
         
         try
         {
            string sqlStr = GetDeleteAllSQL(dbId, true);
            
		      SqlCommand  cmd = new SqlCommand( sqlStr,
		                                        Globals.Repository.Connection );
		      if ( transaction != null )
		      {
		         cmd.Transaction = transaction; 
		      }
		      
            cmd.ExecuteNonQuery();
            
            retval = true;
         }
         catch( Exception ex )
         {
            errMsg = ex.Message;
         }
         
         return retval;
      }

	   //-----------------------------------------------------------------------------
	   // CreateUserTables
	   //-----------------------------------------------------------------------------
      static public bool
         CreateUserTables(
            ICollection       newItems,
            int               dbId,
            SqlTransaction    transaction
         )
      {
         return UpdateUserTables( newItems,
                                  0,     // no existing rows 
                                  dbId,
                                  transaction );
      }

	   //-----------------------------------------------------------------------------
	   // UpdateUserTables
	   //-----------------------------------------------------------------------------
      static public bool
         UpdateUserTables(
            ICollection              newItems,
            int                      oldCount,
            int                      dbId,
            SqlTransaction           transaction
         )
      {
         bool retval       = false;
         int  expectedRows = 0;
         
         try
         {
            StringBuilder s = new StringBuilder("");
            
            // Delete existing tables
            if ( oldCount > 0 )
            {
               s.Append( GetDeleteAllSQL(dbId, true) );
               expectedRows = oldCount;
            }
            
            // Insert Tables
            foreach ( ListViewItem x in newItems )
            {
               DatabaseObjectRecord dbo = (DatabaseObjectRecord)x.Tag;
               if (dbo.dbId==-1) dbo.dbId = dbId;
               s.Append( dbo.GetInsertSQL(true) );
            }
            expectedRows += newItems.Count;
            
            //---------
            // Execute 
            //---------
		      SqlCommand  cmd = new SqlCommand( s.ToString(), Globals.Repository.Connection );
		      if ( transaction != null )
		      {
		         cmd.Transaction = transaction; 
		      }
		      
            int nRows = cmd.ExecuteNonQuery();
            if ( nRows == expectedRows )
            {
               retval = true;
            }
         }
         catch( Exception ex )
         {
            errMsg = ex.Message;
         }
         
         return retval;
      }

	   //-----------------------------------------------------------------------------
	   // GetDatabaseObjects
	   //-----------------------------------------------------------------------------
	   static public ICollection
	      GetDatabaseObjects(
	         string           whereClause
         )
	   {
   		IList dboList ;
         
         // Load Database Objects
			try
			{
			   string cmdstr = GetSelectSQL( whereClause );

			   using ( SqlCommand    cmd    = new SqlCommand( cmdstr,
			                                                  Globals.Repository.Connection ) )
            {			                                                 
               using ( SqlDataReader reader = cmd.ExecuteReader() )
               {
				      dboList = new ArrayList();

			         while ( reader.Read() )
                  {
                     DatabaseObjectRecord dbo = new DatabaseObjectRecord();
                     dbo.Load( reader );

                     // Add to list               
                     dboList.Add( dbo );
                  }
               }
            }
			}
			catch( Exception ex )
			{
				Debug.Write( String.Format("Error loading list: {0}", ex.Message ) );
				dboList = new ArrayList();  // return an empty array
			}
			
			return dboList;
	   }
	   
	   #endregion
	   
     
      #region Private Methods
      
      //-------------------------------------------------------------
      // Load - Loads DatabaseRecord from SELECT result set
      //--------------------------------------------------------------
      private void
         Load(
            SqlDataReader reader
         )
      {
         this.objectId   = reader.GetInt32(0);
         this.dbId       = reader.GetInt32(1);
         this.name       = SQLHelpers.GetString( reader, 2);
         this.id         = SQLHelpers.GetInt32( reader, 3);
         this.objectType = reader.GetInt32(4);
         this.schemaName = SQLHelpers.GetString(reader, 5);
      }
      
      #endregion
      
      #region SQL Builders
      
      //-------------------------------------------------------------
      // GetSelectSQL
      //--------------------------------------------------------------
      private static string
         GetSelectSQL(       
           string             strWhere )
      {
          string tmp = "SELECT objectId" +
			                  ",dbId" +
			                  ",name" +
			                  ",id" +
			                  ",objectType" +
                           ",schemaName" +
		                     " FROM {0}" +
		                     " {1} {2}" +
                           " ORDER by name ASC;";		                     
		                     
         return string.Format( tmp,
                               CoreConstants.RepositoryDatabaseObjectsTable,
                               (strWhere!="") ? "WHERE " : "",
                               strWhere );
      }
      
      //-------------------------------------------------------------
      // GetInsertSQL
      //--------------------------------------------------------------
      public string
         GetInsertSQL( bool withLocking )
      {
          string tmp = "INSERT INTO {0} {1}" +
                           "(" +
			                     "dbId" +
			                     ",name" +
			                     ",id" +
			                     ",objectType" +
                              ",schemaName" +
			                  ") VALUES ({2},{3},{4},{5},{6});";
		                     
         return string.Format( tmp,
                               CoreConstants.RepositoryDatabaseObjectsTable,
                               (withLocking) ? "WITH (TABLOCKX) " : "",
                               this.dbId,
                               SQLHelpers.CreateSafeString(this.name),
                               this.id,
                               this.objectType,
                               SQLHelpers.CreateSafeString(this.schemaName));
      }

      //-------------------------------------------------------------
      // GetDeleteAllSQL - Create UPDATE SQL to delete a database
      //--------------------------------------------------------------
      public static string
         GetDeleteAllSQL(
            int               databaseId,
            bool              withLocking
         )
      {
         string cmdStr = String.Format( "DELETE FROM {0} {1} where dbId={2};",
                                        CoreConstants.RepositoryDatabaseObjectsTable,
                                       (withLocking) ? "WITH (TABLOCKX) " : "",
                                        databaseId );
         return cmdStr;
      }
      
      #endregion
	}
}
