using System;
using System.Data;
using System.Data.SqlClient;

using Idera.SQLcompliance.Core;

namespace Idera.SQLcompliance.Core.Collector
{
	/// <summary>
	/// Summary description for ArchiveCopy.
	/// </summary>
	/// 
	
	internal class ArchiveCopy
	{
	   string   instance;
		string   sourceDatabase;
		string   targetDatabase;
		DateTime beginTime;
		DateTime endTime;
	   int      eventCount;
	   int      eventSqlCount;
	   int      batchSize;
		
      #region Constructors
   
		public
		   ArchiveCopy(
		      string   inInstance,
		      string   inSourceDatabase,
		      string   inTargetDatabase,
		      DateTime inBeginTime,
		      DateTime inEndTime,
		      int      inBatchSize
		   )
		{
		   instance       = inInstance;
		   sourceDatabase = inSourceDatabase;
		   targetDatabase = inTargetDatabase;
		   beginTime      = inBeginTime;
		   endTime        = inEndTime;
		   eventCount = 0;
		   eventSqlCount = 0;
		   batchSize = inBatchSize;
		}
	   
		public
		   ArchiveCopy(
		      string   inInstance,
		      string   inSourceDatabase,
		      string   inTargetDatabase,
		      DateTime inBeginTime,
		      DateTime inEndTime
		   ) 
		   : this( inInstance, inSourceDatabase, inTargetDatabase, inBeginTime, inEndTime, CoreConstants.archiveBatchSize ){}

		public
		   ArchiveCopy(
		      string   inSourceDatabase,
		      string   inTargetDatabase,
		      DateTime inBeginTime,
		      DateTime inEndTime
		   ) 
		   : this( "", inSourceDatabase, inTargetDatabase, inBeginTime, inEndTime, CoreConstants.archiveBatchSize ){}
	   
		public
		   ArchiveCopy(
		      string   inSourceDatabase,
		      string   inTargetDatabase,
		      DateTime inBeginTime,
		      DateTime inEndTime,
		      int      inBatchSize
		   ) 
		   : this( "", inSourceDatabase, inTargetDatabase, inBeginTime, inEndTime, inBatchSize ){}
      #endregion
	   
      #region Properties
	   
	   public int ArchivedEventCount
	   {
	      get { return eventCount; }
	   }
	   
	   public int ArchivedEventSqlCount
	   {
	      get { return eventSqlCount; }
	   }
	   
      #endregion
	   
      #region Public Methods
	   
	   public void Perform( bool sqlTable )
	   {
	      Repository conn = new Repository();
	      SqlTransaction trans;
	      string stmt;
	      
	      try
	      {
	         int insertedRows;
	         
	         conn.OpenConnection(sourceDatabase);
	         
	         stmt = sqlTable ? GetCopyEventSQLStatement() : GetCopyEventsStatement();
	         
            using ( SqlCommand cmd = new SqlCommand( stmt, conn.connection ) )
            {
               using( trans = conn.connection.BeginTransaction(IsolationLevel.ReadUncommitted))
               {
                  try
                  {
                     cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                     cmd.Transaction = trans;
                     insertedRows = cmd.ExecuteNonQuery();
                     
                     string deleteSql = GetDeleteBatchStatement( sourceDatabase,
                                        sqlTable ? CoreConstants.RepositoryEventSqlTable 
                                                 : CoreConstants.RepositoryEventsTable );
                     cmd.CommandText = deleteSql;
                     cmd.ExecuteNonQuery();
                     
                     trans.Commit();
                     
                     if( sqlTable )
                        eventSqlCount += insertedRows;
                     else
                        eventCount += insertedRows;
                  }
                  catch( Exception e)
                  {
                     if( trans != null )
                        trans.Rollback();
                     throw e;
                  }
               }
            }

	      }
	      catch( Exception e)
	      {
	         throw e;
	      }
	      finally
	      {
	         if( conn != null)
	            conn.CloseConnection();
	      }
	   }
	   
	   
      #endregion
		
		#region Events Table

