using ShireBank.Models;
using System.Collections.Generic;

namespace ShireBank.Helpers
{
    public interface IOutputFormatter
    {
        string AccountHistoriesToHumanReadable(IEnumerable<AccountHistory> historiesList);
    }
}