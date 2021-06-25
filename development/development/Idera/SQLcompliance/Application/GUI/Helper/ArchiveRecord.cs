using System;
using System.Data.SqlClient;


using Idera.SQLcompliance.Core;


namespace Idera.SQLcompliance.Application.GUI.Helper
{
	/// <summary>
	/// Summary description for ArchiveRecord.
	/// </summary>
	public class ArchiveRecord
	{
	   #region Constructor
	   
		public ArchiveRecord()
		{
		  instance     = "";
		  displayName  = "";
		  databaseName = ""; 
		  description  = "";
		}
		
		#endregion
		
		#region Private Properties
		
		private string       instance;
		private string       displayName;
		private string       databaseName;
		private string       description;
		
      private int          bias;
      private int          standardBias;
      private DateTime     standardDate;
      private int          daylightBias;
      private DateTime     daylightDate;
		
		private int          defaultAccess;
		private int          containsBadEvents;
		
		private DateTime     startDate;
		private DateTime     endDate;
		
      private DateTime     timeLastIntegrityCheck;
      private int          lastIntegrityCheckResult;
      
      private int          sqlComplianceDbSchemaVersion;
      private int          eventDbSchemaVersion;
		
		
		#endregion
		
		#region Public Properties
		
		public string     Instance
		{
		   get { return instance;  }
		   set { instance = value; }
		}
		
		public string  DisplayName
		{
		   get { return displayName;  }
		   set { displayName = value; }
		}
		
		public string  DatabaseName
		{
		   get { return databaseName;  }
		   set { databaseName = value; }
		}
		
		public string  Description
		{
		   get { return description;  }
		   set { description = value; }
		}
		
		public DateTime  StartDate
		{
		   get { return startDate;  }
		   set { startDate = value; }
		}
		public DateTime  EndDate
		{
		   get { return endDate;  }
		   set { endDate = value; }
		}
		
		public DateTime  TimeLastIntegrityCheck
		{
		   get { return timeLastIntegrityCheck;  }
		   set { timeLastIntegrityCheck = value; }
		}
		public int LastIntegrityCheckResult
		{
			get { return lastIntegrityCheckResult;  }
			set {	lastIntegrityCheckResult = value; }
		}
		
		public int Bias
		{
			get { return bias;  }
			set {	bias = value; }
		}
		public int StandardBias
		{
			get { return standardBias;  }
			set {	standardBias = value; }
		}
		public DateTime StandardDate
		{
			get { return standardDate;  }
			set {	standardDate = value; }
		}
		public DateTime DaylightDate
		{
			get { return daylightDate;  }
			set {	daylightDate = value; }
		}
		public int DaylightBias
		{
			get { return daylightBias;  }
			set {	daylightBias = value; }
		}
		
		public int DefaultAccess
		{
		   get { return defaultAccess; }
		   set { defaultAccess = value; }
		}
		
		public int ContainsBadEvents
		{
		   get { return containsBadEvents; }
		   set { containsBadEvents = value; }
		}

      public int SqlComplianceDbSchemaVersion
      {
         get { return sqlComplianceDbSchemaVersion; }
         set { sqlComplianceDbSchemaVersion = value; }
      }

      public int EventDbSchemaVersion
      {
         get { return eventDbSchemaVersion; }
         set { eventDbSchemaVersion = value; }
      }
		
		#endregion
		
		#region Update Properties
		
		static public void
		   UpdateArchiveProperties(
		      SqlConnection     conn,
		      string            instance,
		      string            databaseName,
		      string            displayName,
		      string            description,
		      int               newDefaultAccess,
		      int               oldDefaultAccess
		   )
		{
		   string      sql     = "";
         SqlCommand  cmd     = null;
		   
		   try
		   {
		      // write record to SystemDatabases
		      sql = String.Format( "UPDATE {0}..{1} SET displayName={2},description={3} " +
		                           "WHERE instance={4} AND databaseName={5};",
		                                 CoreConstants.RepositoryDatabase,
		                                 CoreConstants.RepositorySystemDatabaseTable,
		                                 SQLHelpers.CreateSafeString(displayName),
		                                 SQLHelpers.CreateSafeString(description),
		                                 SQLHelpers.CreateSafeString(instance),
		                                 SQLHelpers.CreateSafeString(databaseName) );
            cmd = new SqlCommand( sql,conn);
            cmd.ExecuteNonQuery();
            
            // update archive database
            if ( newDefaultAccess != oldDefaultAccess )
            {
                  EventDatabase.SetDefaultSecurity( databaseName,
                                                   newDefaultAccess,
                                                   oldDefaultAccess,
                                                   true,
                                                   conn );
            }
            
		      sql = String.Format( "UPDATE {0}..{1} SET displayName={2},description={3},defaultAccess={4};",
		                           SQLHelpers.CreateSafeDatabaseName(databaseName),
		                           CoreConstants.RepositoryArchiveMetaTable,
		                           SQLHelpers.CreateSafeString(displayName),
		                           SQLHelpers.CreateSafeString(description),
		                           newDefaultAccess );
            cmd = new SqlCommand( sql,conn);
            cmd.ExecuteNonQuery();
         }
         catch (Exception ex)
         {
            ErrorLog.Instance.Write( sql,ex );
            throw ex;
         }
		}
		      
		
		#endregion
		
		#region SystemDatabase IO
		
