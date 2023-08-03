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
            return string.IsNullOrEmpty(bankName) ? "" : $"{bankName.Substring(0, Math.Min(3, bankName.Length))}{DateTime.Now:yyyyMMddHHmmss}";
        }

        public static string GenerateEmployeeID()
        {
            return $"Employee_{DateTime.Now.ToString("yyyyMMddHHmmss")}";
        }

        public static string GenerateTransactionId(string bankId, string accountId)
        {        
            return $"TXN{bankId}{accountId}{DateTime.Now.ToString("yyyyMMddHHmmss")}";
        }

        public static string GenerateAccountNumber(string name)
        {
            return string.IsNullOrEmpty(name) ? "" : $"{DateTime.Now.ToString("yyMMddHHmmss")}";
        }

        public static string GenerateAccountId(string name)
        {
            return string.IsNullOrEmpty(name) ? null : $"{name.Substring(0, Math.Min(3, name.Length)).ToUpper()}{DateTime.Now:yyMMddHHmmssfff}";
        }
    }
}
