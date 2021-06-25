using System;
using System.Collections.Generic;
using System.Text;

namespace Idera.SQLcompliance.Core
{
   [Serializable()]
   public class ClusterAgentUpgrade
   {
      public Dictionary<string, string> AssemblyDirectories = null;

      public ClusterAgentUpgrade()
      {
      }
   }
}
