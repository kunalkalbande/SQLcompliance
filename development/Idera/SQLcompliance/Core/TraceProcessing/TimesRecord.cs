using System;
using System.Data.SqlClient;

namespace Idera.SQLcompliance.Core.TraceProcessing
{
	/// <summary>
	/// Summary description for TimesRecord.
	/// </summary>
	internal class TimesRecord
	{
	   #region Properties
	   private static Object timesRecordLock = new Object();
	  
	   private string     m_instance      = "";
	   private int        m_traceType    = 0;
	   private DateTime   m_startTime     = DateTime.MinValue;
	   private DateTime   m_endTime       = DateTime.MinValue;
	   private DateTime   m_updated       = DateTime.MinValue;

	   public string     Instance
	   {
	      get { return m_instance; }
	      set{ m_instance = value; }
	   } 
	  
	   public int        TraceType
	   {
	      get { return m_traceType; }
	      set{ m_traceType = value; }
	   }

	   public DateTime   StartTime
	   {
	      get { return m_startTime; }
	      set{ m_startTime = value; }
	   }

	   public DateTime   EndTime
	   {
	      get { return m_endTime; }
	      set{ m_endTime = value; }
	   }

	   public DateTime   Updated
	   {
	      get { return m_updated; }
	   }

      
	   #endregion
	
	   #region Constructor

      //----------------------------------------------------------------------		
      // Constructor
      //----------------------------------------------------------------------		
	   public
	      TimesRecord(
	         string            instance,
	         int               traceType
	      )
	   {
	      m_instance  = instance;
	      m_traceType = traceType;
      }
		
		#endregion
		
		#region Methods

      //----------------------------------------------------------------------		
      // Read
      //----------------------------------------------------------------------		
      static internal TimesRecord
         Read(
            string instance,
            int    traceType
         )
      {
	      Repository           rep = new Repository();
         string               sqlText = "";
         bool                 recordFound  = false;
         TimesRecord          tr = new TimesRecord( instance, traceType );
         
         lock (timesRecordLock)
         {
            try
            {
               rep.OpenConnection( CoreConstants.RepositoryTempDatabase );
               
               sqlText = String.Format( "SELECT startTime,endTime,updated " +
                                          "FROM {0} " +
                                          "WHERE instance={1} and traceType={2}",
                                       CoreConstants.RepositoryTemp_TimesTable,
                                       SQLHelpers.CreateSafeString(instance),
                                       traceType );
               using ( SqlCommand cmd = new SqlCommand( sqlText, rep.connection ) )
               {
			         cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                  using ( SqlDataReader reader = cmd.ExecuteReader() )
                  {
                     if ( reader.Read() )
                     {
                        tr.m_startTime = SQLHelpers.GetDateTime( reader, 0 );
                        tr.m_endTime   = SQLHelpers.GetDateTime( reader, 1 );
                        tr.m_updated   = SQLHelpers.GetDateTime( reader, 2 );
                        
                        recordFound = true;
                     }
                  }
               }
            }
            catch (Exception ex)
            {
               ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                                        "Read Time Table",
                                        sqlText,
                                        ex,
                                        ErrorLog.Severity.Informational );
               throw ex;
            }
            finally
            {
               rep.CloseConnection();
               
               if ( ! recordFound )
               {
                  tr.m_startTime = DateTime.MinValue;
                  tr.m_endTime   = DateTime.MinValue;
                  tr.m_updated   = DateTime.MinValue;
               }
            }
         }
         
         return tr;
      }

      //----------------------------------------------------------------------		
      // Write - need to either insert a new record or update an old one
      //         since update will happen far more often then inserts we
      //         will attempt that first and then fall back to insert
      //----------------------------------------------------------------------		
      internal void
         Write()
      {
	      Repository           rep = new Repository();
         string               sqlText = "";
         SqlCommand           cmd = null;
         

         lock (timesRecordLock)
         {
            try
            {
               rep.OpenConnection( CoreConstants.RepositoryTempDatabase );
            
               sqlText = String.Format( "UPDATE {0} " +
                                          "SET startTime={1},endTime={2},updated=GETUTCDATE() " +
                                          "WHERE instance={3} and traceType={4}",
                                       CoreConstants.RepositoryTemp_TimesTable,
                                       SQLHelpers.CreateSafeDateTimeString( m_startTime ),
                                       SQLHelpers.CreateSafeDateTimeString( m_endTime ),
                                       SQLHelpers.CreateSafeString(m_instance),
                                       m_traceType );
				int nRows = 0 ;
				using(cmd = new SqlCommand( sqlText, rep.connection ))
				{
					cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
			      
					nRows = cmd.ExecuteNonQuery();
				}
               if ( nRows == 0 )
               {
                  // no row to update - do the insert
                  sqlText = String.Format( "INSERT INTO {0} " +
                                             "(instance,traceType,startTime,endTime,updated) " +
                                             "VALUES ({1},{2},{3},{4},GETUTCDATE())",
                                          CoreConstants.RepositoryTemp_TimesTable,
                                          SQLHelpers.CreateSafeString(m_instance),
                                          m_traceType,
                                          SQLHelpers.CreateSafeDateTimeString( m_startTime ),
                                          SQLHelpers.CreateSafeDateTimeString( m_endTime ) );
				   using(cmd = new SqlCommand( sqlText, rep.connection ))
				   {
					   cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
					   cmd.ExecuteNonQuery();
				   }
               }
            }
            catch ( Exception ex )
            {
               throw ex;
            }
            finally
            {
               rep.CloseConnection();
            }
         }
      }

		#endregion
	}
}
