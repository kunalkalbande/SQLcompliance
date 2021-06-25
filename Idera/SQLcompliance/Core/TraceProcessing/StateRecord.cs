using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Text;


namespace Idera.SQLcompliance.Core.TraceProcessing
{
	/// <summary>
	/// Summary description for StateRecord.
	/// </summary>
	internal class StateRecord : TraceRow
	{
	   private static Object stateRecordLock = new Object();
	   
      //------------
	   // Constants
      //------------
      public const int      staleEventAge = 15; // minutes - flush if it has been 
                                                //           sitting around this long
                                                //           waiting for a mate
      //------------
      // Properties	  
      //------------

      // persisted properties
	   public string     m_instance  = "";
	   public int        m_traceType = 0;
	   public int        m_state     = 0;
	   public int        m_flushed   = 0;
	   public int        m_flushedEventId;
	   public DateTime   m_updated = DateTime.MinValue;  // last time event touched
	   // Note: 
	   // (1) spid and startTime are in inherited TraceRow properties
	   // (2) rest of properties are from TraceRow class
 
      // in-memory only properties	   
	   public int        m_row       = -1;   // exists in DataRow
	   public int        m_persisted = 0;    // exists in persistence datastore
	
      //----------------------------------------------------------------------
      // Constructors
      //----------------------------------------------------------------------
	   public
	      StateRecord()
		{
		   m_instance  = "";
		   m_traceType = 0;
		   spid        = 0;
		}

      public
         StateRecord(
	         string                 instance,
	         int                    traceType,
	         int                    inSpid
	   )
		{
		   m_instance  = instance;
		   m_traceType = traceType;
		   spid        = inSpid;
		}

      //----------------------------------------------------------------------
      // IsStale
      //----------------------------------------------------------------------
      internal bool
         IsStale()
      {
         if ( m_updated == DateTime.MinValue ) return true;
         
         DateTime staleTime = DateTime.UtcNow;
         staleTime = staleTime.AddMinutes( -staleEventAge );
         if (staleTime.CompareTo(m_updated) < 0 ) return true;
         
         return false;
      }

		
      //----------------------------------------------------------------------
      // Load
      //----------------------------------------------------------------------
		private void
		   Load(
		      SqlDataReader reader
		   )
		{
		   int        col = 0;
		   
	      m_state          = SQLHelpers.GetInt32( reader, col++ );
	      m_flushed        = SQLHelpers.GetInt32( reader, col++ );
	      m_flushedEventId = SQLHelpers.GetInt32( reader, col++ );
	      m_updated        = SQLHelpers.GetDateTime( reader, col++ );
	      
	      m_persisted      = 1;   // read from db
	      m_row            = -1;  // not in DataRow

         // load rest of event data	      
	      base.LoadReader( reader, col );
		}
		
      //----------------------------------------------------------------------
      // Clear
      //----------------------------------------------------------------------
		new internal void
		   Clear()
		{
	      m_state          = TraceJob.state_None;
	      m_flushed        = 0;
	      m_flushedEventId = 0;
	      m_updated        = DateTime.MinValue;
	      m_persisted      = 0;
	      m_row            = -1;
	      
	      base.Clear();
		}
		
      //----------------------------------------------------------------------
      // GetSelectSQL
      //----------------------------------------------------------------------
      internal string
         GetSelectSQL( 
         bool  is2005
         )
      {
         string loadTemp = "SELECT {1} " +
                           "FROM [{0}] " +
                           "WHERE instance={2} AND traceType={3} and SPID={4}";
                           
         return String.Format( loadTemp,
                               CoreConstants.RepositoryTemp_StatesTable,
                               StateRecord.GetColumnsSQL( is2005, true ),
                               m_instance,
                               m_traceType,
                               spid );
		}
		
      //-----------------------------------------------------------------------
      // GetColumnsSQL - Select for walking temp table
      //-----------------------------------------------------------------------
      new static internal string
         GetColumnsSQL(
         bool is2005
         )
      {
         string SQL = "state,flushed,flushedEventId,updated,";
         SQL += TraceRow.GetColumnsSQL( is2005, true );
         return SQL;
      }      
      
