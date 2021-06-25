using System;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.ManagedInstances
{
    [Serializable]
    public enum SqlServerSecurityModel
    {
        IntegratedWindowsImpersonation = 0,
        User = 1,
        Integrated = 2
    }
}