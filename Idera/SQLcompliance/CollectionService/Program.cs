using System;
using Idera.SQLcompliance.Core.Collector;

namespace Idera.SQLcompliance.CollectionService
{
    class Program
    {
        private static CollectionServer collectionServer;

        private static void Main()
        {
            bool wasStarted = false;

            Console.WriteLine("Starting collection service...");

            try
            {
                collectionServer = CollectionServer.Instance;
                collectionServer.Start();
                wasStarted = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Collection service hasn't started because of the following exception:");
                Console.WriteLine(ex.ToString());
                Console.WriteLine("Press any key to exit.");
                Console.ReadLine();
            }

            if (wasStarted)
            {
                Console.WriteLine("Collection service has started.");
                Console.WriteLine("Press any key to stop the service.");
                Console.ReadLine();
                Console.WriteLine("Stopping collection service...");
                collectionServer.Stop();
                Console.WriteLine("Collection service has stopped.");
            }
            
        }
    }
}
