using System;

namespace Idera.SQLcompliance.Core.Rules.Alerts
{
	public enum SmtpAuthProtocol
	{
		Anonymous = 0,
		Basic,
		NTLM
	}

	/// <summary>
	/// Summary description for SmtpConfiguration.
	/// </summary>
	public class SmtpConfiguration
	{
		#region Member Variables

		private string _server ;
		private int _port ;
		private SmtpAuthProtocol _authType ;
		private bool _useSsl ;
		private string _username ;
		private string _password ;
		private string _senderAddress ;
		private string _senderName ;

		#endregion

		#region Properties

		public string Server
		{
			get { return _server ; }
			set 
			{
				if(value != null)
					_server = value ; 
				else
					_server = "" ;
			}
		}

		public int Port
		{
			get { return _port ; }
			set { _port = value ; }
		}

		public SmtpAuthProtocol Authentication
		{
			get { return _authType ; }
			set { _authType = value ; }
		}

		public string Username
		{
			get { return _username ; }
			set 
			{
				if(value != null)
					_username = value ; 
				else
					_username = "" ;
			}
		}

		public string Password
		{
			get { return _password ; }
			set 
			{
				if(value != null)
					_password = value ; 
				else
					_password = "" ;
			}
		}

		public string SenderAddress
		{
			get { return _senderAddress ; }
			set 
			{
				if(value != null)
					_senderAddress = value ; 
				else
					_senderAddress = "" ;
			}
		}

		public string SenderName
		{
			get { return _senderName ; }
			set 
			{
				if(value != null)
					_senderName = value ; 
				else
					_senderName = "" ;
			}
		}

		public bool UseSsl
		{
			get { return _useSsl ; }
			set { _useSsl = value ; }
		}

		#endregion

		#region Construction/Destruction

		public SmtpConfiguration()
		{
			_server = "" ;
			_port = 25 ;
			_authType = SmtpAuthProtocol.Anonymous ;
			_useSsl = false ;
			_username = "" ;
			_password = "" ;
			_senderAddress = "" ;
			_senderName = "" ;
		}

		#endregion

		public override bool Equals(object obj)
		{
			if (obj == null) return false;

			if (this.GetType() != obj.GetType()) return false;

			// safe because of the GetType check
			SmtpConfiguration config = (SmtpConfiguration)obj;     

			// use this pattern to compare value members
			if(!_port.Equals(config._port)) return false ;
			if(!_authType.Equals(config._authType)) return false ;
			if(!_useSsl.Equals(config._useSsl)) return false ;

			// use this pattern to compare reference members
			if(!Object.Equals(_server, config._server)) return false ;
			if(!Object.Equals(_username, config._username)) return false ;
			if(!Object.Equals(_password, config._password)) return false ;
			if(!Object.Equals(_senderAddress, config._senderAddress)) return false ;
			if(!Object.Equals(_senderName, config._senderName)) return false ;

			return true ;
		}

      public override int GetHashCode()
      {
         return _server.GetHashCode() ;
      }


		public SmtpConfiguration Clone()
		{
			return (SmtpConfiguration)MemberwiseClone(); 
		}
	}
}
