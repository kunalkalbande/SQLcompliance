using System;

namespace Idera.SQLcompliance.Core.Remoting
{
    [Serializable]
    public class RemotingUser
    {
        public string Name { get; set; }
        public string Password { get; set; }
    }
}
