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
                new EndpointAddress(Constants.FullBankAddress)))
            {
            }
        }
    }
}
