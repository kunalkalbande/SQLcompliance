#if NO_MORE_PRIV
using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Text;


namespace Idera.SQLcompliance.Core.TraceProcessing
{
	/// <summary>
	/// Summary description for DupRecord.
	/// </summary>
	internal class DupRecord
	{
	   private static Object dupRecordLock = new Object();
	   
      //------------
      // Properties	  
      //------------

      // persisted properties
	   public string     m_instance  = "";
	   public int        m_traceType = 0;
	   public int        m_spid      = 0;
	   
	   public DateTime   m_startTime = DateTime.MinValue;
	   public int        m_checksum  = 0;
	   public int        m_dupCount   = 0;
	
      //----------------------------------------------------------------------
      // Constructors
      //----------------------------------------------------------------------
	   public
	      DupRecord()
		{
		}
		
	   public
	      DupRecord(
	         string   instance,
	         int      traceType,
	         int      spid
	      )
		{
		   m_instance       = instance;
		   m_traceType      = traceType;
		   m_spid           = spid;
	      m_startTime      = DateTime.MinValue;
	      m_checksum       = 0;
	      m_dupCount       = 0;
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
		   
	      m_startTime      = SQLHelpers.GetDateTime( reader, col++ );
	      m_checksum       = SQLHelpers.GetInt32( reader, col++ );
	      m_dupCount       = SQLHelpers.GetInt32( reader, col++ );
		}
		
      //----------------------------------------------------------------------
      // Clear
      //----------------------------------------------------------------------
		internal void
		   Clear()
		{
	      m_startTime      = DateTime.MinValue;
	      m_checksum       = 0;
	      m_dupCount       = 0;
		}
		
      //----------------------------------------------------------------------
      // GetSelectSQL
      //----------------------------------------------------------------------
		internal string
		   GetSelectSQL()
		{
         string loadTemp = "SELECT startTime,checksum,dupCount " +
                           "FROM [{0}] " +
                           "WHERE instance={1} AND traceType={2} AND spid={3}";
                           
         return String.Format( loadTemp,
                               CoreConstants.RepositoryTemp_DupTable,
                               m_instance,
                               m_traceType,
                               m_spid );
		}
		
      //-----------------------------------------------------------------------
      // GetInsertPropsSQL
      //-----------------------------------------------------------------------
     internal string
         GetInsertPropsSQL()
      {
         StringBuilder insertSql = new StringBuilder();
         
         insertSql.Append( "instance" );
         insertSql.Append( ",traceType" );
         insertSql.Append( ",spid" );
         insertSql.Append( ",startTime" );
         insertSql.Append( ",checksum" );
         insertSql.Append( ",dupCount" );
         
         return insertSql.ToString();
      }      

      //-----------------------------------------------------------------------
      // GetInsertValuesSQL - Select for walking temp table
      //-----------------------------------------------------------------------
     internal string
         GetInsertValuesSQL()
      {
         StringBuilder insertSql = new StringBuilder();
         
         insertSql.AppendFormat( "{0}", SQLHelpers.CreateSafeString(m_instance) );
         insertSql.AppendFormat( ",{0}", m_traceType );
         insertSql.AppendFormat( ",{0}", m_spid );
         insertSql.AppendFormat( ",{0}", SQLHelpers.CreateSafeDateTimeString(m_startTime) );
         insertSql.AppendFormat( ",{0}", m_checksum );
         insertSql.AppendFormat( ",{0}", m_dupCount );
         
         return insertSql.ToString();
      }      
      
      //-----------------------------------------------------------------------
      // GetUpdateColumnsSQL
      //-----------------------------------------------------------------------
      internal string
         GetUpdateColumnsSQL()
      {
         StringBuilder updateSql = new StringBuilder();
         
         updateSql.AppendFormat( "startTime={0}", SQLHelpers.CreateSafeDateTimeString(m_startTime) );
         updateSql.AppendFormat( ",checksum={0}",  m_checksum );
         updateSql.AppendFormat( ",dupCount={0}",  m_dupCount );
         
         return updateSql.ToString();
      }
      