      //-----------------------------------------------------------------------
      // GetInsertPropsSQL
      //-----------------------------------------------------------------------
      new internal string
         GetInsertPropsSQL()
      {
         StringBuilder insertSql = new StringBuilder();
         
         insertSql.Append( base.GetInsertPropsSQL() );
         insertSql.Append(",guid"); // 5.5
         insertSql.Append( ",instance" );
         insertSql.Append( ",traceType" );
         insertSql.Append( ",state" );
         insertSql.Append( ",flushed" );
         insertSql.Append( ",flushedEventId" );
         insertSql.Append( ",updated" );
         
         return insertSql.ToString();
      }      

      //-----------------------------------------------------------------------
      // GetInsertValuesSQL - Select for walking temp table
      //-----------------------------------------------------------------------
      new internal string
         GetInsertValuesSQL()
      {
         StringBuilder insertSql = new StringBuilder();
         
         insertSql.Append( base.GetInsertValuesSQL() );
         
         insertSql.AppendFormat( ",{0}", SQLHelpers.CreateSafeString(m_instance) );
         insertSql.AppendFormat( ",{0}", m_traceType );
         insertSql.AppendFormat( ",{0}", m_state );
         insertSql.AppendFormat( ",{0}", m_flushed );
         insertSql.AppendFormat( ",{0}", m_flushedEventId );
         insertSql.AppendFormat( ",GETUTCDATE()" ); // m_updated
         
         return insertSql.ToString();
      }      
      
      //-----------------------------------------------------------------------
      // GetUpdateColumnsSQL
      //-----------------------------------------------------------------------
      new internal string
         GetUpdateColumnsSQL()
      {
         StringBuilder updateSql = new StringBuilder();
         
         updateSql.Append( base.GetUpdateColumnsSQL() );
         
         updateSql.AppendFormat( ",state={0}",          m_state );
         updateSql.AppendFormat( ",flushed={0}",        m_flushed );
         updateSql.AppendFormat( ",flushedEventId={0}", m_flushedEventId );
         
         return updateSql.ToString();
      }
      
      //-----------------------------------------------------------------------
		// GetStateTable
      //-----------------------------------------------------------------------
		static internal IList
		   GetStateTable(
		      string                instance,
		      int                   traceType
		   )
		{
	      Repository     rep       = new Repository();
   		IList          stateList = null;
         string         cmdStr    = "";
         
         lock( stateRecordLock )
         {
			   try
			   {
               rep.OpenConnection( CoreConstants.RepositoryTempDatabase );
               
               cmdStr = String.Format( "SELECT {1},guid " +      
                                       "FROM [{0}] " +
                                       "WHERE instance={2} AND traceType={3}",
                                       CoreConstants.RepositoryTemp_StatesTable,
                                       StateRecord.GetColumnsSQL( true ), /* always have 2005 columns */
                                       SQLHelpers.CreateSafeString(instance),
                                       traceType );
   			   
			      using ( SqlCommand    cmd    = new SqlCommand( cmdStr, rep.connection ) )
			      {
			         cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                  using ( SqlDataReader reader = cmd.ExecuteReader() )
                  {
				         stateList = new ArrayList();

			            while ( reader.Read() )
                     {
                        StateRecord stateRecord = new StateRecord();
                        
                        int col = 0;
                        stateRecord.m_state          = SQLHelpers.GetInt32( reader, col ++ );
                        stateRecord.m_flushed        = SQLHelpers.GetInt32( reader, col ++ );
                        stateRecord.m_flushedEventId = SQLHelpers.GetInt32( reader, col ++ );
                        stateRecord.m_updated        = SQLHelpers.GetDateTime( reader, col ++ );                  
                        
                        stateRecord.LoadReader( reader, col );
                        
		                  stateRecord.m_instance  = instance;
		                  stateRecord.m_traceType = traceType;
                        stateRecord.m_persisted = 1;
                        
                        stateList.Add( stateRecord );
                     }
                  }   
               }
			   }
			   catch( Exception ex )
			   {
               ErrorLog.Instance.Write( ErrorLog.Level.Verbose,
                                        String.Format( "GetStateTable {0} - {1}", instance, traceType ),
                                        ex );
               throw ex;                         
				   //stateList = new ArrayList();  // return an empty array
			   }
			   finally
			   {
			      rep.CloseConnection();
			   }
			}
			
			return stateList;
	   }
	   
