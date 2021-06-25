using System;
using System.Collections;

namespace Idera.SQLcompliance.Core.TraceProcessing
{
	/// <summary>
	/// Summary description for StateTable.
	/// </summary>
	internal class StateTable
	{
	   public string   m_instance;
	   public int      m_traceType;
	   public IList    m_records;
	
      //-----------------------------------------------------------------------
		// Constructor
      //-----------------------------------------------------------------------
		public
		   StateTable(
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
		static public StateTable
		   Read(
		      string                instance,
		      int                   traceType
		   )
		{
		   StateTable stateTable = new StateTable( instance, traceType );
		   
		   stateTable.m_records = StateRecord.GetStateTable( instance,
		                                                     traceType );   
		                                          
         return stateTable;		                                          
		}
		
      //-----------------------------------------------------------------------
		// Write
      //-----------------------------------------------------------------------
		public void
		   Write()
		{
		   if ( m_records == null ) return;
		   
		   foreach ( StateRecord stateRecord in m_records )
		   {
		      if ( stateRecord.m_state == TraceJob.state_None )
		      {
               stateRecord.Delete();
		      }
		      else
		      {
		         if ( stateRecord.m_persisted == 1 )
		            stateRecord.Update();
		         else
		            stateRecord.Insert();
		      }
		   }
		}
		
      //-----------------------------------------------------------------------
		// GetRecord - Get StateRecord for a SPID
      //-----------------------------------------------------------------------
		public StateRecord
		   GetRecord(
		      int                   spid
		   )
		{
		   StateRecord stateRecord = null;
		   
		   bool found = false;

		   foreach ( StateRecord sr in m_records )
		   {
		      if ( sr.spid == spid )
		      {
		         stateRecord = sr;
		         found = true;
		         break;
		      }
		   }
		   
		   if ( ! found )
		   {
		      stateRecord = new StateRecord( this.m_instance,
		                                     this.m_traceType,
		                                     spid );
		      m_records.Add( stateRecord );
		   }

         return stateRecord;		   
		}
		
      //-----------------------------------------------------------------------
		// PutRecord
      //-----------------------------------------------------------------------
		public void
		   PutRecord(
		      StateRecord           stateRecord
		   )
		{
		   for ( int i=0; i<m_records.Count; i++ )
		   {
		      StateRecord sr = (StateRecord)m_records[i];
		      
		      if ( sr.spid == stateRecord.spid )
		      {
		         m_records[i] = stateRecord;
		         break;
		      }
		   }
		}
		
	}
}

