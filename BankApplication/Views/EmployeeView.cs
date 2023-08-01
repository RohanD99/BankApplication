using BankApplication.Common;
using BankApplication.Models;
using BankApplication.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using static BankApplication.Common.Enums;

namespace BankApplication.Views
{
    internal class EmployeeView
    {  
        BankService BankService = new BankService();
        public void UserAccountMenu(AccountHolder loggedInAccount)
        {
            UserAccountOption option;
            do
            {
                Utility.GenerateOptions(Constants.UserAccountOption);

                option = (UserAccountOption)Convert.ToInt32(Console.ReadLine());
                switch (option)
                {
                    case UserAccountOption.Deposit:
                        Utility.GetStringInput("Enter the amount to deposit: ", true);
                        decimal depositAmount = Convert.ToDecimal(Console.ReadLine());
                        Response<string> depositResponse = BankService.Deposit(loggedInAccount, depositAmount);
                        Console.WriteLine(depositResponse.Message);
                        break;

                    case UserAccountOption.Withdraw:
                        Utility.GetStringInput("Enter the amount to withdraw: ", true);
                        decimal withdrawAmount = Convert.ToDecimal(Console.ReadLine());
                        Response<string> WithdrawResponse = BankService.Withdraw(loggedInAccount, withdrawAmount);
                        Console.WriteLine(WithdrawResponse.Message);
                        break;

                    case UserAccountOption.Transfer:
                        EmployeeService.TransferFundsMenu(loggedInAccount);
                        break;

                    case UserAccountOption.CheckBalance:
                        Response<string> BalanceResponse = BankService.CheckBalance(loggedInAccount);
                        Console.WriteLine($"Your account balance: {BalanceResponse.Data}");
                        break;

                    case UserAccountOption.Transactions:
                        Response<string> TransactionHistoryResponse = BankService.ViewTransactionHistory(loggedInAccount);
                        Console.WriteLine(TransactionHistoryResponse.Message);
                        Console.WriteLine(TransactionHistoryResponse.Data);
                        break;

                    case UserAccountOption.Logout:
                        break;

                    default:
                        Console.WriteLine("Please enter a valid input.");
                        break;
                }
            } while (option != UserAccountOption.Logout);
        }
    }
}

