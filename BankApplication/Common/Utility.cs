using BankApplication.Models;
using BankApplication.Views;
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
            return $"{namePrefix}{DateTime.Now:yyMMddHHmmssfff}";
        }

        public static string GetUpdatedValue(string currentValue, string field)
        {
            Console.WriteLine($"Current {field}: {currentValue}");
            string newValue = Utility.GetStringInput($"Enter new {field} (leave empty to keep the current one):", false);
            return string.IsNullOrEmpty(newValue) ? currentValue : newValue;
        }
    }
}