      //-----------------------------------------------------------------------
		// GetDupTable
      //-----------------------------------------------------------------------
		static internal IList
		   GetDupTable(
		      string                instance,
		      int                   traceType
		   )
		{
	      Repository     rep       = new Repository();
   		IList          dupList = null;
         string         cmdStr    = "";
         
         lock( dupRecordLock )
         {
			   try
			   {
               rep.OpenConnection( CoreConstants.RepositoryTempDatabase );
               
               cmdStr = String.Format( "SELECT spid,startTime,checksum,dupCount " +
                                       "FROM [{0}] " +
                                       "WHERE instance={1} AND traceType={2} ",
                                       CoreConstants.RepositoryTemp_DupTable,
                                       SQLHelpers.CreateSafeString(instance),
                                       traceType );
   			   
			      using ( SqlCommand cmd = new SqlCommand( cmdStr, rep.connection ) )
			      {
			         cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                  using ( SqlDataReader reader = cmd.ExecuteReader() )
                  {
				         dupList = new ArrayList();

			            while ( reader.Read() )
                     {
                        DupRecord dupRecord = new DupRecord();
                        
		                  dupRecord.m_instance  = instance;
		                  dupRecord.m_traceType = traceType;
                        
                        int col = 0;
                        dupRecord.m_spid       = SQLHelpers.GetInt32( reader, col ++ );
                        dupRecord.m_startTime  = SQLHelpers.GetDateTime( reader, col ++ );                  
                        dupRecord.m_checksum   = SQLHelpers.GetInt32( reader, col ++ );
                        dupRecord.m_dupCount   = SQLHelpers.GetInt32( reader, col ++ );
                        
                        dupList.Add( dupRecord );
                     }
                  }   
               }
			   }
			   catch( Exception ex )
			   {
               ErrorLog.Instance.Write( ErrorLog.Level.Verbose,
                                        "GetDupTable",
                                        ex );
				   dupList = new ArrayList();  // return an empty array
			   }
			   finally
			   {
			      rep.CloseConnection();
			   }
			}
			
			return dupList;
	   }
	   
	   //-------------------------------------------------------------------------
	   // Delete
	   //-------------------------------------------------------------------------
	   internal void
	      Delete()
	   {
	      Repository rep = new Repository();
	      string     cmdStr  = "";
	      
         lock( dupRecordLock )
         {
	         try
	         {
               rep.OpenConnection( CoreConstants.RepositoryTempDatabase );
               
		         cmdStr = "DELETE FROM {0} " +
		                  "WHERE instance={1} AND traceType={2} AND spid={3}";
   		                        
		         cmdStr = String.Format( cmdStr,
		                                 CoreConstants.RepositoryTemp_DupTable,
		                                 SQLHelpers.CreateSafeString(this.m_instance),
		                                 this.m_traceType,
		                                 this.m_spid );

		         using ( SqlCommand cmd = new SqlCommand( cmdStr, rep.connection ) )
		         {
			         cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
		            cmd.ExecuteNonQuery();
		         }
		      }
		      catch (Exception ex)
		      {
		         ErrorLog.Instance.Write( ErrorLog.Level.Debug,
		                                 "DupRecord::Delete",
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
	      
         lock( dupRecordLock )
         {
	         try
	         {
               rep.OpenConnection( CoreConstants.RepositoryTempDatabase );
               
		         cmdStr = "INSERT INTO {0} " +
		                  "({1}) VALUES ({2})";
   		                        
		         cmdStr = String.Format( cmdStr,
		                                 CoreConstants.RepositoryTemp_DupTable,
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
		                                 "DupRecord::Insert",
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
	      
         lock( dupRecordLock )
         {
	         try
	         {
               rep.OpenConnection( CoreConstants.RepositoryTempDatabase );
               
		         cmdStr = "UPDATE {0} " +
		                     "SET {1} " +
		                     "WHERE instance={2} AND traceType={3} AND spid={4}";
   		                        
		         cmdStr = String.Format( cmdStr,
		                                 CoreConstants.RepositoryTemp_DupTable,
		                                 GetUpdateColumnsSQL(),
		                                 SQLHelpers.CreateSafeString(this.m_instance),
		                                 this.m_traceType,
		                                 this.m_spid );

		         using ( SqlCommand cmd = new SqlCommand( cmdStr, rep.connection ) )
		         {
			         cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
		            cmd.ExecuteNonQuery();
		         }
		      }
		      catch (Exception ex)
		      {
		         ErrorLog.Instance.Write( ErrorLog.Level.Debug,
		                                  "DupRecord::Update",
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
#endif