      //-------------------------------------------------------------------------------------------
      // ReadSystemDatabaseRecord - reads metadata for an archive from SystemDatabases table
      //                            and creates an ArchiveRecord for it
      //-------------------------------------------------------------------------------------------
		static public ArchiveRecord
		   ReadSystemDatabaseRecord(
		      SqlConnection     conn,
		      string            databaseName
		   )
		{
         return null;
		}
		
      //-------------------------------------------------------------------------------------------
      // WriteSystemDatabaseRecord - writes metadata in archiverecord back to SystemDatabase
      //-------------------------------------------------------------------------------------------
		public void
		   WriteSystemDatabaseRecord(
		      SqlConnection     conn
		   )
		{
		   string      sql     = "";
         SqlCommand  cmd     = null;
		   
		   try
		   {
		      // get database id
            sql = String.Format( "SELECT dbid from master..sysdatabases where name={0};",
                                 SQLHelpers.CreateSafeString(this.databaseName) );
            cmd = new SqlCommand( sql, conn );
            
            System.Int16 sqlDatabaseId;
            
			   object obj = cmd.ExecuteScalar();
			   if( obj is System.DBNull )
				   throw new Exception(String.Format("Database {0} not found.", this.DatabaseName ) );
			   else
				   sqlDatabaseId = (System.Int16)obj;
		      
		      // write record to SystemDatabases
		      sql = String.Format( "INSERT INTO {0}..{1} (sqlDatabaseId,databaseName,instance,displayName,description,databaseType) " +
		                                 "VALUES ({2},{3},{4},{5},{6},'Archive')",
		                                 CoreConstants.RepositoryDatabase,
		                                 CoreConstants.RepositorySystemDatabaseTable,
		                                 sqlDatabaseId,
		                                 SQLHelpers.CreateSafeString(this.databaseName),
		                                 SQLHelpers.CreateSafeString(this.instance),
		                                 SQLHelpers.CreateSafeString(this.displayName),
		                                 SQLHelpers.CreateSafeString(this.description) );
            cmd = new SqlCommand( sql,conn);
            cmd.ExecuteNonQuery();
         }
         catch (Exception ex)
         {
            ErrorLog.Instance.Write( sql,ex );
            throw ex;
         }
		}
		
		#endregion
		
		#region EventDatabase IO

      //-------------------------------------------------------------------------------------------
      // ReadArchiveMetaRecord - reads metadata for an archive and creates an ArchiveRecord for it
      //-------------------------------------------------------------------------------------------
		static public ArchiveRecord
		   ReadArchiveMetaRecord(
		      SqlConnection     conn,
		      string            inDatabaseName
		   )
		{
		   ArchiveRecord  newArchive = null;
		   string         selectSQL = "";
		   
		   try
		   {
		      selectSQL = String.Format( "SELECT instance,displayName,description,startDate,endDate,"+
		                                        "bias,standardBias,standardDate,daylightBias,daylightDate," +
		                                        "defaultAccess,containsBadEvents,timeLastIntegrityCheck,lastIntegrityCheckResult," +
		                                        "sqlComplianceDbSchemaVersion,eventDbSchemaVersion " +
		                                 "FROM {0}..{1}",
		                                 SQLHelpers.CreateSafeDatabaseName(inDatabaseName),
		                                 CoreConstants.RepositoryArchiveMetaTable );
            using ( SqlCommand cmd = new SqlCommand( selectSQL,conn) )
            {
               using ( SqlDataReader reader = cmd.ExecuteReader() )
               {
                  if ( reader.Read() )
                  {
                     int col=0;
                     newArchive = new ArchiveRecord();
                      
                     newArchive.databaseName      = inDatabaseName;
		               newArchive.instance          = SQLHelpers.GetString(reader,col++);
		               newArchive.displayName       = SQLHelpers.GetString(reader,col++);
		               newArchive.description       = SQLHelpers.GetString(reader,col++);
		               newArchive.startDate         = SQLHelpers.GetDateTime(reader,col++);
		               newArchive.endDate           = SQLHelpers.GetDateTime(reader,col++);
		               
		               newArchive.bias              = SQLHelpers.GetInt32(reader,col++);
		               newArchive.standardBias      = SQLHelpers.GetInt32(reader,col++);
		               newArchive.standardDate      = SQLHelpers.GetDateTime(reader,col++);
		               newArchive.daylightBias      = SQLHelpers.GetInt32(reader,col++);
		               newArchive.daylightDate      = SQLHelpers.GetDateTime(reader,col++);
		               
		               newArchive.defaultAccess     = SQLHelpers.GetInt32(reader,col++);
		               newArchive.containsBadEvents = SQLHelpers.ByteToInt(reader,col++);
		               
		               newArchive.timeLastIntegrityCheck   = SQLHelpers.GetDateTime(reader,col++);
		               newArchive.lastIntegrityCheckResult = SQLHelpers.GetInt32(reader,col++);
		               
		               newArchive.sqlComplianceDbSchemaVersion = SQLHelpers.GetInt32(reader,col++);
		               newArchive.eventDbSchemaVersion         = SQLHelpers.GetInt32(reader,col++);
		            }
		         }
		      }
         }
         catch (Exception ex)
         {
            ErrorLog.Instance.Write( selectSQL,
                                     ex );
            newArchive = null;
         }
         
         return newArchive;
		}
		
		#endregion
   }
}
