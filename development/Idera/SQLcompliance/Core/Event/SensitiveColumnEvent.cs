using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Idera.SQLcompliance.Core.TraceProcessing;

namespace Idera.SQLcompliance.Core.Event
{
   public class SensitiveColumnEvent : ColumnLevelEvent
   {
      public string instanceName;
      public string dataseName;
      public string objectName;
      public string columnName;
      public string hostName;
      public string loginName;
      public string applicationName;
      public long rowCounts;

      public SensitiveColumnEvent()
      {
         instanceName = String.Empty;
         dataseName = String.Empty;
         objectName = String.Empty;
         columnName = String.Empty;
         hostName = String.Empty;
         loginName = String.Empty;
         applicationName = String.Empty;
         rowCounts = 0;
      }

      public override void Load(SqlDataReader reader)
      {
         int column = 0;

         eventId = SQLHelpers.GetInt32(reader, column++);
         startTime = SQLHelpers.GetDateTime(reader, column++);
         instanceName = SQLHelpers.GetString(reader, column++);
         dataseName = SQLHelpers.GetString(reader, column++);
         objectName = SQLHelpers.GetString(reader, column++);
         columnName = SQLHelpers.GetString(reader, column++);
         hostName = SQLHelpers.GetString(reader, column++);
         loginName = SQLHelpers.GetString(reader, column++);
         applicationName = SQLHelpers.GetString(reader, column++);
         rowCounts = SQLHelpers.GetLong(reader, column++);
         alertEventType = SQLHelpers.GetInt32(reader, column++);
      }
   }
}
