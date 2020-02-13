using Autofac;
using Autofac.Integration.Wcf;
using SharedInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ShireBank
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Shire Bank app started");

            var container = IoC.BuildContainer();

            using (var scope = container.BeginLifetimeScope())
            {
                var serviceHost = new ServiceHost(typeof(ShireBankService), new Uri(Constants.BankBaseAddress));
                serviceHost.AddServiceEndpoint(typeof(ICustomerInterface), new BasicHttpBinding(), Constants.ServiceName);
                serviceHost.AddDependencyInjectionBehavior<ShireBankService>(scope);
                serviceHost.Open();
                Console.ReadKey();

                serviceHost.Close();
            }
        }
    }
}
