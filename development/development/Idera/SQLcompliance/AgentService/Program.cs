using System;
using Idera.SQLcompliance.Core.Agent;

namespace Idera.SQLcompliance.AgentService
{
    class Program
    {
        private static SQLcomplianceAgent agentServer;

        private static void Main()
        {
            bool wasStarted = false;

            Console.WriteLine("Starting agent service...");

            try
            {
                agentServer = SQLcomplianceAgent.Instance;
                agentServer.Start();
                wasStarted = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("agent service hasn't started because of the following exception:");
                Console.WriteLine(ex.ToString());
                Console.WriteLine("Press any key to exit.");
                Console.ReadLine();
            }

            if (wasStarted)
            {
                Console.WriteLine("agent service has started.");
                Console.WriteLine("Press any key to stop the service.");
                Console.ReadLine();
                Console.WriteLine("Stopping agent service...");
                agentServer.Stop();
                Console.WriteLine("agent service has stopped.");
            }
        }
    }
}