      /*
		//------------------------------------------------------------------
		// CopyEvents
		//------------------------------------------------------------------
      public void
         CopyEvents()
      {
         DataSet ds = ReadEvents();
         
         if ( ds != null )WriteEvents( ds );
      }
      
		//------------------------------------------------------------------
		// ReadEvents
		//------------------------------------------------------------------
      private DataSet
         ReadEvents()
      {
         string     sqlText;
         DataSet    ds  = null;
         Repository rep = new Repository();
         
         // read events
         try
         {
            rep.OpenConnection(sourceDatabase);
            
            sqlText = String.Format( "SELECT TOP {0} * " +
                                       "FROM {1}.dbo.{2} " +
                                       "WHERE startTime >= {3} " +
                                       "  AND startTime <  {4} " +
                                       "ORDER by eventId",
                                     batchSize,  
                                     SQLHelpers.CreateSafeDatabaseName(sourceDatabase),
                                     CoreConstants.RepositoryEventsTable,
                                     SQLHelpers.CreateSafeDateTime(beginTime),
                                     SQLHelpers.CreateSafeDateTime(endTime) );

			 using(SqlCommand  selectCmd    = new SqlCommand( sqlText, rep.connection ))
			 {
				 selectCmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
				 selectCmd.CommandType    = CommandType.Text;
            
				 // SqlDataAdapter
				 using(SqlDataAdapter dataAdapter = new SqlDataAdapter())
				 {
					 dataAdapter.TableMappings.Add("Table", "Events");
					 dataAdapter.SelectCommand = selectCmd;
            
					 // Load DataSet  
					 ds = new DataSet("Trace");
					 dataAdapter.Fill(ds);
				 }
			 }
         }
         finally
         {
            rep.CloseConnection();
         }
         
         return ds;
      }
		
		//------------------------------------------------------------------
		// WriteEvents
		//------------------------------------------------------------------
      private void
         WriteEvents(
            DataSet  ds
         )
      {
         string sqlText = "";
         Repository rep = new Repository();
         
         // write events
         SqlTransaction writeTrans = null;

         try
         {         
            rep.OpenConnection(targetDatabase);
         
			 using(writeTrans = rep.connection.BeginTransaction( IsolationLevel.ReadUncommitted ))
			 {
				 try
				 {
         
					 foreach ( DataRow row in ds.Tables["Events"].Rows )
					 {
						 sqlText = CreateEventsInsertStmt( row );
	            
						 using ( SqlCommand cmd = new SqlCommand( sqlText, rep.connection ) )
						 {
							 cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
							 cmd.Transaction    = writeTrans;
							 cmd.ExecuteNonQuery();
						 }
					 }
            
					 if ( writeTrans != null )
					 {
						 writeTrans.Commit();
						 writeTrans = null;
					 }
				 }
				 catch(Exception e)
				 {
					 if ( writeTrans != null ) writeTrans.Rollback();
					 throw e ;
				 }
			 }
         }
         catch ( Exception ex )
         {
            ErrorLog.Instance.Write( "Archive write failure",
                                     sqlText,
                                     ex );
         }
         finally
         {
            rep.CloseConnection();
         }
      }
      
		//------------------------------------------------------------------
		// CreateEventsInsertStmt
		//------------------------------------------------------------------
		private string
		   CreateEventsInsertStmt(
		      DataRow  row
		   )
		{
		   int col=0;
		   
         string fmt = "INSERT INTO {0}..{1} " +
                      "("+		   
                         "[startTime],[checksum],[eventId],[eventType],[eventClass]," +
                         "[eventSubclass],[spid],[applicationName],[hostName],[serverName]," +
                         "[loginName],[success],[databaseName],[databaseId],[dbUserName]," +
                         "[objectType],[objectName],[objectId],[permissions],[columnPermissions]," +
                         "[targetLoginName],[targetUserName],[roleName],[ownerName],[targetObject]," +
                         "[details],[eventCategory],[hash],[alertLevel],[privilegedUser]" +
                      ") VALUES (" +
                         "{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}," +
		                   "{12},{13},{14},{15},{16},{17},{18},{19},{20},{21}," +
		                   "{22},{23},{24},{25},{26},{27},{28},{29},{30},{31}" +
		                 ")";
		                 
         string sql = String.Format( fmt,
                                     SQLHelpers.CreateSafeDatabaseName(targetDatabase),
                                     CoreConstants.RepositoryEventsTable,
		            
                                     SQLHelpers.CreateSafeDateTimeString(SQLHelpers.GetRowDateTime(row,col++)),
                                     SQLHelpers.GetRowInt32(row,col++),
                                     SQLHelpers.GetRowInt32(row,col++),
                                     SQLHelpers.GetRowInt32(row,col++),
                                     SQLHelpers.GetRowInt32(row,col++),
                                     SQLHelpers.GetRowInt32(row,col++),
                                     SQLHelpers.GetRowInt32(row,col++),
                                     SQLHelpers.CreateSafeString(SQLHelpers.GetRowString(row,col++)),
                                     SQLHelpers.CreateSafeString(SQLHelpers.GetRowString(row,col++)),
                                     SQLHelpers.CreateSafeString(SQLHelpers.GetRowString(row,col++)),
                                 	
                                     SQLHelpers.CreateSafeString(SQLHelpers.GetRowString(row,col++)),
                                     SQLHelpers.GetRowInt32(row,col++),
                                     SQLHelpers.CreateSafeString(SQLHelpers.GetRowString(row,col++)),
                                     SQLHelpers.GetRowInt32(row,col++),
                                     SQLHelpers.CreateSafeString(SQLHelpers.GetRowString(row,col++)),
                                     SQLHelpers.GetRowInt32(row,col++),
                                     SQLHelpers.CreateSafeString(SQLHelpers.GetRowString(row,col++)),
                                     SQLHelpers.GetRowInt32(row,col++),
                                     SQLHelpers.GetRowInt32(row,col++),
                                     SQLHelpers.GetRowInt32(row,col++),
                                 	
                                     SQLHelpers.CreateSafeString(SQLHelpers.GetRowString(row,col++)),
                                     SQLHelpers.CreateSafeString(SQLHelpers.GetRowString(row,col++)),
                                     SQLHelpers.CreateSafeString(SQLHelpers.GetRowString(row,col++)),
                                     SQLHelpers.CreateSafeString(SQLHelpers.GetRowString(row,col++)),
                                     SQLHelpers.CreateSafeString(SQLHelpers.GetRowString(row,col++)),
                                     SQLHelpers.CreateSafeString(SQLHelpers.GetRowString(row,col++)),
                                     SQLHelpers.GetRowInt32(row,col++),
                                     SQLHelpers.GetRowInt32(row,col++),
                                     SQLHelpers.GetRowInt32(row,col++),
                                     SQLHelpers.GetRowInt32(row,col++) );
         return sql;
      }
      */

