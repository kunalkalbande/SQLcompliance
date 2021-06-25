using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Idera.SQLcompliance.Core
{
    public class ActiveQueryCollector
    {
        bool isAssemblyLoaded = false;
        Assembly assembly;

        public Assembly Assembly
        {
            get { return assembly; }
            set { assembly = value; }
        }

        public bool IsAssemblyLoaded
        {
            get { return isAssemblyLoaded; }
            set { isAssemblyLoaded = value; }
        }

        public ActiveQueryCollector()
        {
            try
            {
                assembly = XEventHelper.loadAssembly("Microsoft.SqlServer.XEvent.Linq.dll");
                if (assembly != null)
                {
                    isAssemblyLoaded = true;
                }
            }
            catch
            {
                throw;
            }
        }

        public bool IsLinqAssemblyLoaded()
        {
            if (isAssemblyLoaded)
            {
                try
                {
                    AppDomain currentDomain = AppDomain.CurrentDomain;
                    currentDomain.AssemblyResolve += new ResolveEventHandler(XEventHelper.XEventDependencyHandler);
                    Type PublishedEventField = this.Assembly.GetType("Microsoft.SqlServer.XEvent.Linq.PublishedEventField");
                    if (PublishedEventField != null)
                        return true;
                    else
                        return false;
                }
                catch (Exception ex)
                {
                    ErrorLog.Instance.Write("Failed to load the assembly 'Microsoft.SqlServer.XEvent.Linq' or its dependencies. Exception: " + ex.Message);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
