using System;
using System.Collections;
using System.Data.SqlClient;


namespace Idera.SQLcompliance.Core.Event
{
	/// <summary>
	/// Summary description for EventId.
	/// </summary>
	internal class EventId
	{
      static Hashtable instanceLocks = new Hashtable();
      static Hashtable instanceStatus = new Hashtable();

      class EventIdStatus
      {
         internal bool insertingBlock   = false;
         internal int  oldHighWatermark = -2100000000;
      }

		private EventId() {}
		
      //-----------------------------------------------------------------------
      // GetNextId
      //-----------------------------------------------------------------------
		static internal int
		   GetNextId(
		      string   instance
		   )
		{
		   return GetNextIdBlock( instance, 1 );
		}
		
		
      //-----------------------------------------------------------------------
      // GetNextIdBlock
      //-----------------------------------------------------------------------
		static internal int
		   GetNextIdBlock(
		      string   instance,
		      int      sizeOfBlock
		   )
		{
		   lock ( AcquireInstanceLock( instance ) )
		   {
            ErrorLog.Instance.Write(ErrorLog.Level.Debug, String.Format("TraceJob: Acquired ID lock for {0}", instance));
            int highWatermark = ReadHighWatermark(instance);
            EventIdStatus status = (EventIdStatus)instanceStatus[instance.ToUpper()];
            status.insertingBlock = true;
            status.oldHighWatermark = highWatermark;
		      
		      return GetNextIdBlock( instance,
                                   highWatermark,
                                   sizeOfBlock );
		   }
		}

      //-----------------------------------------------------------------------
      // GetNextIdBlock
      //-----------------------------------------------------------------------
      static internal int
         GetNextIdBlock(
            string instance,
            int    currentHighWatermark,
            int    sizeOfBlock
         )
      {
		   int nextId = currentHighWatermark + 1;
		   
		   currentHighWatermark += sizeOfBlock;
		   WriteHighWatermark( instance, currentHighWatermark);
		   ErrorLog.Instance.Write( ErrorLog.Level.Debug, String.Format( "TraceJob: Acquired ID block for {0}: next ID={1}, block size = {2}.", 
		                                                                   instance, nextId, sizeOfBlock ));
		   
		   return nextId;
      }

      //-----------------------------------------------------------------------
      // UpdateStatus
      //-----------------------------------------------------------------------
      static internal void
         UpdateStatus(
            string instance,
            int    highWatermark
         )
      {
		   lock ( AcquireInstanceLock( instance ) )
		   {
            EventIdStatus status = (EventIdStatus)instanceStatus[instance.ToUpper()];
            status.oldHighWatermark = highWatermark;
            status.insertingBlock = false;
		   }
      }

		
      //-----------------------------------------------------------------------
      // AcquireInstanceLock
      //-----------------------------------------------------------------------
      static internal Object
         AcquireInstanceLock(
            string      instance
         )
      {
         string upperInstance = instance.ToUpper();
         
         if ( instanceLocks.Contains( upperInstance ) )
         {
            return instanceLocks[upperInstance];
         }
         else
         {
            Object        syncObj = new Object();
            EventIdStatus status  = new EventIdStatus();
            status.oldHighWatermark = -2100000000;

            instanceLocks.Add( upperInstance, syncObj );
            instanceStatus.Add( upperInstance, status );

            return syncObj;
         }
      }


      //------------------------------------------------------------------
      // GetHighWatermark
      //------------------------------------------------------------------
      static internal int 
         GetHighWatermark(
            string instance
         )
      {
         int highWatermark = -2100000000;
         string upperInstance = instance.ToUpper();

         lock( AcquireInstanceLock( instance ) )
         {
            EventIdStatus status = (EventIdStatus)instanceStatus[upperInstance];
            
            int currentHighWatermark = ReadHighWatermark( instance );
            
            if ( currentHighWatermark < status.oldHighWatermark )
            {
               // event database got cleared - archiving or server drop/readd
               highWatermark = currentHighWatermark;
            }
            else if ( status.oldHighWatermark == -2100000000 
                      &&  !status.insertingBlock )
            {
               highWatermark = currentHighWatermark;
            }
            else
            {
               highWatermark = status.oldHighWatermark;
            }
         }

         return highWatermark;
      }

