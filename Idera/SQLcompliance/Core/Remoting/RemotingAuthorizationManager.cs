namespace Idera.SQLcompliance.Core.Remoting
{
    public class RemotingAuthorizationManager : IRemotingAuthorizationManager
    {
        private string _validUser = "RemotingUser";
        private string _validPassword = "RemotingPassword";
        public bool IsValid(RemotingUser user)
        {
            if (user == null || user.Name == null || user.Password == null) return false;

            return user.Name == _validUser && user.Password == _validPassword;
        }
    }
}
