using SharedInterface;
using ShireBank.Helpers;
using ShireBank.Models;
using ShireBank.Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace ShireBank
{
    [ServiceBehavior(
        ConcurrencyMode = ConcurrencyMode.Multiple,
        InstanceContextMode = InstanceContextMode.PerSession, // TODO find out about other options
        IncludeExceptionDetailInFaults = true
    )]
    public partial class ShireBankService : ICustomerInterface, IInspectorInterface
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IOutputFormatter _outputFormatter;

        public ShireBankService(IAccountRepository accountRepository, IOutputFormatter outputFormatter)
        {
            _accountRepository = accountRepository;
            _outputFormatter = outputFormatter;
        }
    }
}
