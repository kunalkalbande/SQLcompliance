using System;
using System.Collections;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Text;

namespace Idera.SQLcompliance.Core.Event
{
	//----------------------------------------------------------------------------
	// Class: EventSqlRecord
	//----------------------------------------------------------------------------
	public class EventSqlRecord
	{
	   #region Constructor
	   
	   //-------------------------------------------------------------------------
	   // EventSqlRecord - Constructor
	   //-------------------------------------------------------------------------
		public
		   EventSqlRecord(
		      SqlConnection     inConnection,
		      string            inServerDB
		   )
		{
		   connection  = inConnection;
		   serverDB    = inServerDB;
		}
		
		public
		   EventSqlRecord()
		{
		}
		
		#endregion
		
		#region Properties

      public int                     eventId        = 0;
      public DateTime                startTime;
      public string                  sqlText;
      public int                     hash           = 0;

      
      // private properties
      SqlConnection                  connection = null;
      string                         serverDB   = "";

		#endregion

		#region LastError
		
	   static string           errMsg   = "";
	   static public  string   GetLastError() { return errMsg; } 
	   
	   #endregion
		
      #region Public Read Methods

	   //-----------------------------------------------------------------------------
	   // Read - reads event record
	   //-----------------------------------------------------------------------------
	   public bool
	      Read(
	         int               inEventId
         )
	   {
         string where = String.Format( "eventId={0}", inEventId );
			return InternalRead( this.connection,
			                     this.serverDB,
			                     where );
      }
      
	   //-----------------------------------------------------------------------------
	   // Read - reads event record
	   //-----------------------------------------------------------------------------
	   public bool
	      Read(
            SqlConnection     inConnection,
            string            inServerDB,
	         int               inEventId
         )
	   {
         string where = String.Format( "eventId={0}", inEventId );
			return InternalRead( inConnection,
			                     inServerDB,
			                     where );
      }
      
	   //-----------------------------------------------------------------------------
	   // GetEventRecords  
	   //-----------------------------------------------------------------------------
      static public ICollection
         GetEventSqlRecords(
            SqlConnection     inConnection,
            string            inServerDB,
            string            whereClause
         )
      {
   		IList         eventSqlList = null;
         
			try
			{
			   string cmdStr = GetSelectSQL( inServerDB,
			                                 whereClause,
			                                 "" );
			   
			   using ( SqlCommand    cmd    = new SqlCommand( cmdStr, inConnection ) ) 
			   {
			      cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
			      
               using ( SqlDataReader reader = cmd.ExecuteReader() )
               {
				      eventSqlList = new ArrayList();

			         while ( reader.Read() )
                  {
                     EventSqlRecord evsqlRec = new EventSqlRecord();
                     evsqlRec.Load( reader );

                     // Add to list               
                     eventSqlList.Add( evsqlRec );
                  }
               }
            }
			}
			catch( Exception ex )
			{
			   errMsg = ex.Message;
				eventSqlList = new ArrayList();  // return an empty array
			}
			
			return eventSqlList;
	   }
	   
	   #endregion
	   
      #region Public Insert Methods
	   
	   //-----------------------------------------------------------------------------
	   // Insert - Insert record into events SQL table
	   //-----------------------------------------------------------------------------
	   public void
	      Insert(
	         SqlConnection     inConnection,
	         string            inDatabaseName
	      )
	   {
	      Insert( inConnection,
	              inDatabaseName,
	              null );
	   }

	   //-----------------------------------------------------------------------------
	   // Insert - Insert record into events table
	   //-----------------------------------------------------------------------------
	   public void
	      Insert(
	         SqlConnection     inConnection,
	         string            inDatabaseName,
	         SqlTransaction    inTransaction
	      )
	   {
	      string sqlCmd = GetInsertSQL(inDatabaseName);
   	   
	      using ( SqlCommand cmd = new SqlCommand( sqlCmd, inConnection ) )
	      {
	         cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
			      
            if ( inTransaction != null )                                            
               cmd.Transaction = inTransaction;
               
            try
            {   
               cmd.ExecuteNonQuery();
            }
            catch (SqlException ex )
            {
               // TODO: remove this exception after testing         
	            ErrorLog.Instance.Write( "Exception during event record SQL: " + ex.Number.ToString(),
	                                    sqlCmd,
	                                    ex,
	                                    true );
               throw ex;
            }
         }
	   }
	   
