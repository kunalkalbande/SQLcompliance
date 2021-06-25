using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Serialization;

namespace Idera.SQLcompliance.Core.Remoting.Sinks.Base
{
	[Serializable]
	public class CustomSinkException : RemotingException
	{
		public CustomSinkException()
		{
		}

		public CustomSinkException(string message) : base(message)
		{
		}

		protected CustomSinkException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public CustomSinkException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}

	public class ExcludeMeException : Exception
	{
		private bool excludeMePermanently = false;

		public ExcludeMeException()
		{
		}

		public ExcludeMeException(bool excludeMePermanently)
		{
			this.excludeMePermanently = excludeMePermanently;
		}

		public bool ExcludeMePermanently
		{
			get {return this.excludeMePermanently;}
		}
	}
    
	public class SinkCreationData
	{
		public SinkCreationData(SinkProviderData configurationData)
		{
			this.ConfigurationData = configurationData;
		}

		public readonly SinkProviderData ConfigurationData;
	}

	public class ClientSinkCreationData : SinkCreationData
	{
		public ClientSinkCreationData(SinkProviderData configurationData, IChannelSender channel, string url, object remoteChannelData)
			: base(configurationData)
		{
			this.Channel = channel;
			this.Url = url;
			this.RemoteChannelData = remoteChannelData;
		}

		public readonly IChannelSender Channel;
		public readonly string Url;
		public readonly object RemoteChannelData;
	}

	public class ServerSinkCreationData : SinkCreationData
	{
		public ServerSinkCreationData(SinkProviderData configurationData, IChannelReceiver channel)
			: base(configurationData)
		{
			this.Channel = channel;
		}

		public readonly IChannelReceiver Channel;
	}
}
