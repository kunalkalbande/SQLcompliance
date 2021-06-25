using System;
using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Collector;

namespace SQLcomplianceCwfAddin.Helpers.Server
{
    internal class ServerManagerHelper
    {
        public static void SetReindexFlag(bool reindex, SQLcomplianceConfiguration configuration)
        {
            ServerManager srvManager;
            string url;

            // check for collection service - cant uninstall if it is down or unreachable
            try
            {
                url = String.Format("tcp://{0}:{1}/{2}",
                                    configuration.Server,
                                    configuration.ServerPort,
                                    typeof(ServerManager).Name);

                srvManager = (ServerManager)Activator.GetObject(typeof(ServerManager), url);
                srvManager.SetReindexFlag(reindex);
            }
            catch (Exception)
            {
                // TODO:  Should we alert the user when we can't talk to the collection server?
                // TODO: Use proxy with authentication
            }
        }
    }
}
