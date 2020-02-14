using ShireBank.Models;
using System.Linq;

namespace ShireBank
{
    public partial class ShireBankService
    {
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

            var updateResult = _accountRepository.Update(dbAccount);

            if (updateResult.AccountWasDeleted)
                return 0.0f;

            return -updateResult.BalanceChange ?? balanceBeforeWithdraw - dbAccount.Balance;
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

            var deleteResult = _accountRepository.Delete(dbAccount);
            if (deleteResult.AccountBalanceWasUpdated)
                return false;

            return true;
        }
    }
}
