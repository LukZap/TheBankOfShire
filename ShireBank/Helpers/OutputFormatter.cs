using ShireBank.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShireBank.Helpers
{
    public class OutputFormatter : IOutputFormatter
    {
        public string AccountHistoriesToHumanReadable(IEnumerable<AccountHistory> historiesList)
        {
            StringBuilder sb = new StringBuilder();
            Dictionary<string, int> dict = new Dictionary<string, int>();

            var props = typeof(AccountHistory).GetProperties().Where(x => x.Name != nameof(AccountHistory.Id));

            foreach (var prop in props)
                dict.Add(prop.Name, prop.Name.Length);
            dict[nameof(AccountHistory.EntryDate)] = 22;

            int i = 0;
            StringBuilder formatBuilder = new StringBuilder();
            formatBuilder.Append("|");
            foreach (var item in dict)
            {
                formatBuilder.Append($"{{{i++},{item.Value}}}|");
            }

            var formatStr = formatBuilder.ToString();
            sb.AppendLine(String.Format(formatStr, dict.Keys.ToArray()));

            foreach (var h in historiesList)
                sb.AppendLine(String.Format(formatStr, props.Select(x => x.GetValue(h)).ToArray()));

            return sb.ToString();
        }
    }
}