      #endregion

      #region EventsViaReader
      /*

      //------------------------------------------------------------------
		// CopyEventsWithReader
		//------------------------------------------------------------------
      public void
         CopyEventsWithReader()
      {
         string         readSql      = "";
         string         writeSql     = "";
         Repository     readConn     = new Repository();
         Repository     writeConn    = new Repository();
         bool           stillReading = true;
         SqlTransaction writeTrans   = null;
         
         try
         {
            readConn.OpenConnection(sourceDatabase);
            writeConn.OpenConnection(targetDatabase);
               
            readSql = String.Format( "SELECT TOP {0} {5} " +
                                       "FROM {1}.dbo.{2} " +
                                       "WHERE startTime >= {3} " +
                                       "  AND startTime <  {4} " +
                                       "ORDER by eventId",
                                       batchSize,  
                                       SQLHelpers.CreateSafeDatabaseName(sourceDatabase),
                                       CoreConstants.RepositoryEventsTable,
                                       SQLHelpers.CreateSafeDateTime(beginTime),
                                       SQLHelpers.CreateSafeDateTime(endTime),
                                       ArchiveCopy.strEventColumns );
            
            while ( stillReading ) 
            {
               //-----------------------
               // Copy a batch of events
               //------------------------
               using ( SqlCommand cmd = new SqlCommand( readSql, readConn.connection ) )
               {
		            cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                  using ( SqlDataReader reader = cmd.ExecuteReader() )
                  {
                     if ( reader.HasRows )
                     {
						 using(writeTrans = writeConn.connection.BeginTransaction( IsolationLevel.ReadUncommitted ))
						 {
							 try
							 {
								 while ( reader.Read() )
								 {
									 writeSql = CreateEventsInsertStmt( reader );

									 using ( SqlCommand writeCmd = new SqlCommand( writeSql, writeConn.connection ) )
									 {
										 writeCmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
										 writeCmd.Transaction    = writeTrans;
										 writeCmd.ExecuteNonQuery();
									 }
								 }
								 writeTrans.Commit();
							 }
							 catch(Exception e)
							 {
								 if ( writeTrans!= null ) writeTrans.Rollback();
								 throw e ;
							 }
						 }
                     }
                     else
                     {
                        stillReading = false;
                     }
                  }
               }
               
               //------------------
               // Delete the Batch
               //------------------
               string deleteSql = GetDeleteBatchStatement( sourceDatabase,
                                                           CoreConstants.RepositoryEventsTable );
               using ( SqlCommand deleteCmd = new SqlCommand( deleteSql, readConn.connection ) )
               {
      	         deleteCmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
	               deleteCmd.ExecuteNonQuery();
	            }
            }
         }
         catch ( Exception ex )
         {
            ErrorLog.Instance.Write( "Archive write failure - events",
                                     writeSql,
                                     ex );
            throw ex;
         }
         finally
         {
            readConn.CloseConnection();
            writeConn.CloseConnection();
         }
      }
      
      
      */
      #endregion

