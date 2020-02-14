using System;
using System.Text;

namespace ShireBank
{
    public partial class ShireBankService
    {
        public string GetFullSummary()
        {
            StringBuilder sb = new StringBuilder();

            var allAccounts = _accountRepository.GetWhere(x => true);
            foreach (var account in allAccounts)
            {
                sb.AppendLine($"Inspection for Account ID - {account.Id}");
                var history = _accountRepository.GetHistory(account.Id);
                var humanString = _outputFormatter.AccountHistoriesToHumanReadable(history);
                sb.AppendLine(humanString);
            }

            return sb.ToString();
        }

        public void StartInspection()
        {
            _manualResetEvent.Reset();
        }

        public void FinishInspection()
        {
            _manualResetEvent.Set();
        }
    }
}
