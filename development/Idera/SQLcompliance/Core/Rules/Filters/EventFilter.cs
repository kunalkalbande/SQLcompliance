using System;

namespace Idera.SQLcompliance.Core.Rules.Filters
{
	/// <summary>
	/// Summary description for EventFilter.
	/// </summary>
	public class EventFilter : EventRule
	{
		public EventFilter()
		{
		   _name = "New Filter" ;
		}

      public new EventFilter Clone()
      {
         return (EventFilter)base.Clone() ;
      }
   }
}
