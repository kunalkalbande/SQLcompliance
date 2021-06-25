namespace Idera.SQLcompliance.Core.EmailSummaryNotification
{
    public class MailConfiguration
    {
        private string _senderAddress;
        private string _server;
        private int _port;
        private bool _useSSL;
        private int _authentication;
        private string _username;
        private string _password;

        public string SenderAddress
        {
            get { return _senderAddress; }
            set { _senderAddress = value; }
        }

        public string Server
        {
            get { return _server; }
            set { _server = value; }
        }

        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }

        public bool UseSSL
        {
            get { return _useSSL; }
            set { _useSSL = value; }
        }

        public int Authentication
        {
            get { return _authentication; }
            set { _authentication = value; }
        }

        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }

        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }
    }
}
