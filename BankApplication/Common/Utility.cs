using System;
using System.Collections.Generic;
using System.Linq;

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

        public static string GetStringInput(string type, bool isRequired, string defaultValue = "")
        {
            Console.Write($"Please enter {type}");
            if (!string.IsNullOrEmpty(defaultValue))
            {
                Console.Write($" (or press Enter to keep the current value '{defaultValue}')");
            }
            Console.Write(": ");
            string input = Console.ReadLine();

            if (isRequired && string.IsNullOrEmpty(input))
            {
                Console.WriteLine($"Please provide a valid {type}.");
                return GetStringInput(type, isRequired, defaultValue);
            }

            return string.IsNullOrEmpty(input) ? defaultValue : input;
        }

        public static string GenerateBankId(string bankName)
        {
            return string.IsNullOrEmpty(bankName) ? string.Empty : (bankName.Substring(0, Math.Min(3, bankName.Length)) + DateTime.Now.ToString("yyyyMMddHHmmss"));
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
            return string.IsNullOrEmpty(name) ? string.Empty : $"{name.Substring(0, Math.Min(3, name.Length)).ToUpper()}{DateTime.Now:yyMMddHHmmss}";
        }

        public static string GenerateAccountId(string bankName)
        {
           return string.IsNullOrEmpty(bankName) ? string.Empty : (bankName.Substring(0, Math.Min(3, bankName.Length)) + DateTime.Now.ToString("yyyyMMddHHmmss"));
        }

        public static string GetUpdatedValue(string currentValue, string field)
        {
            Console.WriteLine($"Current {field}: {currentValue}");
            string newValue = Utility.GetStringInput($"Enter new {field} (leave empty to keep the current one):", false);
            return string.IsNullOrEmpty(newValue) ? currentValue : newValue;
        }
    }
}
