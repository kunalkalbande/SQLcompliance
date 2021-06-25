using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Idera.SQLcompliance.Core
{
    [Serializable]
    public struct AlwaysOnAVG : ISerializable
    {
        public string AvailGroupId;
        public string AvailGroupName;
        public int NodesCount;

        #region Constructor
        public AlwaysOnAVG(
           string inAvailGroupId,
           string inAvailGroupName,
           int inNodesCount)
        {
            AvailGroupId = inAvailGroupId;
            AvailGroupName = inAvailGroupName;
            NodesCount = inNodesCount;
        }

        // Deserialization constructor
        public AlwaysOnAVG(
           SerializationInfo info,
           StreamingContext context)
        {
            AvailGroupId = null;
            AvailGroupName = null;
            NodesCount = 0;

            try
            {
                AvailGroupId = info.GetString("AvailGroupId");
                AvailGroupName = info.GetString("AvailGroupName");
                NodesCount = info.GetInt32("NodesCount");
            }
            catch (Exception e)
            {
                SerializationHelper.ThrowDeserializationException(e, this.GetType());
            }
        }
        #endregion

        #region ISerializable Members
        // Required ISerializable member
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            try
            {
                info.AddValue("AvailGroupId", AvailGroupId);
                info.AddValue("AvailGroupName", AvailGroupName);
                info.AddValue("NodesCount", NodesCount);
            }
            catch (Exception e)
            {
                SerializationHelper.ThrowSerializationException(e, this.GetType());
            }

        }

        #endregion
    }
}
