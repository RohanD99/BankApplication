using BankApplication.Models;
using BankApplication.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BankApplication.Common
{
    internal class Utility
    {
        public static void GenerateOptions(List<string> options)
        {
            foreach (var item in options.Select((value, index) => new { index, value }))
            {
                Console.WriteLine($"{item.index} {item.value}");
            }
        }

        public static string GetStringInput(string type, bool isRequired)
        {
            string input = string.Empty;
            do
            {     
                Console.Write($"Please enter {type}: ");
                input = Console.ReadLine();

                if (isRequired && string.IsNullOrEmpty(input))
                {
                    Console.WriteLine($"Please provide valid {type}.");
                }
            } while (isRequired && string.IsNullOrEmpty(input));

            return input;
        }


        public static string GenerateBankId(string bankName)
        {
            string currentDate = DateTime.Now.ToString("yyyyMMddHHmmss");
            return bankName.Substring(0, 3) + currentDate;
        }

        public static string GenerateEmployeeID()
        {
            return $"Employee_{DateTime.Now.ToString("yyyyMMddHHmmss")}";
        }

        public static string GenerateTransactionId(string bankId, string accountId)
        {
            string currentDate = DateTime.Now.ToString("yyyyMMddHHmmss");
            return $"TXN{bankId}{accountId}{currentDate}";
        }

        public static string GenerateAccountNumber(string name)
        {       
            return $"{name.Substring(0, Math.Min(3, name.Length)).ToUpper()}{DateTime.Now:yyMMddHHmmss}";
        }

        public static string GenerateAccountId(string name)
        {
            string namePrefix = name.Substring(0, Math.Min(3, name.Length)).ToUpper();
            string currentDate = DateTime.Now.ToString("yyMMddHHmmssfff");
            string accountId = $"{namePrefix}{currentDate}";
            return accountId;
        }

        public static string GetUpdatedValue(string currentValue, string field)
        {
            Console.WriteLine($"Current {field}: {currentValue}");
            string newValue = Utility.GetStringInput($"Enter new {field} (leave empty to keep the current one):", false);
            return string.IsNullOrEmpty(newValue) ? currentValue : newValue;
        }

        public static Action<string> GetConsoleWriteLineDelegate()
        {
            return Console.WriteLine;
        }

        public static void PrintTransactionDetails(List<Transaction> transactions)
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
            Console.WriteLine(sb.ToString());
        }

        public static void PrintAccountDetails(IEnumerable<AccountHolder> Accounts)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var account in Accounts)
            {
                sb.AppendLine($"Account Number: {account.AccountNumber}");
                sb.AppendLine($"Constants.AccountHolderName: {account.Name}");
                sb.AppendLine($"Balance: {account.Balance}");
                sb.AppendLine($"Account Type: {account.AccountType}");
                sb.AppendLine("----------------------------");
            }
            Console.WriteLine(sb.ToString());
        }

        public static string GetTransactionHistoryString(List<Transaction> transactions)
        {
            if (transactions == null || transactions.Count == 0)
            {
                return "No transaction history found.";
            }

            string result = string.Empty;
            foreach (var transaction in transactions)
            {
                result += $"Transaction ID: {transaction.Id}\n";
                result += $"Transaction Type: {transaction.Type}\n";
                result += $"Transaction Amount: {transaction.Amount}\n";
                result += $"Transaction Date: {transaction.CreatedOn}\n";
                result += "----------------------------\n";
            }

            return result;
        }

        public static Employee GetEmployeeByUsernameAndPassword(string username, string password)
        {
            return DataStorage.Employees.FirstOrDefault(e => e.UserName == username && e.Password == password);
        }

        public static AccountHolder GetAccountHolderByUsernameAndPassword(string username, string password)
        {
            return DataStorage.Accounts.FirstOrDefault(a => a.UserName == username && a.Password == password);
        }

      

    }
}
