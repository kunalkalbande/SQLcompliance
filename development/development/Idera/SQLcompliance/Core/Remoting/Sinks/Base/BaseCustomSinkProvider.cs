using System;
using System.Collections;
using System.Reflection;
using System.Runtime.Remoting.Channels;

namespace Idera.SQLcompliance.Core.Remoting.Sinks.Base
{
    public abstract class BaseCustomSinkProvider
    {
        protected SinkProviderData data;
        protected Type customSinkType;
        protected object perProviderState;
        protected const string initMethodName = "Init";

        public BaseCustomSinkProvider(IDictionary properties, ICollection providerData)
        {
            string customSinkType = (string)properties["customSinkType"];
            if (customSinkType == null)
            {
                throw new CustomSinkException("no customSinkType property in the <provider> element.");
            }
            this.customSinkType = Type.GetType(customSinkType);
            if (this.customSinkType == null)
            {
                throw new CustomSinkException(
                    string.Format("Could not load type {0}", customSinkType));
            }

            // make sure the custom sink type inherits BaseCustomSink
            if (!this.customSinkType.IsSubclassOf(typeof(BaseCustomSink)))
            {
                throw new CustomSinkException("Custom sink type does not inherit from BaseCustomSink");
            }

            // see if there is a <customData> element in the provider data
            // and save it for passing it to the custom sink's constructor
            foreach (SinkProviderData data in providerData)
            {
                if (data.Name == "customData")
                {
                    this.data = data;
                    break;
                }
            }

            Type[] paramTypes = { typeof(SinkProviderData), Type.GetType("System.Object&") };
            MethodInfo initMethod = this.customSinkType.GetMethod(initMethodName, paramTypes);
            if (initMethod != null)
            {
                object[] param = { this.data, null };
                try
                {
                    initMethod.Invoke(null, param);
                    this.perProviderState = param[1];
                }
                catch (TargetInvocationException e)
                {
                    throw new CustomSinkException(
                        string.Format("The static Init method of type {0} threw a {1} exception. Message: {2}",
                        this.customSinkType.ToString(), e.InnerException.GetType().ToString(),
                        e.InnerException.Message),
                        e);
                }
            }

        }
    }
}
