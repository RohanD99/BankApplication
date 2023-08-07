using BankApplication.Common;
using BankApplication.Models;
using System;
using System.Linq;

namespace BankApplication.Services
{
    internal class SecurityService
    {
        public static T Login<T>(string username, string password, Type userType)
        {
            if (userType == typeof(Employee))
            {
                return (T)Convert.ChangeType(DataStorage.Employees.FirstOrDefault(e => e.UserName == username && e.Password == password), typeof(T));
            }
            else if (userType == typeof(AccountHolder))
            {
                return (T)Convert.ChangeType(DataStorage.AccountHolders.FirstOrDefault(a => a.UserName == username && a.Password == password), typeof(T));
            }
            else
            {
                return default(T);
            }
        }
    }
}
