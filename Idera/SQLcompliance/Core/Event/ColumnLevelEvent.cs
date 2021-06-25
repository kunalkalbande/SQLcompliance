using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Idera.SQLcompliance.Core.TraceProcessing;

namespace Idera.SQLcompliance.Core.Event
{
   abstract public class ColumnLevelEvent
   {
      public int eventId;
      public int alertEventType;
      public DateTime startTime;

      public ColumnLevelEvent()
      {
         eventId = -2100000000;
         startTime = DateTime.MinValue;
         alertEventType = 0;
      }

      abstract public void Load(SqlDataReader reader);
   }
}
