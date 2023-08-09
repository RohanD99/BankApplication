using BankApplication.Common;
using System;
using System.Linq;
using static BankApplication.Common.Enums;

namespace BankApplication.Services
{
    internal class SecurityService
    {
        public T Login<T>(string username, string password, UserType usertype)
        {
            if (usertype == UserType.Employee)
            {
                return (T)Convert.ChangeType(DataStorage.Employees.FirstOrDefault(e => e.UserName == username && e.Password == password), typeof(T));
            }
            else if (usertype == UserType.AccountHolder)
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
