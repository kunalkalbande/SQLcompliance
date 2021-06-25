using System;
using System.Collections.Generic;
using System.Text;

namespace Idera.SQLcompliance.Core
{
   public class AgentHeartBeat
   {
      private DateTime _lastHeartbeat;
      private string _agent;
      private int _heartbeatInterval;

      public DateTime LastHeartbeat
      {
         get { return _lastHeartbeat; }
         set { _lastHeartbeat = value; }
      }

      public string Agent
      {
         get { return _agent; }
         set { _agent = value; }
      }

      public int HeartbeatInterval
      {
         get { return _heartbeatInterval; }
         set { _heartbeatInterval = value; }
      }
   }
}
