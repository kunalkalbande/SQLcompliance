#if NO_MORE_PRIV
using System;
using System.Collections;
using System.Data.SqlClient;

namespace Idera.SQLcompliance.Core.TraceProcessing
{
	/// <summary>
	/// Summary description for DupTable.
	/// </summary>
	internal class DupTable
	{
	   public string   m_instance;
	   public int      m_traceType;
	   public IList    m_records;
	
      //-----------------------------------------------------------------------
		// Constructor
      //-----------------------------------------------------------------------
		public
		   DupTable(
		      string                instance,
		      int                   traceType
		   )
		{
		   m_instance  = instance;
		   m_traceType = traceType;
		   m_records   = null;
		}
		
      //-----------------------------------------------------------------------
		// Read
      //-----------------------------------------------------------------------
		static public DupTable
		   Read(
		      string                instance,
		      int                   traceType
		   )
		{
		   DupTable dupTable = new DupTable( instance, traceType );
		   
		   dupTable.m_records = DupRecord.GetDupTable( instance,
		                                               traceType );   
		                                          
         return dupTable;		                                          
		}
		
      //-----------------------------------------------------------------------
		// Write
      //-----------------------------------------------------------------------
		public void
		   Write(
		      DateTime lastTraceFileTime
		   )
		{
		   // delete existing table
		   Delete();
		   
		   // write new records
		   if ( m_records == null ) return;
		   
		   foreach ( DupRecord dupRecord in m_records )
		   {
		      // only write records that will match on other side (same time)
		      if ( lastTraceFileTime.CompareTo( dupRecord.m_startTime ) == 0 )
		      {
               dupRecord.Insert();
		      }
		   }
		}
		
      //-----------------------------------------------------------------------
		// GetDupCount
      //-----------------------------------------------------------------------
		public int
		   GetDupCount(
		      int                   spid,
		      DateTime              startTime,
		      int                   checksum
		   )
		{
		   int  dupCount  = 0;
		   
		   bool found = false;

		   foreach ( DupRecord dup in m_records )
		   {
		      if ( dup.m_spid == spid )
		      {
		         if ( dup.m_checksum == checksum 
		                 && dup.m_startTime.CompareTo(startTime)==0 )
		         {
		            dupCount = ++ dup.m_dupCount;
		         } 
		         else
		         {
                  dup.m_checksum  = checksum;
                  dup.m_startTime = startTime;
		            dup.m_dupCount = 0;
		         }
		         
		         found = true;
		         break;
		      }
		   }
		   
		   if ( ! found )
		   {
		      DupRecord dupRecord = new DupRecord( this.m_instance,
		                                           this.m_traceType,
		                                           spid );
            dupRecord.m_checksum  = checksum;
            dupRecord.m_startTime = startTime;
            dupRecord.m_dupCount  = 0;
            		                                           
		      m_records.Add( dupRecord );
		   }

         return dupCount;		   
		}
		
      //-----------------------------------------------------------------------
		// Delete
      //-----------------------------------------------------------------------
		internal void
		   Delete()
		{
	      Repository     rep       = new Repository();
         string         cmdStr    = "";
         
			try
			{
            rep.OpenConnection( CoreConstants.RepositoryTempDatabase );
            
            cmdStr = String.Format( "DELETE FROM [{0}] " +
                                    "WHERE instance={1} AND traceType={2} ",
                                    CoreConstants.RepositoryTemp_DupTable,
                                    SQLHelpers.CreateSafeString(m_instance),
                                    m_traceType );
   			
			   using ( SqlCommand cmd = new SqlCommand( cmdStr, rep.connection ) )
			   {
			      cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
               cmd.ExecuteNonQuery();
            }
			}
			catch( Exception ex )
			{
            ErrorLog.Instance.Write( ErrorLog.Level.Verbose,
                                       "DupTable::Delete",
                                       ex );
			}
			finally
			{
			   rep.CloseConnection();
			}
	   }
	}
}

#endif
