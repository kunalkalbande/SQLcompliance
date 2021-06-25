namespace Idera.SQLcompliance.Core.AlwaysOn
{
    public enum OperationalState
    {
        NotLocal = -1,
        PendingFailover = 0,
        Pending = 1,
        Online = 2,
        Offline = 3,
        Failed = 4,
        FailedNoQuorum = 5,
    }
}