      #region EventSQL Table
      /*
      
		//------------------------------------------------------------------
		// CopyEventSql
		//------------------------------------------------------------------
      public void
         CopyEventSql()
      {
         DataSet ds = ReadEventSql();
         
         if ( ds != null ) WriteEventSql( ds );
      }
      
		//------------------------------------------------------------------
		// ReadEventSql
		//------------------------------------------------------------------
      private DataSet
         ReadEventSql()
      {
         string     sqlText;
         DataSet    ds  = null;
         Repository rep = new Repository();
         
         // read events
         try
         {
            rep.OpenConnection(sourceDatabase);
            
            sqlText = String.Format( "SELECT TOP {0} * " +
                                       "FROM {1}.dbo.{2} " +
                                       "WHERE startTime >= {3} " +
                                       "  AND startTime <  {4} " +
                                       "ORDER by eventId",
                                     batchSize,  
                                     SQLHelpers.CreateSafeDatabaseName(sourceDatabase),
                                     CoreConstants.RepositoryEventSqlTable,
                                     SQLHelpers.CreateSafeDateTime(beginTime),
                                     SQLHelpers.CreateSafeDateTime(endTime) );

			 using(SqlCommand  selectCmd    = new SqlCommand( sqlText, rep.connection ))
			 {
				 selectCmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
				 selectCmd.CommandType    = CommandType.Text;
            
				 // SqlDataAdapter
				 using(SqlDataAdapter dataAdapter = new SqlDataAdapter())
				 {
					 dataAdapter.TableMappings.Add("Table", "EventSql");
					 dataAdapter.SelectCommand = selectCmd;
            
					 // Load DataSet  
					 ds = new DataSet("Trace");
					 dataAdapter.Fill(ds);
				 }
			 }
         }
         finally
         {
            rep.CloseConnection();
         }
         
         return ds;
      }
		
		//------------------------------------------------------------------
		// WriteEventSql
		//------------------------------------------------------------------
      private void
         WriteEventSql(
            DataSet  ds
         )
      {
         string sqlText = "";
         Repository rep = new Repository();
         
         // write events
         SqlTransaction writeTrans = null;

         try
         {         
            rep.OpenConnection(targetDatabase);
         
			 using(writeTrans = rep.connection.BeginTransaction( IsolationLevel.ReadUncommitted ))
			 {
				 try
				 {
					 foreach ( DataRow row in ds.Tables["EventSql"].Rows )
					 {
						 sqlText = CreateEventSqlInsertStmt( row );
	            
						 using ( SqlCommand cmd = new SqlCommand( sqlText, rep.connection ) )
						 {
							 cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
							 cmd.Transaction    = writeTrans;
							 cmd.ExecuteNonQuery();
						 }
					 }
            
					 if ( writeTrans != null )
					 {
						 writeTrans.Commit();
					 }
				 }
				 catch(Exception e)
				 {
					 if ( writeTrans != null ) writeTrans.Rollback();
					 throw e ;
				 }
			 }
         }
         catch ( Exception ex )
         {
            ErrorLog.Instance.Write( "Archive EventSql write failure",
                                     sqlText,
                                     ex );
         }
         finally
         {
            rep.CloseConnection();
         }
      }
      */

