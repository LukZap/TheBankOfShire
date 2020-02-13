using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShireBank.Models
{
    public class Account
    {
        public uint Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public float DebtLimit { get; set; }
        [ConcurrencyCheck]
        public float Balance { get; set; }
        public ICollection<AccountHistory> AccountHistories { get; set; }
    }
}
