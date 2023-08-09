using BankApplication.Common;
using BankApplication.Models;
using BankApplication.Services;
using System;
using static BankApplication.Common.Enums;

namespace BankApplication.Views
{
    internal class EmployeeView
    {  
        BankService bankService = new BankService();
        AccountHolderService accountHolderService = new AccountHolderService();
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
                        Console.Write("Enter your account ID: ");
                        string accountHolderID = Console.ReadLine();
                        Console.Write("Enter the amount to deposit: ");
                        string depositAmountInput = Console.ReadLine();
                        if (!decimal.TryParse(depositAmountInput, out decimal depositAmount))
                        {
                            Console.WriteLine("Invalid amount. Please enter a valid decimal value.");
                            break;
                        }

                        AccountHolder accountHolder = accountHolderService.GetAccountHolderById(accountHolderID);

                        if (accountHolder != null)
                        {
                            Response<string> depositResponse = bankService.Deposit(accountHolder, depositAmount);

                            Console.WriteLine(depositResponse.Message);
                        }
                        else
                        {
                            Console.WriteLine("Account holder not found.");
                        }
                        break;

                    case UserAccountOption.Withdraw:
                        Utility.GetStringInput("Enter the amount to withdraw: ", true);
                        decimal withdrawAmount = Convert.ToDecimal(Console.ReadLine());
                        Response<string> WithdrawResponse = bankService.Withdraw(loggedInAccount, withdrawAmount);
                        Console.WriteLine(WithdrawResponse.Message);
                        break;

                    case UserAccountOption.Transfer:
                        BankView bankView = new BankView();
                        bankView.GetTransferFunds(loggedInAccount);
                        break;

                    case UserAccountOption.CheckBalance:
                        Response<string> balanceResponse = bankService.CheckBalance(loggedInAccount);
                        Console.WriteLine($"Your account balance: {balanceResponse.Data}");
                        break;

                    case UserAccountOption.Transactions:
                        TransactionService transactionService = new TransactionService();
                        Response<string> transactionHistoryResponse = transactionService.GetTransactionHistory(loggedInAccount);

                        if (transactionHistoryResponse.IsSuccess)
                        {
                            Console.WriteLine(transactionHistoryResponse.Message);
                            Console.WriteLine(transactionHistoryResponse.Data);
                        }
                        else
                        {
                            Console.WriteLine(transactionHistoryResponse.Message);
                        }
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


