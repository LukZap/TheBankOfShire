using SharedInterface;
using System;
using System.ServiceModel;

namespace InspectorTest
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var channelFactory = new ChannelFactory<IInspectorInterface>(
                new BasicHttpBinding { SendTimeout = TimeSpan.FromMinutes(1) },
                new EndpointAddress(Constants.FullInspectorAddress)))
            {
                var inspector = channelFactory.CreateChannel();

                Console.WriteLine("Press key to start inspection...");
                Console.ReadKey();

                inspector.StartInspection();
                Console.WriteLine(inspector.GetFullSummary());

                Console.WriteLine("Press key to finish inspection...");
                Console.ReadKey();

                inspector.FinishInspection();

                Console.WriteLine();
                Console.WriteLine("Press key to close...");
                Console.ReadKey();
            }
        }
    }
}
