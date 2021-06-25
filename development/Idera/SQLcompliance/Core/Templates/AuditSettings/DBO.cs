using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;

namespace Idera.SQLcompliance.Core.Templates.AuditSettings
{
   public class DBO
   {
      #region Constructor

      public DBO()
      {
         objectId = -1;
         dbId = -1;

         name = "";
         id = -1;
         objectType = -1;
      }

      public DBO(RawTableObject table)
      {
         objectId = -1; // -1 means not in repository yet

         dbId = -1;
         name = table.TableName;
         id = table.id;
         objectType = Table;
         schemaName = table.SchemaName;
      }

      #endregion

      public static int Table = 0;
      public static int SystemTable = 1;
      public static int View = 2;

      #region Private Properties

      private int objectId;
      private int dbId;

      private string name;
      private int id;         // id in SQL Server
      private int objectType; // Type of object: Table, etc
      private string schemaName;


      #endregion

      #region Public Properties

      public int DbId
      {
         get { return dbId; }
         set { dbId = value; }
      }

      public int ObjectId
      {
         get { return objectId; }
         set { objectId = value; }
      }

      public string Name
      {
         get { return name; }
         set { name = value; }
      }

      public string SchemaName
      {
         get { return schemaName; }
         set { schemaName = value; }
      }

      public int Id
      {
         get { return id; }
         set { id = value; }
      }

      public int ObjectType
      {
         get { return objectType; }
         set { objectType = value; }
      }

      public string FullName
      {
          get { return schemaName + "." + name; }
      }

      #endregion

      #region LastError

      static string errMsg = "";
      static public string GetLastError() { return errMsg; }

      #endregion

      #region static Public Routines


      //-----------------------------------------------------------------------------
      // DeleteUserTables
      //-----------------------------------------------------------------------------
      static public bool
         DeleteUserTables(
            int dbId,
            SqlTransaction transaction
         )
      {
         bool retval = false;

         try
         {
            string sqlStr = GetDeleteAllSQL(dbId, true);

            using( SqlConnection conn = new SqlConnection( Repository.CreateConnectionString( null)))
            {
               conn.Open();
               SqlCommand cmd = new SqlCommand( sqlStr,
                                                conn );
               if ( transaction != null )
               {
                  cmd.Transaction = transaction;
               }

               cmd.ExecuteNonQuery();
            }

            retval = true;
         }
         catch (Exception ex)
         {
            errMsg = ex.Message;
         }

         return retval;
      }

      /*
      //-----------------------------------------------------------------------------
      // CreateUserTables
      //-----------------------------------------------------------------------------
      static public bool
         AddAuditedTables( string connStr,
            List<DBO> newItems,
            int dbId,
            SqlTransaction transaction
         )
      {
         return UpdateUserTables( connStr,
                                  newItems,
                                  0,     // no existing rows 
                                  dbId,
                                  transaction);
      }
      */

      //-----------------------------------------------------------------------------
      // UpdateUserTables
      //-----------------------------------------------------------------------------
      static public bool
         UpdateUserTables(
            SqlConnection conn,
            List<DBO> newItems,
            int oldCount,
            int dbId,
            SqlTransaction transaction,
            bool replace
         )
      {
         bool retval = false;
         int expectedRows = 0;

         try
         {
            StringBuilder s = new StringBuilder("");

            // Delete existing tables
            if (replace && oldCount > 0)
            {
               s.Append(GetDeleteAllSQL(dbId, true));
               expectedRows = oldCount;
            }

            // Insert Tables
            foreach (DBO dbo in newItems)
            {
               if (dbo.dbId == -1) dbo.dbId = dbId;
               s.Append(dbo.GetInsertSQL(true));
            }
            expectedRows += newItems.Count;

            //---------
            // Execute 
            //---------
            if( conn.State == ConnectionState.Closed )
               conn.Open();
            SqlCommand cmd = new SqlCommand( s.ToString(),
                                             conn );
            if ( transaction != null )
            {
               cmd.Transaction = transaction;
            }

            int nRows = cmd.ExecuteNonQuery();
            if (nRows == expectedRows)
            {
               retval = true;
            }
         }
         catch (Exception ex)
         {
            errMsg = ex.Message;
         }

         return retval;
      }

      //-----------------------------------------------------------------------------
      // GetAuditedTables
      //-----------------------------------------------------------------------------
      static public Dictionary<string, DBO>
         GetAuditedTables( string connStr, int dbId )
      {
         string where = String.Format("dbId={0} AND objectType=0", dbId);
         Dictionary<string, DBO> dboList = new Dictionary<string, DBO>();

         // Load Database Objects
         try
         {
            string cmdstr = GetSelectSQL(where);
            using( SqlConnection conn = new SqlConnection( connStr))
            {
               conn.Open();
               using ( SqlCommand cmd = new SqlCommand( cmdstr,
                                                        conn ) )
               {
                  using ( SqlDataReader reader = cmd.ExecuteReader() )
                  {
                     while ( reader.Read() )
                     {
                        DBO dbo = new DBO();
                        dbo.Load( reader );

                        // Add to list               
                        dboList.Add( dbo.FullName,
                                     dbo );
                     }
                  }
               }
            }
         }
         catch (Exception ex)
         {
            Debug.Write(String.Format("Error loading list: {0}", ex.Message));
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
         objectId = reader.GetInt32(0);
         dbId = reader.GetInt32(1);
         name = SQLHelpers.GetString(reader, 2);
         id = SQLHelpers.GetInt32(reader, 3);
         objectType = reader.GetInt32(4);
         schemaName = SQLHelpers.GetString(reader, 5);
      }

      #endregion

      #region SQL Builders

      //-------------------------------------------------------------
      // GetSelectSQL
      //--------------------------------------------------------------
      private static string
         GetSelectSQL(
           string strWhere)
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

         return string.Format(tmp,
                               CoreConstants.RepositoryDatabaseObjectsTable,
                               (strWhere != "") ? "WHERE " : "",
                               strWhere);
      }

      //-------------------------------------------------------------
      // GetInsertSQL
      //--------------------------------------------------------------
      public string
         GetInsertSQL(bool withLocking)
      {
         string tmp = "INSERT INTO {0} {1}" +
                          "(" +
                             "dbId" +
                             ",name" +
                             ",id" +
                             ",objectType" +
                             ",schemaName" +
                          ") VALUES ({2},{3},{4},{5},{6});";

         return string.Format(tmp,
                               CoreConstants.RepositoryDatabaseObjectsTable,
                               (withLocking) ? "WITH (TABLOCKX) " : "",
                               dbId,
                               SQLHelpers.CreateSafeString(name),
                               id,
                               objectType,
                               SQLHelpers.CreateSafeString(schemaName));
      }

      //-------------------------------------------------------------
      // GetDeleteAllSQL - Create UPDATE SQL to delete a database
      //--------------------------------------------------------------
      public static string
         GetDeleteAllSQL(
            int databaseId,
            bool withLocking
         )
      {
         string cmdStr = String.Format("DELETE FROM {0} {1} where dbId={2};",
                                        CoreConstants.RepositoryDatabaseObjectsTable,
                                       (withLocking) ? "WITH (TABLOCKX) " : "",
                                        databaseId);
         return cmdStr;
      }

      #endregion
   }
}
