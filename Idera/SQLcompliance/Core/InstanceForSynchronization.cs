using System;
using System.Globalization;
using PluginCommon;

namespace Idera.SQLcompliance.Core
{
    public class InstanceForSynchronization
    {
        public int SrvId { get; set; }
        public string Instance { get; set; }
        public string SqlVersion { get; set; }
        public string Description { get; set; }
        public DateTime TimeLastCollection { get; set; }
        public string VersionName { get; set; }
        public string Owner { get; set; }
        public string Location { get; set; }
        public string Comments { get; set; }
        public DateTime TimeCreated { get; set; }


        public static explicit operator CreateInstance(InstanceForSynchronization instance)
        {
            if (instance == null) return null;

            var result = new CreateInstance();
            result.Comments = instance.Comments ?? "";
            result.Edition = instance.SqlVersion ?? "";
            result.Location = instance.Location ?? "";
            result.Name = instance.Instance ?? "";
            result.Owner = instance.Owner ?? "";
            result.UtcFirstSeen = instance.TimeCreated;
            result.UtcLastSeen = instance.TimeLastCollection.ToString(CultureInfo.InvariantCulture);
            result.Version = instance.VersionName ?? "";
            return result;
        }
    }
}
