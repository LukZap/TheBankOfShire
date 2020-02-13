using Microsoft.EntityFrameworkCore;
using ShireBank.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ShireBank.Repos
{
    class AccountRepository : IAccountRepository
    {
        private readonly BankContext _dbContext;

        public AccountRepository(BankContext dbContext)
        {
            _dbContext = dbContext;
        }
        public Account Get(uint accountId)
        {
            return _dbContext.Accounts.SingleOrDefault(x => x.Id == accountId); 
        }

        public IEnumerable<AccountHistory> GetHistory(uint accountId)
        {
            return _dbContext.Accounts
                .Include(x => x.AccountHistories)
                .SingleOrDefault(x => x.Id == accountId)
                ?.AccountHistories
                    .OrderBy(x => x.EntryDate);
        }

        public IEnumerable<Account> GetWhere(Expression<Func<Account,bool>> predicate)
        {
            return _dbContext.Accounts.Where(predicate).ToList();
        }

        public void Add(Account account)
        {
            _dbContext.Accounts.Add(account);
            _dbContext.SaveChanges();

            AddHistoryEntry(account);
            _dbContext.SaveChanges();
        }

        public void Update(Account account)
        {
            Func<object> operation = () => {
                _dbContext.Entry(account).State = EntityState.Modified;
                AddHistoryEntry(account);

                _dbContext.SaveChanges();
                return null;
            };

            HandleConcurrentOperation(operation);
        }

        public bool Delete(uint accountId)
        {
            Func<bool> operation = () => {
                var account = _dbContext.Accounts.SingleOrDefault(x => x.Id == accountId);
                if (account == null) return false;

                _dbContext.Accounts.Remove(account);

                _dbContext.SaveChanges(); 
                return true;
            };

            return HandleConcurrentOperation(operation);
        }

        private T HandleConcurrentOperation<T>(Func<T> operation)
        {
            T result = default;
            var saved = false;

            // for concurrent issues checks
            // _dbContext.Database.ExecuteSqlRaw("UPDATE Accounts SET Balance = 600");

            while (!saved)
            {
                using (var transaction = _dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        result = operation();

                        transaction.Commit();
                        saved = true;
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        foreach (var entry in ex.Entries)
                        {
                            var proposedValues = entry.CurrentValues;
                            var databaseValues = entry.GetDatabaseValues();

                            foreach (var property in proposedValues.Properties)
                            {
                                if (property.Name == nameof(Account.Balance))
                                {
                                    var proposedValue = (float)proposedValues[property];
                                    var databaseValue = (float)databaseValues[property];
                                    var originalValue = (float)entry.OriginalValues[property];
                                    var debitLimit = (float)proposedValues[nameof(Account.Balance)];

                                    var calculatedNewBalance = proposedValue - originalValue + databaseValue;

                                    proposedValues[property] = calculatedNewBalance > -debitLimit ?
                                        calculatedNewBalance : -debitLimit;
                                }                                
                            }
                            
                            entry.OriginalValues.SetValues(databaseValues);
                        }
                    }
                }                
            }

            return result;
        }

        private void AddHistoryEntry(Account account)
        {
            var history = new AccountHistory
            {
                AccountId = account.Id,
                Balance = account.Balance,
                DebtLimit = account.DebtLimit,
                EntryDate = DateTime.Now,
                FirstName = account.FirstName,
                LastName = account.LastName
            };

            _dbContext.AccountHistories.Add(history);
        }
    }
}
