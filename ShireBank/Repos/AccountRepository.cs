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

        public ConcurentResult Update(Account account)
        { 
            return HandleConcurrentOperation(() => {
                _dbContext.Entry(account).State = EntityState.Modified;
                _dbContext.SaveChanges();

                AddHistoryEntry(account);
                _dbContext.SaveChanges();
            });
        }

        public ConcurentResult Delete(Account account)
        {
            return HandleConcurrentOperation(() => {
                _dbContext.Accounts.Remove(account);
                _dbContext.SaveChanges();
            });
        }

        private ConcurentResult HandleConcurrentOperation(Action operation)
        {
            var result = new ConcurentResult();
            var saved = false;

            while (!saved)
            {
                using (var transaction = _dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        operation();

                        transaction.Commit();
                        saved = true;
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        foreach (var entry in ex.Entries)
                        {
                            var proposedValues = entry.CurrentValues;
                            var databaseValues = entry.GetDatabaseValues();

                            if (databaseValues == null)
                            {
                                result.AccountWasDeleted = true;
                                return result;
                            }

                            if (entry.State == EntityState.Deleted)
                            {
                                result.AccountBalanceWasUpdated = true;
                                return result;
                            }

                            foreach (var property in proposedValues.Properties)
                            {
                                if (property.Name == nameof(Account.Balance))
                                {
                                    var proposedValue = (float)proposedValues[property];
                                    var databaseValue = (float)databaseValues[property];
                                    var originalValue = (float)entry.OriginalValues[property];
                                    var debitLimit = -(float)proposedValues[nameof(Account.DebtLimit)];

                                    var balanceChange = proposedValue - originalValue;
                                    var calculatedNewBalance = balanceChange + databaseValue;

                                    if (calculatedNewBalance > debitLimit)
                                    {
                                        proposedValues[property] = calculatedNewBalance;
                                        result.BalanceChange = balanceChange;
                                    }
                                    else
                                    {
                                        proposedValues[property] = debitLimit;
                                        result.BalanceChange = calculatedNewBalance - debitLimit;
                                    }
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
