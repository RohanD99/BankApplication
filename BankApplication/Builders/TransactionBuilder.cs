using BankApplication.Models;
using System.Collections.Generic;
using System.Text;

namespace BankApplication.Builders
{
    public class TransactionBuilder
    {
        public static string GetTransactionDetails(List<Transaction> transactions)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var transaction in transactions)
            {
                sb.AppendLine($"Transaction ID: {transaction.Id}");
                sb.AppendLine($"Transaction Type: {transaction.Type}");
                sb.AppendLine($"Transaction Amount: {transaction.Amount}");
                sb.AppendLine($"Transaction Date: {transaction.CreatedOn}");
                sb.AppendLine("----------------------------");
            }
            return sb.ToString();
        }
    }
}