      //------------------------------------------------------------------
      // ReadHighWatermark
      //------------------------------------------------------------------
      static internal int
         ReadHighWatermark(
            string      instance
         )
      {
         int highWatermark = -2100000000;
         
         Repository rep = new Repository();
              
         string cmdStr =  String.Format( "SELECT highWatermark FROM {0}..{1} WHERE instance = {2}",
                                         CoreConstants.RepositoryDatabase,
                                         CoreConstants.RepositoryServerTable,
                                         SQLHelpers.CreateSafeString(instance) );

         try
         {
            rep.OpenConnection();
            
			   using(SqlCommand command = new SqlCommand( cmdStr, rep.connection ))
			   {
				   command.CommandTimeout = CoreConstants.sqlcommandTimeout;
				   object obj = command.ExecuteScalar();
   			   if( obj is System.DBNull )
   			      highWatermark = -2100000000;
   			   else
   			      highWatermark = (int)obj;
			   }
         }
         catch( Exception e )
         {
            if ( ErrorLog.Instance.ErrorLevel >= ErrorLog.Level.Debug )
            {
               ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                                        String.Format( "An error occurred retrieving high watermarks for {0}\nSQL: {1}",
                                                      instance,
                                                      cmdStr ),
                                        e,
                                        true );
            }
            throw e;                                          
         }
         finally
         {
            rep.CloseConnection();
         }

         return highWatermark;
      }

