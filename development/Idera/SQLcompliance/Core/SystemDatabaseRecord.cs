using System;
using System.Collections ;
using System.Data.SqlClient ;

namespace Idera.SQLcompliance.Core
{
	/// <summary>
	/// Summary description for SystemDatabaseRecord.
	/// </summary>
	public class SystemDatabaseRecord
	{
      private string _instance ;
      private string _databaseName ;
      private string _type ;
      private DateTime _created ;
      private int _databaseId ;
      private string _displayName ;
      private string _description ;

      public string Instance
      {
         get { return _instance ; }
         set { _instance = value ; }
      }

      public string DatabaseName
      {
         get { return _databaseName ; }
         set { _databaseName = value ; }
      }

      public string DatabaseType
      {
         get { return _type ; }
         set { _type = value ; }
      }

	   public DateTime Created
	   {
	      get { return _created ; }
	      set { _created = value ; }
	   }

	   public int DatabaseId
	   {
	      get { return _databaseId ; }
	      set { _databaseId = value ; }
	   }

	   public string DisplayName
	   {
	      get { return _displayName ; }
	      set { _displayName = value ; }
	   }

	   public string Description
	   {
	      get { return _description ; }
	      set { _description = value ; }
	   }

      //
      // Read()
      //
      // Read all records from the SystemDatabases table in the repository.
      //
      public static SystemDatabaseRecord[] Read(SqlConnection conn)
      {
         return InternalRead(conn) ;
      }

      public static SystemDatabaseRecord Read(SqlConnection conn, string databaseName)
      {
         SystemDatabaseRecord[] retVal ;
         string whereClause = String.Format("databaseName={0}", SQLHelpers.CreateSafeString(databaseName)) ;

         retVal = InternalRead(conn, whereClause, "") ;
         if(retVal != null && retVal.Length > 0)
            return retVal[0] ;
         else
            return null ;
      }

      private static SystemDatabaseRecord[] InternalRead(SqlConnection conn)
      {
         return InternalRead(conn, "", "") ;
      }

      private static SystemDatabaseRecord[] InternalRead(SqlConnection conn, 
         string whereClause, string sortClause)
      {
         ArrayList list = new ArrayList() ;
         if(whereClause.Length > 0)
            whereClause = " WHERE " + whereClause ;
         if(sortClause.Length > 0)
            sortClause = " SORT BY " + sortClause ;

         string cmdstr = String.Format("SELECT instance, databaseName, databaseType, " +
            "dateCreated, sqlDatabaseId, displayName, description " +
            "FROM {0}..{1} {2} {3}",
            CoreConstants.RepositoryDatabase,
            CoreConstants.RepositorySystemDatabaseTable,
            whereClause, sortClause);

         using(SqlCommand cmd = new SqlCommand(cmdstr, conn))
         {
            cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
            using(SqlDataReader reader = cmd.ExecuteReader())
            {
               while ( reader.Read() )
               {
                  SystemDatabaseRecord record = new SystemDatabaseRecord() ;
                  int i = 0 ;
                  record.Instance = SQLHelpers.GetString(reader, i++) ;
                  record.DatabaseName = SQLHelpers.GetString(reader, i++) ;
                  record.DatabaseType = SQLHelpers.GetString(reader, i++) ;
                  record.Created = SQLHelpers.GetDateTime(reader, i++) ;
                  record.DatabaseId = SQLHelpers.GetInt16(reader, i++) ;
                  record.DisplayName = SQLHelpers.GetString(reader, i++) ;
                  record.Description = SQLHelpers.GetString(reader, i++) ;
                  list.Add(record) ;
               }
            }
         }
         SystemDatabaseRecord[] retVal = new SystemDatabaseRecord[list.Count];
         for(int i = 0 ; i < list.Count ; i++)
            retVal[i] = (SystemDatabaseRecord)list[i] ;

         return retVal ;
      }
	}
}