	   //-------------------------------------------------------------------------
	   // Delete
	   //-------------------------------------------------------------------------
	   internal void
	      Delete()
	   {
	      Repository rep = new Repository();
	      string     cmdStr  = "";
	      
         lock( stateRecordLock )
         {
	         try
	         {
               rep.OpenConnection( CoreConstants.RepositoryTempDatabase );
               
		         cmdStr = "DELETE FROM {0} " +
		                  "WHERE instance={1} AND traceType={2} AND SPID={3}";
   		                        
		         cmdStr = String.Format( cmdStr,
		                                 CoreConstants.RepositoryTemp_StatesTable,
		                                 SQLHelpers.CreateSafeString(this.m_instance),
		                                 this.m_traceType,
		                                 this.spid );

		         using ( SqlCommand cmd = new SqlCommand( cmdStr, rep.connection ) )
		         {
			         cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
		            cmd.ExecuteNonQuery();
		         }
		      }
		      catch (Exception ex)
		      {
		         ErrorLog.Instance.Write( ErrorLog.Level.Debug,
		                                 "StateRecord::Delete",
		                                 cmdStr,
		                                 ex );
		         throw ex;
		      }
			   finally
			   {
			      rep.CloseConnection();
			   }
			}
	   }
	   
	   //-------------------------------------------------------------------------
	   // Insert
	   //-------------------------------------------------------------------------
	   internal void
	      Insert()
	   {
	      Repository rep = new Repository();
	      string     cmdStr  = "";
	      
         lock( stateRecordLock )
         {
	         try
	         {
               rep.OpenConnection( CoreConstants.RepositoryTempDatabase );
               
		         cmdStr = "INSERT INTO {0} " +
		                  "({1}) VALUES ({2})";
   		                        
		         cmdStr = String.Format( cmdStr,
		                                 CoreConstants.RepositoryTemp_StatesTable,
		                                 GetInsertPropsSQL(),
		                                 GetInsertValuesSQL() );

		         using ( SqlCommand cmd = new SqlCommand( cmdStr, rep.connection ) )
		         {
			         cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
		            cmd.ExecuteNonQuery();
		         }
		      }
		      catch (Exception ex)
		      {
		         ErrorLog.Instance.Write( ErrorLog.Level.Debug,
		                                 "StateRecord::Insert",
		                                 cmdStr,
		                                 ex );
		         throw ex;
		      }
			   finally
			   {
			      rep.CloseConnection();
			   }
			}
	   }

	   //-------------------------------------------------------------------------
	   // Update
	   //-------------------------------------------------------------------------
	   internal void
	      Update()
	   {
	      Repository rep = new Repository();
	      string     cmdStr  = "";
	      
         lock( stateRecordLock )
         {
	         try
	         {
               rep.OpenConnection( CoreConstants.RepositoryTempDatabase );
               
		         cmdStr = "UPDATE {0} " +
		                     "SET {1} " +
		                     "WHERE instance={2} AND traceType={3} AND SPID={4}";
   		                        
		         cmdStr = String.Format( cmdStr,
		                                 CoreConstants.RepositoryTemp_StatesTable,
		                                 GetUpdateColumnsSQL(),
		                                 SQLHelpers.CreateSafeString(this.m_instance),
		                                 this.m_traceType,
		                                 this.spid );

		         using ( SqlCommand cmd = new SqlCommand( cmdStr, rep.connection ) )
		         {
			         cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
		            cmd.ExecuteNonQuery();
		         }
		      }
		      catch (Exception ex)
		      {
		         ErrorLog.Instance.Write( ErrorLog.Level.Debug,
		                                 "StateRecord::Update",
		                                 cmdStr,
		                                 ex );
		         throw ex;
		      }
			   finally
			   {
			      rep.CloseConnection();
			   }
			}
	   }
	}
}
