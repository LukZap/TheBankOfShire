using Autofac;
using Microsoft.EntityFrameworkCore;
using SharedInterface;
using ShireBank.Helpers;
using ShireBank.Models;
using ShireBank.Repos;
using ShireBank.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShireBank
{
    static class IoC
    {
        public static IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();
            RegisterDependencies(builder);

            return builder.Build();
        }

        private static void RegisterDependencies(ContainerBuilder builder)
        {
            builder.RegisterType<BankContext>().WithParameter("options", BankContextOptionsFactory.Build());
            builder.RegisterType<ShireBankService>().AsSelf();
            builder.RegisterType<ShireBankService>().As<ICustomerInterface>();
            builder.RegisterType<AccountRepository>().As<IAccountRepository>();
            builder.RegisterType<OutputFormatter>().As<IOutputFormatter>().SingleInstance();
        }
    }
}
