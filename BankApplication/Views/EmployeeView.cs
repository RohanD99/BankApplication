using BankApplication.Common;
using BankApplication.Models;
using BankApplication.Services;
using System;
using System.Collections.Generic;
using System.Text;
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
                        Console.Write("Enter your account ID: ");
                        string accountHolderID = Console.ReadLine();

                        Console.Write("Enter the amount to deposit: ");
                        string depositAmountInput = Console.ReadLine();
                        if (!decimal.TryParse(depositAmountInput, out decimal depositAmount))
                        {
                            Console.WriteLine("Invalid amount. Please enter a valid decimal value.");
                            break;
                        }
                        Response<string> depositResponse = BankService.Deposit(accountHolderID, depositAmount);
                        Console.WriteLine(depositResponse.Message);
                        break;

                    case UserAccountOption.Withdraw:
                        Utility.GetStringInput("Enter the amount to withdraw: ", true);
                        decimal withdrawAmount = Convert.ToDecimal(Console.ReadLine());
                        Response<string> WithdrawResponse = BankService.Withdraw(loggedInAccount, withdrawAmount);
                        Console.WriteLine(WithdrawResponse.Message);
                        break;

                    case UserAccountOption.Transfer:
                        BankView bankView = new BankView();
                        bankView.TransferFunds(loggedInAccount);
                        break;

                    case UserAccountOption.CheckBalance:
                        Response<string> balanceResponse = BankService.CheckBalance(loggedInAccount);
                        Console.WriteLine($"Your account balance: {balanceResponse.Data}");
                        break;

                    case UserAccountOption.Transactions:
                        TransactionService transactionService = new TransactionService();
                        Response<string> transactionHistoryResponse = transactionService.ViewTransactionHistory(loggedInAccount);
                        Console.WriteLine(transactionHistoryResponse.Message);
                        Console.WriteLine(transactionHistoryResponse.Data);
                        break;

                    case UserAccountOption.Logout:
                        break;

                    default:
                        Console.WriteLine("Please enter a valid input.");
                        break;
                }
            } while (option != UserAccountOption.Logout);
        }

        public static void PrintTransactionDetails(List<Transaction> transactions)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var transaction in transactions)
            {
                sb.AppendLine($"Transaction ID: {transaction.Id}");
                sb.AppendLine($"Transaction Type: {transaction.Type}");
                sb.AppendLine($"Transaction Amount: {transaction.Amount}");
                sb.AppendLine($"Transaction Date: {transaction.CreatedOn}");
                sb.AppendLine("----------------------------");
            }
            Console.WriteLine(sb.ToString());
        }
        public static string GetTransactionHistoryString(List<Transaction> transactions)
        {
            if (transactions == null || transactions.Count == 0)
            {
                return "No transaction history found.";
            }

            string result = string.Empty;
            foreach (var transaction in transactions)
            {
                result += $"Transaction ID: {transaction.Id}\n";
                result += $"Transaction Type: {transaction.Type}\n";
                result += $"Transaction Amount: {transaction.Amount}\n";
                result += $"Transaction Date: {transaction.CreatedOn}\n";
                result += "----------------------------\n";
            }

            return result;
        }  
    }
}


