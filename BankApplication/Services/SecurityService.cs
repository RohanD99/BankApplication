using BankApplication.Common;
using BankApplication.Models;
using static BankApplication.Common.Enums;

namespace BankApplication.Services
{
    internal class SecurityService
    {
        public User Login(string username, string password, UserType usertype)
        {
            if (usertype == UserType.Employee)
            {
                return DataStorage.Employees.Find(e => e.UserName == username && e.Password == password);
            }
            else if (usertype == UserType.AccountHolder)
            {
                return DataStorage.AccountHolders.Find(a => a.UserName == username && a.Password == password);
            }
            else if(usertype == UserType.Admin)
            {
                return DataStorage.Employees.Find(b => b.UserName == username && b.Password == password);
            }
            else
            {
                return null;
            }
        }
    }
}
