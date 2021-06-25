#if NO_MORE_PRIV
using System;
using System.Collections;

namespace Idera.SQLcompliance.Core.TraceProcessing
{
	/// <summary>
	/// Summary description for DupCount.
	/// </summary>
	internal class DupCount
	{
	   public int       m_spid;
	   public DateTime  m_startTime;
	   public int       m_checksum;
	   public int       m_dupCount;
	   
	   
	   static ArrayList  spidArray = new ArrayList();
	   
		public
		   DupCount(
            int       spid,
	         DateTime  startTime,
	         int       checksum
		   )
		{
		   m_spid      = spid;
		   m_startTime = startTime;
		   m_checksum  = checksum;
		   m_dupCount  = 0;
		}
		
		static public void
		   Initialize()
		{
		   spidArray.Clear();
		}
		
		static public int
		   GetDuplicateCount(
	         int       spid,
	         DateTime  startTime,
	         int       checksum
		   )
		{
		   int dupCount = 0;
		   bool found     = false;
		   
		   for ( int i=0;
		         i < spidArray.Count;
		         i ++ )
		   {
		      if ( ((DupCount)spidArray[i]).m_spid == spid )
		      {
   		      if ( ((DupCount)spidArray[i]).m_checksum == checksum && 
	   	           ((DupCount)spidArray[i]).m_startTime == startTime )
	   	      {
   		         dupCount = ++((DupCount)spidArray[i]).m_dupCount;
		         }
		         else
		         {
                  ((DupCount)spidArray[i]).m_checksum  = checksum;
	   	         ((DupCount)spidArray[i]).m_startTime = startTime;
   		         ((DupCount)spidArray[i]).m_dupCount  = 0;
   		      }
   		      found = true;
		         break;
		      }
		   }
		   
		   if ( ! found )
		   {
		      DupCount dc = new DupCount( spid, startTime, checksum );
		      spidArray.Add( dc );
		      dupCount = 0;
		   }
		   
		   return dupCount;
		}
	}
}
#endif
