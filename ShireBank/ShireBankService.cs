using SharedInterface;
using ShireBank.Helpers;
using ShireBank.Models;
using ShireBank.Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;

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
        private readonly EventWaitHandle _manualResetEvent;

        public ShireBankService(IAccountRepository accountRepository, IOutputFormatter outputFormatter)
        {
            _accountRepository = accountRepository;
            _outputFormatter = outputFormatter;
            _manualResetEvent = new EventWaitHandle(true, EventResetMode.ManualReset, "InspectorEvent");
        }
    }
}