      #endregion
      
      #region EventsSqlViaReader

      /*
		//------------------------------------------------------------------
		// CopyEventSqlWithReader
		//------------------------------------------------------------------
      public void
         CopyEventSqlWithReader()
      {
         string         readSql      = "";
         string         writeSql     = "";
         Repository     readConn     = new Repository();
         Repository     writeConn    = new Repository();
         bool           stillReading = true;
         SqlTransaction writeTrans   = null;
         
         try
         {
            readConn.OpenConnection(sourceDatabase);
            writeConn.OpenConnection(targetDatabase);
               
            readSql = String.Format( "SELECT TOP {0} {5} " +
                                       "FROM {1}.dbo.{2} " +
                                       "WHERE startTime >= {3} " +
                                       "  AND startTime <  {4} " +
                                       "ORDER by eventId",
                                       batchSize,  
                                       SQLHelpers.CreateSafeDatabaseName(sourceDatabase),
                                       CoreConstants.RepositoryEventSqlTable,
                                       SQLHelpers.CreateSafeDateTime(beginTime),
                                       SQLHelpers.CreateSafeDateTime(endTime),
                                       ArchiveCopy.strEventSqlColumns );
            
            while ( stillReading ) 
            {
               //-----------------------
               // Copy a batch of events
               //------------------------
               using ( SqlCommand cmd = new SqlCommand( readSql, readConn.connection ) )
               {
		            cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                  using ( SqlDataReader reader = cmd.ExecuteReader() )
                  {
                     if ( reader.HasRows )
                     {
						 using(writeTrans = writeConn.connection.BeginTransaction( IsolationLevel.ReadUncommitted ))
						 {
							 try
							 {
								 while ( reader.Read() )
								 {
									 writeSql = CreateEventSqlInsertStmt( reader );

									 using ( SqlCommand writeCmd = new SqlCommand( writeSql, writeConn.connection ) )
									 {
										 writeCmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
										 writeCmd.Transaction    = writeTrans;
										 writeCmd.ExecuteNonQuery();
									 }
								 }
								 writeTrans.Commit();

							 }
							 catch(Exception e)
							 {
								 if ( writeTrans!= null ) writeTrans.Rollback();
								 throw e; 
							 }
						 }
                     }
                     else
                     {
                        stillReading = false;
                     }
                  }
               }
               
               //------------------
               // Delete the Batch
               //------------------
               string deleteSql = GetDeleteBatchStatement( sourceDatabase,
                                                           CoreConstants.RepositoryEventSqlTable );
               using ( SqlCommand deleteCmd = new SqlCommand( deleteSql, readConn.connection ) )
               {
      	         deleteCmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
	               deleteCmd.ExecuteNonQuery();
	            }
            }
         }
         catch ( Exception ex )
         {
            
            ErrorLog.Instance.Write( "Archive write failure - events",
                                     writeSql,
                                     ex );
            throw ex;
         }
         finally
         {
            readConn.CloseConnection();
            writeConn.CloseConnection();
         }
      }
      
      */
      
      #endregion
      
      #region ChangeLog Table
      
		//------------------------------------------------------------------
		// CopyChangeLog
		//------------------------------------------------------------------
      public void
         CopyChangeLog()
      {
         DataSet ds = ReadChangeLog();
         
         if ( ds != null ) WriteChangeLog( ds );
      }
      
