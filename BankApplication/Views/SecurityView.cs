using BankApplication.Common;
using BankApplication.Models;
using BankApplication.Services;
using System;
using static BankApplication.Common.Enums;

namespace BankApplication.Views
{
    internal class SecurityView
    {
        public void LoginAsBankStaff()
        {
            SecurityService securityService = new SecurityService();
            string username = Utility.GetStringInput("Username", true);
            string password = Utility.GetStringInput("Password", true);
            Employee loggedInEmployee = securityService.Login<Employee>(username, password, UserType.Employee);

            if (loggedInEmployee != null)
            {
                string bankId = Utility.GetStringInput("Bank ID", true);
                AccountHolderView.InitiateBankStaff(bankId);
            }
            else
                Console.WriteLine("Account not found");
        }

        public void LoginAsAccountHolder()
        {
            SecurityService securityService = new SecurityService();
            EmployeeView EmployeeView = new EmployeeView();
            string username = Utility.GetStringInput("Username", true);
            string password = Utility.GetStringInput("Password", true);
            AccountHolder loggedInAccountHolder = securityService.Login<AccountHolder>(username, password, UserType.AccountHolder);
            if (loggedInAccountHolder != null)
                EmployeeView.InitiateUserAccount(loggedInAccountHolder);
            else
                Console.WriteLine("Account not found");
        }
    }
}
