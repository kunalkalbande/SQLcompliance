using System;
using System.Collections;
using System.Reflection;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;
using System.Threading;

namespace Idera.SQLcompliance.Core.Remoting.Sinks.Base
{
	public class CustomClientSinkProvider : BaseCustomSinkProvider, IClientChannelSinkProvider
	{
		private delegate BaseCustomSink CustomSinkCreator(IChannelSender channel, string url, object remoteChannelData);

		private IClientChannelSinkProvider nextProvider;
        private object sinkCreator;

		public CustomClientSinkProvider(IDictionary properties, ICollection providerData) 
			: base(properties, providerData)
		{
			this.sinkCreator = new CustomSinkCreator(this.CreateCustomSink);
		}

		public IClientChannelSinkProvider Next
		{
			get {return this.nextProvider; }
			set {this.nextProvider = value;}
		}

		public IClientChannelSink CreateSink(IChannelSender channel, string url, object remoteChannelData) 
		{
			BaseCustomSink customSinkObject;

			CallContext.SetData("perProviderState", this.perProviderState);

			IClientChannelSink next = this.nextProvider.CreateSink(channel, url, remoteChannelData);

			customSinkObject = ((CustomSinkCreator)this.sinkCreator)(channel, url, remoteChannelData);

			if (customSinkObject != null)
			{
				customSinkObject.SetNextSink(next);
				return customSinkObject;
			}
			else
			{
				return next;
			}
		}

		private BaseCustomSink CreateCustomSink(IChannelSender channel, string url, object remoteChannelData)
		{
			BaseCustomSink customSinkObject;
			CustomSinkCreator nullCreator = new CustomSinkCreator(NullCustomSink);

			try
			{
				customSinkObject = CreateCustomSink1(channel, url, remoteChannelData);
				if (nullCreator != (CustomSinkCreator)this.sinkCreator)
				{
					Interlocked.Exchange(ref this.sinkCreator, 
						new CustomSinkCreator(CreateCustomSink1));
				}
				return customSinkObject;
			}
			catch (Exception e)
			{
				if (!(e.InnerException is MissingMethodException))
				{
					throw;
				}
			}

			customSinkObject = CreateCustomSink2(channel, url, remoteChannelData);
			if (nullCreator != (CustomSinkCreator)this.sinkCreator)
			{
					Interlocked.Exchange(ref this.sinkCreator, 
						new CustomSinkCreator(CreateCustomSink2));
			}
			return customSinkObject;

		}

		private BaseCustomSink CreateCustomSink1(IChannelSender channel, string url, object remoteChannelData)
		{
			BaseCustomSink customSinkObject = null;

			object [] par = {new ClientSinkCreationData(this.data, channel, url, remoteChannelData)};
			try
			{
				customSinkObject = (BaseCustomSink)Activator.CreateInstance(this.customSinkType, par);
			}
			catch (Exception e)
			{
				ExamineConstructionException(e);
			}

			return customSinkObject;
		}

		private BaseCustomSink CreateCustomSink2(IChannelSender channel, string url, object remoteChannelData)
		{
			BaseCustomSink customSinkObject = null;

			try
			{
				customSinkObject = (BaseCustomSink)Activator.CreateInstance(this.customSinkType);
			}
			catch (Exception e)
			{
				ExamineConstructionException(e);
			}

			return customSinkObject;
		}

		private BaseCustomSink NullCustomSink(IChannelSender channel, string url, object remoteChannelData)
		{
			return null;
		}

		private void ExamineConstructionException(Exception e)
		{
			if (e.InnerException is ExcludeMeException)
			{
				ExcludeMeException excludeExeption = (ExcludeMeException)e.InnerException;
				if (excludeExeption.ExcludeMePermanently)
				{
					Interlocked.Exchange(ref this.sinkCreator, 
						new CustomSinkCreator(NullCustomSink));
				}
			}
			else
			{
				if (e is TargetInvocationException)
				{
					throw new CustomSinkException(
						string.Format("Could not create instance of {0}. {1} was thrown during construction. Message: {2}", 
						this.customSinkType.ToString(), e.InnerException.GetType().ToString(),
						e.InnerException.Message), e);
				}
				else
				{
					throw new CustomSinkException(
						string.Format("Could not create instance of {0}.", 
						this.customSinkType.ToString()), e);
				}
			}
		}
	}
}