		//------------------------------------------------------------------
		// ReadChangeLog
		//------------------------------------------------------------------
      private DataSet
         ReadChangeLog()
      {
         string     sqlText;
         DataSet    ds;
         Repository rep = new Repository();
         
         // read events
         try
         {
            rep.OpenConnection(sourceDatabase);
            
            sqlText = String.Format( "SELECT TOP {0} * " +
                                       "FROM {1}.dbo.{2} " +
                                       "WHERE eventTime >= {3} " +
                                          "AND eventTime < {4} " +
                                          "AND logSqlServer = {5} " +
                                       "ORDER by logId",
                                     batchSize,  
                                     SQLHelpers.CreateSafeDatabaseName(sourceDatabase),
                                     CoreConstants.RepositoryChangeLogEventTable,
                                     SQLHelpers.CreateSafeDateTime(beginTime),
                                     SQLHelpers.CreateSafeDateTime(endTime),
                                     SQLHelpers.CreateSafeString( instance ));

			 using(SqlCommand  selectCmd    = new SqlCommand( sqlText, rep.connection ))
			 {
				 selectCmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
				 selectCmd.CommandType    = CommandType.Text;
            
				 // SqlDataAdapter
				 using(SqlDataAdapter dataAdapter = new SqlDataAdapter())
				 {
					 dataAdapter.TableMappings.Add("Table", "Events");
					 dataAdapter.SelectCommand = selectCmd;
            
					 // Load DataSet  
					 ds = new DataSet("ChangeLog");
					 dataAdapter.Fill(ds);
				 }
			 }
         }
         finally
         {
            rep.CloseConnection();
         }
         
         return ds;
      }
		
		//------------------------------------------------------------------
		// WriteChangeLog
		//------------------------------------------------------------------
      private void
         WriteChangeLog(
            DataSet  ds
         )
      {
         string sqlText = "";
         Repository rep = new Repository();
         
         // write events
         SqlTransaction writeTrans;

         try
         {         
            rep.OpenConnection(targetDatabase);
         
            using(writeTrans = rep.connection.BeginTransaction( IsolationLevel.ReadUncommitted ))
            {
               try
               {
                  foreach ( DataRow row in ds.Tables["Events"].Rows )
                  {
                     sqlText = CreateChangeLogInsertStmt( row );

                     using ( SqlCommand cmd = new SqlCommand( sqlText, rep.connection ) )
                     {
                        cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                        cmd.Transaction    = writeTrans;
                        cmd.ExecuteNonQuery();
                     }
                  }

                  if ( writeTrans != null )
                  {
                     writeTrans.Commit();
                  }
               }
               catch(Exception e)
               {
                  if ( writeTrans != null ) writeTrans.Rollback();
                  throw e ;
               }
            }
         }
         catch ( Exception ex )
         {
            ErrorLog.Instance.Write( "Archive ChangeLog write failure",
                                     sqlText,
                                     ex );
                                     
            throw ex;
         }
         finally
         {
            rep.CloseConnection();
         }
      }
      
		//------------------------------------------------------------------
		// CreateChangeLogInsertStmt
		//------------------------------------------------------------------
		private string
		   CreateChangeLogInsertStmt(
		      DataRow  row
		   )
		{
		   int col=0;
		   
         string fmt = "INSERT INTO {0}..{1} " +
                      "([logId], [eventTime], [logType], [logUser], [logSqlServer], [logInfo])" +
                      "VALUES ( {2},{3},{4},{5},{6},{7} )";
		                 
         string sql = String.Format( fmt,
                                     SQLHelpers.CreateSafeDatabaseName(targetDatabase),
                                     CoreConstants.RepositoryChangeLogEventTable,
                                     SQLHelpers.GetRowInt32(row,col++),
                                     SQLHelpers.CreateSafeDateTimeString(SQLHelpers.GetRowDateTime(row,col++)),
                                     SQLHelpers.GetRowInt32(row,col++),
                                     SQLHelpers.CreateSafeString(SQLHelpers.GetRowString(row,col++)),
                                     SQLHelpers.CreateSafeString(SQLHelpers.GetRowString(row,col++)),
                                     SQLHelpers.CreateSafeString(SQLHelpers.GetRowString(row,col)) );
         return sql;
      }                        
      
      #endregion
      
      #region Stats Table
      
      #endregion
	   
      #region SQL

