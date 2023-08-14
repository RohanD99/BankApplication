using BankApplication.Models;
using System.Collections.Generic;

namespace BankApplication.Common
{
    internal static class DataStorage
    {
        // Storing banks
        public static List<Bank> Banks = new List<Bank>();

        // Storing employees
        public static List<Employee> Employees = new List<Employee>();

        // Storing accounts
        public static List<AccountHolder> AccountHolders = new List<AccountHolder>();

        // Storing transactions
        public static List<Transaction> Transactions = new List<Transaction>();       
    }
}
