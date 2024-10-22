﻿using ShireBank.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ShireBank.Repos
{
    public interface IAccountRepository
    {
        void Add(Account account);
        ConcurentResult Delete(Account account);
        Account Get(uint accountId);
        IEnumerable<AccountHistory> GetHistory(uint accountId);
        IEnumerable<Account> GetWhere(Expression<Func<Account, bool>> predicate);
        ConcurentResult Update(Account account);
    }
}
