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
        InstanceContextMode = InstanceContextMode.PerSession,
        IncludeExceptionDetailInFaults = true
    )]
    public class ShireBankService : ICustomerInterface
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IOutputFormatter _outputFormatter;

        public ShireBankService(IAccountRepository accountRepository, IOutputFormatter outputFormatter)
        {
            _accountRepository = accountRepository;
            _outputFormatter = outputFormatter;
        }

        public uint? OpenAccount(string firstName, string lastName, float debtLimit)
        {
            var accountExists = _accountRepository
                .GetWhere(x => x.FirstName == firstName && x.LastName == lastName)
                .Any();

            if (accountExists) return null;

            var newAccount = new Account
            {
                FirstName = firstName,
                LastName = lastName,
                DebtLimit = debtLimit
            };

            _accountRepository.Add(newAccount);

            return newAccount.Id;
        }

        public float Withdraw(uint account, float amount)
        {
            var dbAccount = _accountRepository.Get(account);

            var balanceBeforeWithdraw = dbAccount.Balance;
            var balanceAfterWithdraw = dbAccount.Balance - amount;

            if (-dbAccount.DebtLimit > balanceAfterWithdraw)
                dbAccount.Balance = -dbAccount.DebtLimit;
            else
                dbAccount.Balance = balanceAfterWithdraw;

            var retreivedAmount = _accountRepository.Update(dbAccount);

            return retreivedAmount ?? balanceBeforeWithdraw - dbAccount.Balance;
        }

        public void Deposit(uint account, float amount)
        {
            var dbAccount = _accountRepository.Get(account);

            dbAccount.Balance += amount;

            _accountRepository.Update(dbAccount);
        }

        public string GetHistory(uint account)
        {
            var historiesList = _accountRepository.GetHistory(account);
            return _outputFormatter.AccountHistoriesToHumanReadable(historiesList);
        }

        public bool CloseAccount(uint account)
        {
            var dbAccount = _accountRepository.Get(account);
            if (dbAccount.Balance != 0.0f)
                return false;

            _accountRepository.Delete(dbAccount);
            return true;
        }
    }
}