      #endregion
      
      #region Private Methods

	   //-----------------------------------------------------------------------------
	   // InternalRead
	   //-----------------------------------------------------------------------------
	   private bool
	      InternalRead(
            SqlConnection     inConnection,
            string            inServerDB,
	         string            where
	      )
	   {
	      bool           retval = false;
	      string         cmdstr = "";
	      
	      try
	      {
			   cmdstr = GetSelectSQL( inServerDB,
			                          where,
			                          "" );

			   using ( SqlCommand    cmd    = new SqlCommand( cmdstr, inConnection ) )
			   {
			      cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
			      
               using ( SqlDataReader reader = cmd.ExecuteReader() )
               {
			         if ( reader.Read() )
                  {
                     Load( reader );
                     retval = true;
                  }
               }
            }
	      }
	      catch (Exception ex )
	      {
	        errMsg = ex.Message;
			}
	   
	      return retval;
	   }
     
      //-------------------------------------------------------------
      // Load
      //--------------------------------------------------------------
      public void
         Load(
            SqlDataReader reader
         )
      {
         int col=0;
         eventId   = SQLHelpers.GetInt32(     reader, col++);
         startTime = SQLHelpers.GetDateTime(  reader, col++);
         sqlText   = SQLHelpers.GetString(    reader, col++);
         hash      = SQLHelpers.GetInt32(     reader, col++);
      }
      
      #endregion
      
      #region SQL
      
      //-------------------------------------------------------------
      // GetSelectSQL
      //--------------------------------------------------------------
      private static string
         GetSelectSQL(       
           string             serverDBName,
           string             strWhere,
           string             strOrder
         )
      {
         return GetSelectSQL( serverDBName,
                              CoreConstants.RepositoryEventSqlTable,
                              strWhere,
                              strOrder );
      }
      
      //-------------------------------------------------------------
      // GetSelectSQL
      //--------------------------------------------------------------
      public static string
         GetSelectSQL(       
           string             serverDBName,
           string             tableName,
           string             strWhere,
           string             strOrder
         )
      {
         string tmp = "SELECT eventId,startTime,sqlText,hash" +
		                  " FROM {0}..{1} AS e" +
		                  " {2}{3}" + // where
		                  " {4};";  // order
		                     
         return string.Format( tmp,
                               SQLHelpers.CreateSafeDatabaseName(serverDBName),
                               tableName,
                               (strWhere!="") ? "WHERE " : "",
                               strWhere,
                               strOrder );
      }
      
      //-------------------------------------------------------------
      // GetInsertSQL
      //--------------------------------------------------------------
      private string
         GetInsertSQL(       
           string             serverDBName
         )
      {
      
         StringBuilder sb = new StringBuilder( "INSERT INTO ", 1024 );
         sb.Append( SQLHelpers.CreateSafeDatabaseName(serverDBName) );
         sb.Append(  ".." );
         sb.Append(  CoreConstants.RepositoryEventSqlTable );
         sb.Append(  " (eventId,startTime,sqlText,hash) VALUES (" );
         sb.Append(  eventId.ToString() );
         sb.Append(  "," );
         sb.Append(  SQLHelpers.CreateSafeDateTimeString(startTime) );
         sb.Append(  "," );
         sb.Append(  SQLHelpers.CreateSafeString(sqlText) );
         sb.Append(  "," );
         sb.Append(  hash.ToString() );
         sb.Append(  ");" );
         
         return sb.ToString();
      
      }
      
      #endregion
	}
}
