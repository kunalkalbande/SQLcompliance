using System;
using Idera.SQLcompliance.Core ;

namespace Idera.SQLcompliance.Cluster
{
	/// <summary>
	/// Summary description for VirtualServer.
	/// </summary>
	public class VirtualServer
	{
		#region Member Variables

		private string _serverName ;
		private string _instanceName = "" ;
		private string _collectionServer ;
		private int _portNumber  ;
		private string _serviceUsername ;
		private string _servicePassword ;
		private string _traceDirectory ;
      private string _triggerAssemblyDirectory;
      private int? _instancePort;
      private string _instanceWithPort;

		#endregion

		#region Properties

		public string InstanceName
		{
			get 
         { 
            if(_instanceName != null && _instanceName.Length > 0)
               return _instanceName ;
            else
               return _serverName ;
         }
			set { _instanceName = value ; }
		}

		public string FullInstanceName
		{
			set
			{
				int index = value.IndexOf('\\') ;
				if(index == -1)
					_serverName = value ;
				else
				{
					_serverName = value.Substring(0, index) ;
					_instanceName = value.Substring(index + 1) ;
				}
			}
			get
			{
				if(_instanceName != null && _instanceName.Length > 0)
					return String.Format("{0}\\{1}", _serverName, _instanceName) ;
				else
					return _serverName ;
			}
		}

		public string ServerName
		{
			get { return _serverName ; }
			set { _serverName = value ; }
		}

		public string CollectionServer
		{
			get { return _collectionServer ; }
			set { _collectionServer = value ; }
		}

        public int? InstancePort
        {
            get { return _instancePort; }
            set { _instancePort = value; }
        }

        public string InstanceWithPort
        {
            get { return _instanceWithPort; }
            set { _instanceWithPort = value; }
        }

		public int PortNumber
		{
			get { return _portNumber ; }
			set { _portNumber = value ; }
		}

		public string ServiceName
		{
			get { return String.Format("SQLcomplianceAgent${0}", _serverName) ; }
		}

		public string ServiceUsername
		{
			get { return _serviceUsername ; }
			set { _serviceUsername = value ; }
		}

		public string ServicePassword
		{
			get { return _servicePassword ; }
			set { _servicePassword = value ; }
		}

		public string TraceDirectory
		{
			get { return _traceDirectory ; }
			set { _traceDirectory = value ; }
		}

      public string TriggerAssemblyDirectory
      {
         get { return _triggerAssemblyDirectory; }
         set { _triggerAssemblyDirectory = value; }
      }

		#endregion

		#region Construction/Destruction

		public VirtualServer()
		{
			_portNumber = CoreConstants.AgentServerTcpPort ;
		}

		#endregion

		public override string ToString()
		{
			return this.FullInstanceName ;
		}
	}
}