      static private string strEventColumns =
         "[startTime],[checksum],[eventId],[eventType],[eventClass]," +
         "[eventSubclass],[spid],[applicationName],[hostName],[serverName]," +
         "[loginName],[success],[databaseName],[databaseId],[dbUserName]," +
         "[objectType],[objectName],[objectId],[permissions],[columnPermissions]," +
         "[targetLoginName],[targetUserName],[roleName],[ownerName],[targetObject]," +
         "[details],[eventCategory],[hash],[alertLevel],[privilegedUser], " +
         "[fileName], [linkedServerName], [parentName], [isSystem], " +
         "[sessionLoginName], [providerName], [appNameId], [hostId], [loginId], " +
         "[endTime], [startSequence], [endSequence], [rowCounts] ";
	
	   static private string strEventSqlColumns =
         "[eventId],[startTime],[sqlText],[hash]";

/*
	   //------------------------------------------------------------------
		// CreateEventsInsertStmt
		//------------------------------------------------------------------
		private string
		   CreateEventsInsertStmt(
		      SqlDataReader  reader
		   )
		{
		   int col=0;
		   
         string fmt = "INSERT INTO {0}..{1} " +
                      "({32}) VALUES (" +
                         "{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}," +
		                   "{12},{13},{14},{15},{16},{17},{18},{19},{20},{21}," +
		                   "{22},{23},{24},{25},{26},{27},{28},{29},{30},{31}" +
		                 ")";
		                 
         string sql = String.Format( fmt,
                                     SQLHelpers.CreateSafeDatabaseName(targetDatabase),
                                     CoreConstants.RepositoryEventsTable,
		            
                                     SQLHelpers.CreateSafeDateTimeString(SQLHelpers.GetDateTime(reader,col++)),
                                     SQLHelpers.GetInt32(reader,col++),
                                     SQLHelpers.GetInt32(reader,col++),
                                     SQLHelpers.GetInt32(reader,col++),
                                     SQLHelpers.GetInt32(reader,col++),
                                     SQLHelpers.GetInt32(reader,col++),
                                     SQLHelpers.GetInt32(reader,col++),
                                     SQLHelpers.CreateSafeString(SQLHelpers.GetString(reader,col++)),
                                     SQLHelpers.CreateSafeString(SQLHelpers.GetString(reader,col++)),
                                     SQLHelpers.CreateSafeString(SQLHelpers.GetString(reader,col++)),
                                 	
                                     SQLHelpers.CreateSafeString(SQLHelpers.GetString(reader,col++)),
                                     SQLHelpers.GetInt32(reader,col++),
                                     SQLHelpers.CreateSafeString(SQLHelpers.GetString(reader,col++)),
                                     SQLHelpers.GetInt32(reader,col++),
                                     SQLHelpers.CreateSafeString(SQLHelpers.GetString(reader,col++)),
                                     SQLHelpers.GetInt32(reader,col++),
                                     SQLHelpers.CreateSafeString(SQLHelpers.GetString(reader,col++)),
                                     SQLHelpers.GetInt32(reader,col++),
                                     SQLHelpers.GetInt32(reader,col++),
                                     SQLHelpers.GetInt32(reader,col++),
                                 	
                                     SQLHelpers.CreateSafeString(SQLHelpers.GetString(reader,col++)),
                                     SQLHelpers.CreateSafeString(SQLHelpers.GetString(reader,col++)),
                                     SQLHelpers.CreateSafeString(SQLHelpers.GetString(reader,col++)),
                                     SQLHelpers.CreateSafeString(SQLHelpers.GetString(reader,col++)),
                                     SQLHelpers.CreateSafeString(SQLHelpers.GetString(reader,col++)),
                                     SQLHelpers.CreateSafeString(SQLHelpers.GetString(reader,col++)),
                                     SQLHelpers.GetInt32(reader,col++),
                                     SQLHelpers.GetInt32(reader,col++),
                                     SQLHelpers.GetInt32(reader,col++),
                                     SQLHelpers.GetInt32(reader,col++),
                                     
                                     ArchiveCopy.strEventColumns );
         return sql;
      }
*/
	   
