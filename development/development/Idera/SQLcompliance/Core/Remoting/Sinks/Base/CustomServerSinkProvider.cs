using System;
using System.Collections;
using System.Reflection;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;

namespace Idera.SQLcompliance.Core.Remoting.Sinks.Base
{
	public class CustomServerSinkProvider : BaseCustomSinkProvider, IServerChannelSinkProvider
	{
		private IServerChannelSinkProvider nextProvider;

		public CustomServerSinkProvider(IDictionary properties, ICollection providerData)
			: base(properties, providerData)
		{
		}

		public IServerChannelSinkProvider Next
		{
			get {return this.nextProvider; }
			set {this.nextProvider = value;}
		}

		public IServerChannelSink CreateSink(IChannelReceiver channel) 
		{
			BaseCustomSink customSinkObject = null;
			bool keepCreatingInstance = false;

			CallContext.SetData("perProviderState", this.perProviderState);

			object [] par = {new ServerSinkCreationData(this.data, channel)};
			try
			{
				customSinkObject = (BaseCustomSink)Activator.CreateInstance(this.customSinkType, par);
			}
			catch (MissingMethodException)
			{
				keepCreatingInstance = true;
			}
			catch (Exception e)
			{
				ExamineConstructionException(e);
			}

			if (keepCreatingInstance)
			{
				try
				{
					customSinkObject = (BaseCustomSink)Activator.CreateInstance(customSinkType);
				}
				catch (Exception e)
				{
					ExamineConstructionException(e);
				}
			}

			IServerChannelSink next = nextProvider.CreateSink(channel);

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

		public void GetChannelData(IChannelDataStore channelData)
		{
		}

		private void ExamineConstructionException(Exception e)
		{
			if (!(e.InnerException is ExcludeMeException))
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
