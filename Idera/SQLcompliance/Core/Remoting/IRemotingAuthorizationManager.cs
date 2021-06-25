namespace Idera.SQLcompliance.Core.Remoting
{
    public interface IRemotingAuthorizationManager
    {
        bool IsValid(RemotingUser user);
    }
}