      //---------------------------------------------------------------
      // GetDeleteBatchStatement
      //---------------------------------------------------------------
      private string
         GetDeleteBatchStatement(
            string databaseName,
            string tableName )
      {
         return String.Format( "DELETE {0}.dbo.{1} " + 
                               "FROM ( SELECT TOP {2} eventId " +
                                      "FROM {0}.dbo.{1} " +
                                      "WHERE startTime >= {3} " +
                                        "AND startTime <  {4} " +
                                      "ORDER by eventId ) AS e " +
                               "WHERE e.eventId={0}.dbo.{1}.eventId",
                               SQLHelpers.CreateSafeDatabaseName( databaseName ),
                               tableName,
                               batchSize,
                               SQLHelpers.CreateSafeDateTime(beginTime),
                               SQLHelpers.CreateSafeDateTime(endTime) );
      }
      

	   private string GetCopyEventsStatement()
	   {
	      	string fmt = "INSERT INTO {0}.dbo.{1} ({2}) " +
	                      "SELECT TOP {3} {2} " +
	                      "FROM {4}.dbo.{1} " +
	                      "WHERE startTime >= {5} AND startTime <  {6} " +
	                      "ORDER by eventId";
	         
	         return String.Format( fmt,
                                  SQLHelpers.CreateSafeDatabaseName(targetDatabase),
                                  CoreConstants.RepositoryEventsTable,
                                  strEventColumns,
                                  batchSize,  
                                  SQLHelpers.CreateSafeDatabaseName(sourceDatabase),
                                  SQLHelpers.CreateSafeDateTime(beginTime),
                                  SQLHelpers.CreateSafeDateTime(endTime) );
	         

	   }
	   
	   private string GetCopyEventSQLStatement()
	   {
	         return String.Format( "INSERT INTO {0}.dbo.{1} ( {2} ) " +
	                               "SELECT TOP {3} {2} " +
                                  "FROM {4}.dbo.{1} " +
                                  "WHERE startTime >= {5} " +
                                  "  AND startTime <  {6} " +
                                  "ORDER by eventId",
                                  SQLHelpers.CreateSafeDatabaseName(targetDatabase),
                                  CoreConstants.RepositoryEventSqlTable,
	                               strEventSqlColumns,
	                               batchSize,  
                                  SQLHelpers.CreateSafeDatabaseName(sourceDatabase),
                                  SQLHelpers.CreateSafeDateTime(beginTime),
                                  SQLHelpers.CreateSafeDateTime(endTime) );

	   }

/*
		//------------------------------------------------------------------
		// CreateEventSqlInsertStmt
		//------------------------------------------------------------------
		private string
		   CreateEventSqlInsertStmt(
		      DataRow  row
		   )
		{
		   int col=0;
		   
         string fmt = "INSERT INTO {0}..{1} " +
                      "([eventId],[startTime],[sqlText],[hash])" +
                      "VALUES ( {2},{3},{4},{5} )";
		                 
         string sql = String.Format( fmt,
                                     SQLHelpers.CreateSafeDatabaseName(targetDatabase),
                                     CoreConstants.RepositoryEventSqlTable,
                                     SQLHelpers.GetRowInt32(row,col++),
                                     SQLHelpers.CreateSafeDateTimeString(SQLHelpers.GetRowDateTime(row,col++)),
                                     SQLHelpers.CreateSafeString(SQLHelpers.GetRowString(row,col++)),
                                     SQLHelpers.GetRowInt32(row,col++) );
         return sql;
      }                        
      
		//------------------------------------------------------------------
		// CreateEventsInsertStmt
		//------------------------------------------------------------------
		private string
		   CreateEventSqlInsertStmt(
		      SqlDataReader  reader
		   )
		{
		   int col=0;
		   
         string fmt = "INSERT INTO {0}..{1} " +
                      "({6}) VALUES (" +
                         "{2},{3},{4},{5}" +
		                 ")";
		                 
         string sql = String.Format( fmt,
                                     SQLHelpers.CreateSafeDatabaseName(targetDatabase),
                                     CoreConstants.RepositoryEventSqlTable,
		            
                                     SQLHelpers.GetInt32(reader,col++),
                                     SQLHelpers.CreateSafeDateTimeString(SQLHelpers.GetDateTime(reader,col++)),
                                     SQLHelpers.CreateSafeString(SQLHelpers.GetString(reader,col++)),
                                     SQLHelpers.GetInt32(reader,col++),
                                     
                                     ArchiveCopy.strEventSqlColumns );
         return sql;
      }
*/      

      #endregion
	}
}
