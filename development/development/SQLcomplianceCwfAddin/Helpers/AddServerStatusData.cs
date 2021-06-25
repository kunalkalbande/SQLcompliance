using Idera.SQLcompliance.Core.Agent;

namespace SQLcomplianceCwfAddin.Helpers
{
    public class AddServerStatusData
    {
        public ServerRecord Server { get; set; }

        public bool ShouldIndexesToBeUpdated { get; set; }
    }
}