      //------------------------------------------------------------------
      // WriteHighWatermark
      //------------------------------------------------------------------
      static internal void
         WriteHighWatermark(
            string         instance,
            int            highWatermark
         )
      {
         Repository rep = new Repository();
         
         string cmdStr = String.Format( "UPDATE {0}..{1} SET highWatermark = {2} WHERE instance = {3}",
                                        CoreConstants.RepositoryDatabase,
                                        CoreConstants.RepositoryServerTable,
                                        highWatermark,
                                        SQLHelpers.CreateSafeString(instance) );
         try
         {
            rep.OpenConnection();

			   using(SqlCommand command = new SqlCommand( cmdStr, rep.connection ))
			   {
				   command.CommandTimeout = CoreConstants.sqlcommandTimeout;
				   command.ExecuteNonQuery();
			   }
         }
         catch( Exception e )
         {
            if ( ErrorLog.Instance.ErrorLevel >= ErrorLog.Level.Debug )
            {
               ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                                        String.Format( "An error occurred updating watermarks for {0}\nSQL: {1}",
                                                       instance,
                                                       cmdStr ),
                                        e,
                                        true );
               throw e;
            }
         }
         finally
         {
            rep.CloseConnection();
         }
      }

      //------------------------------------------------------------------
      // UpdateWatermarks
      //------------------------------------------------------------------
      static internal void
         UpdateWatermarks(
            string instance,
            int highWatermark,
            int lowWatermark,
            int alertHighWatermark
         )
      {
          Repository rep = new Repository();

          string cmdStr = String.Format("UPDATE {0}..{1} SET highWatermark = {2} ,lowWatermark = {3},alertHighWatermark = {4}  WHERE instance = {5}",
                                         CoreConstants.RepositoryDatabase,
                                         CoreConstants.RepositoryServerTable,
                                         highWatermark,
                                         lowWatermark,
                                         alertHighWatermark,
                                         SQLHelpers.CreateSafeString(instance));
          try
          {
              rep.OpenConnection();

              using (SqlCommand command = new SqlCommand(cmdStr, rep.connection))
              {
                  command.CommandTimeout = CoreConstants.sqlcommandTimeout;
                  command.ExecuteNonQuery();
              }
          }
          catch (Exception e)
          {
              if (ErrorLog.Instance.ErrorLevel >= ErrorLog.Level.Debug)
              {
                  ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                           String.Format("An error occurred updating watermarks for {0}\nSQL: {1}",
                                                          instance,
                                                          cmdStr),
                                           e,
                                           true);
                  throw e;
              }
          }
          finally
          {
              rep.CloseConnection();
          }
      }
      
      //------------------------------------------------------------------
      // IsInsertingBlock - return the status of the instance
      //------------------------------------------------------------------
      static internal bool
         IsInsertingBlock(
            string instance
         )
      {
            return ((EventIdStatus)instanceStatus[instance.ToUpper()]).insertingBlock;
      }
      //------------------------------------------------------------------
      // ResetWatermarks - used at end of hash chain to set watermarks
      //
      // Special cases: no events left
      //------------------------------------------------------------------
      static internal void
         ResetWatermarks(
            string         instance,
            string         database
         )
      {
         string sql = "";
         long eventCount = 0;
         int minEvent   = -2100000000;
         int maxEvent   = -2100000000;         
         
         Repository rep = new Repository();
         
		   lock ( AcquireInstanceLock( instance ) )
		   {
            try
            {
               rep.OpenConnection();

               sql = String.Format( "DECLARE @eventCount bigint\n" +
                                    "DECLARE @minEventId int\n" +
                                    "SET @eventCount = (SELECT count(eventId) FROM {0}..{1})\n" +
                                    "SET @minEventId = (SELECT min(eventId) FROM  {0}..{1} where eventId >= \n" +
                                    "					(SELECT lowWatermark from {2}..{3} \n" +
                                    "					 where instance = {4}))\n" +
                                    "if(@minEventId IS NULL AND @eventCount > 0)\n" +
                                    "	SET @minEventId = (SELECT min(eventId) FROM  {0}..{1})\n" +
                                    "SELECT @eventCount,@minEventId",
                                    SQLHelpers.CreateSafeDatabaseName(database),
                                    CoreConstants.RepositoryEventsTable,
                                    CoreConstants.RepositoryDatabase,
                                    CoreConstants.RepositoryServerTable,
                                    SQLHelpers.CreateSafeString(instance)
                                    );

			      using(SqlCommand command = new SqlCommand( sql, rep.connection ))
			      {
				      command.CommandTimeout = CoreConstants.sqlcommandTimeout;
				      using ( SqlDataReader reader = command.ExecuteReader() )
				      {
				         if ( ! reader.Read() ) return;
   				      
                     if( ! reader.IsDBNull(0) ) eventCount = reader.GetInt64(0);
                     if( ! reader.IsDBNull(1) ) minEvent   = reader.GetInt32(1); 
                     // Low watermark is one less than the lowest event ID
                     if( eventCount > 0 )
                        minEvent -= 1;
				      }
				   }
				   
				   // read highwatermark
               sql = String.Format( "SELECT highWatermark FROM {0}..{1} WHERE instance={2}",
                                    CoreConstants.RepositoryDatabase,
                                    CoreConstants.RepositoryServerTable,
                                    SQLHelpers.CreateSafeString(instance) );

			      using(SqlCommand command = new SqlCommand( sql, rep.connection ))
			      {
				      command.CommandTimeout = CoreConstants.sqlcommandTimeout;
				      object obj = command.ExecuteScalar();
                  if( obj is System.DBNull )
                  {
                     maxEvent = -2100000000;
                     minEvent = -2100000000;
                  }
                  else
                  {
                     maxEvent = (int)obj;
                     if ( eventCount == 0 )
                     {
                        // When the table is empty, low watermark equals high watermark
                        minEvent = maxEvent;
                     }
                  }
               }
   				   
				   sql = String.Format( "UPDATE {0}..{1} SET lowWatermark={2},highWatermark={3} WHERE instance = {4}",
                                    CoreConstants.RepositoryDatabase,
                                    CoreConstants.RepositoryServerTable,
                                    minEvent,
                                    maxEvent,
                                    SQLHelpers.CreateSafeString(instance) );

			      using(SqlCommand command = new SqlCommand( sql, rep.connection ))
			      {
				      command.CommandTimeout = CoreConstants.sqlcommandTimeout;
				      command.ExecuteNonQuery();
				   }
               EventIdStatus status = (EventIdStatus)instanceStatus[instance.ToUpper()];

               if( !status.insertingBlock )
                  status.oldHighWatermark = maxEvent;

            }
            catch( Exception e )
            {
               ErrorLog.Instance.Write( "An error occurred updating watermarks.",
                                       e,
                                       ErrorLog.Severity.Warning );
               throw e;                                       
            }
            finally
            {
               rep.CloseConnection();
            }
         }
      }

      //------------------------------------------------------------------
      // ReadMaxEventId - To read the Max(eventId) for the instance specific DB
      //
      //------------------------------------------------------------------
      static internal int
         ReadMaxEventId(
            string instance,
            string database,
            int highWatermark
         )
      {
         string sql = "";
         int maxEvent   = -2100000000;
         Repository rep = new Repository();

         lock (AcquireInstanceLock(instance))
         {
             try
             {
                 rep.OpenConnection();
                 string query = "DECLARE @MaxEventID int\n" +
                                "SET @MaxEventID = (SELECT max(eventId)\n" +
                                "    FROM  {0}..{1})\n" +
                                "IF(@MaxEventID = 2147483647)\n" +
                                "SELECT max(eventId)\n" +
                                "    FROM  {0}..{1}\n" +
                                "        WHERE\n" +
                                "        eventId <= {2}\n" +
                                "ELSE\n" +
                                "SELECT @MaxEventID";

                 sql = String.Format(query,
                                      SQLHelpers.CreateSafeDatabaseName(database),
                                      CoreConstants.RepositoryEventsTable,
                                      highWatermark);               

                 using (SqlCommand command = new SqlCommand(sql, rep.connection))
                 {
                     command.CommandTimeout = CoreConstants.sqlcommandTimeout;
                     using (SqlDataReader reader = command.ExecuteReader())
                     {
                         if (reader.Read())
                         {
                             if (!reader.IsDBNull(0))
                                 maxEvent = reader.GetInt32(0);
                             else //Events are not available in event database
                             {
                                 maxEvent = highWatermark;
                             }
                         }
                     }
                     return maxEvent;
                 }
             }
             catch (Exception e)
             {
                 ErrorLog.Instance.Write("An error occurred reading max(eventId).",
                                         e,
                                         ErrorLog.Severity.Warning);
                 throw e;
             }
             finally
             {
                 rep.CloseConnection();
             }
         }
      }

      //------------------------------------------------------------------
      // SyncWatermarks - highWatermark,lowWatermark and alertHighWatermark are in sync if not sync it
      //------------------------------------------------------------------
      static internal bool
         SyncWatermarks(
            string instance,
            string database
         )
      {
          string sql = "";
          int maxEventId = -2100000000;
          int minEventId = -2100000000;
          int highWatermark = -2100000000;
          int alertHighWatermark = -2100000000;
          int lowWatermark = -2100000000;
          int latestEventId = -2100000000;
          bool isEventDatabaseEmpty = false;
          bool isUpdateRequired = false;
          Repository rep = new Repository();

          lock (AcquireInstanceLock(instance))
          {
              try
              {
                  rep.OpenConnection();
                  string query = "DECLARE @MinEventId int\n" +
                                "DECLARE @MaxEventId int\n" +
                                "DECLARE @LatestEventId int\n" +
                                "SELECT @MinEventId = MIN(eventId), @MaxEventId = MAX(eventId) FROM {0}..{1}\n" +
                                "SET @LatestEventId = @MaxEventId \n"+
                                "IF(@LatestEventId = 2147483647)\n" +
                                "SELECT @LatestEventId = MAX(eventId) FROM {0}..{1} INNER JOIN " +
                                "{2}..{3} ON (instance = '{4}' AND eventId <= highWatermark)\n" +
                                "SELECT highWatermark,lowWatermark,alertHighWatermark,@MaxEventId as maxEventId,@LatestEventId as latestEventId,@MinEventId as minEventId " +
                                "from {2}..{3} WHERE instance = '{4}'";

                  sql = String.Format(query,
                                       SQLHelpers.CreateSafeDatabaseName(database),
                                       CoreConstants.RepositoryEventsTable,
                                       CoreConstants.RepositoryDatabase,
                                       CoreConstants.RepositoryServerTable,
                                       instance);

                  using (SqlCommand command = new SqlCommand(sql, rep.connection))
                  {
                      command.CommandTimeout = CoreConstants.sqlcommandTimeout;
                      using (SqlDataReader reader = command.ExecuteReader())
                      {
                          if (reader.Read())
                          {
                              if (!reader.IsDBNull(0))
                                  highWatermark = reader.GetInt32(0);
                              if (!reader.IsDBNull(1))
                                  lowWatermark = reader.GetInt32(1);
                              if (!reader.IsDBNull(2))
                                  alertHighWatermark = reader.GetInt32(2);
                              if (!reader.IsDBNull(3))
                                  maxEventId = reader.GetInt32(3);
                              if (!reader.IsDBNull(4))
                                  latestEventId = reader.GetInt32(4);
                              if (!reader.IsDBNull(5))
                                  minEventId = reader.GetInt32(5);
                              else
                                  isEventDatabaseEmpty = true;
                          }
                      }
                  }

                  if (isEventDatabaseEmpty)
                  {
                      //if events are not available in Events table then update watermark with current HighWatermark
                      alertHighWatermark = lowWatermark = highWatermark;
                      isUpdateRequired = true;
                  }
                  else
                  {
                      if (highWatermark != latestEventId)
                      {
                          highWatermark = latestEventId;
                          isUpdateRequired = true;
                          ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                         String.Format("WriteEvents -> Syncing HighWatermark with Max(eventID):" +
                                                       " Current HighWatermark: {0}, Current Max EventID: {1}",
                                                         highWatermark,
                                                         latestEventId));
                      }
                      if (alertHighWatermark > latestEventId)
                      {
                          alertHighWatermark = latestEventId;
                          isUpdateRequired = true;
                          ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                         String.Format("WriteEvents -> Syncing AlertHighWatermark with Max(eventID):" +
                                                       " Current AlertHighWatermark: {0}, Current Max EventID: {1}",
                                                         alertHighWatermark,
                                                         latestEventId));
                      }
                      if (maxEventId != int.MaxValue && lowWatermark != (minEventId - 1) && (minEventId-1) == -2100000000)
                      {
                          lowWatermark = minEventId - 1;
                          isUpdateRequired = true;
                          ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                         String.Format("WriteEvents -> Syncing LowWatermark with min(eventID):" +
                                                       " Current LowWatermark: {0}, Current Min EventID: {1}",
                                                         lowWatermark,
                                                         minEventId));
                      }
                  }

                  if (isUpdateRequired)
                  {
                      UpdateWatermarks(instance, highWatermark, lowWatermark, alertHighWatermark);
                      return true;
                  }
              }
              catch (Exception e)
              {
                  ErrorLog.Instance.Write("An error occurred to sync watermarks",
                                          e,
                                          ErrorLog.Severity.Warning);
                  throw e;
              }
              finally
              {
                  rep.CloseConnection();
              }
          }
          return false;
      }
	}
}



