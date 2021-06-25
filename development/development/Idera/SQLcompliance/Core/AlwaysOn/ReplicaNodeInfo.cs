using System;
using System.Runtime.Serialization;

namespace Idera.SQLcompliance.Core.AlwaysOn
{
    [Serializable]
    public class ReplicaNodeInfo : ISerializable
    {
        public string ReplicaServerName { get; set; }

        public ReplicaRole Role { get; set; }

        public OperationalState OperationalState { get; set; }

        public bool IsPrimary
        {
            get
            {
                return Role == ReplicaRole.Primary;
            }
        }

        public bool IsSecondary
        {
            get
            {
                return Role == ReplicaRole.Secondary;
            }
        }

        public bool IsAccessable
        {
            get
            {
                return OperationalState == OperationalState.Online ||
                       OperationalState == OperationalState.NotLocal;
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            try
            {
                info.AddValue("ReplicaServerName", ReplicaServerName);
                info.AddValue("Role", Role);
                info.AddValue("OperationalState", OperationalState);
            }
            catch (Exception e)
            {
                SerializationHelper.ThrowSerializationException(e, this.GetType());
            }
        }

        #region Constructors
        public ReplicaNodeInfo()
        {
        }

        public ReplicaNodeInfo(SerializationInfo info, StreamingContext context)
        {
            try
            {
                ReplicaServerName = info.GetString("ReplicaServerName");
                Role = (ReplicaRole)info.GetValue("Role", typeof(ReplicaRole));
                OperationalState = (OperationalState)info.GetValue("OperationalState", typeof(OperationalState));
            }
            catch( Exception e )
            {
                SerializationHelper.ThrowDeserializationException( e, this.GetType());
            }
        }
        #endregion
    }
}
