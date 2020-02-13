using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShireBank.Models
{
    public class AccountHistory
    {
        public Guid Id { get; set; }
        public uint AccountId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public float DebtLimit { get; set; }
        public float Balance { get; set; }
        public DateTime EntryDate { get; set; }
    }
}
