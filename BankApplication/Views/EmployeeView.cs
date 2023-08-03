﻿using BankApplication.Common;
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
                        Console.Write("Enter the amount to deposit: ");
                        string depositAmountInput = Console.ReadLine();
                        if (!decimal.TryParse(depositAmountInput, out decimal depositAmount))
                        {
                            Console.WriteLine("Invalid amount. Please enter a valid decimal value.");
                            break;
                        }

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
                        BankView BankView = new BankView(); 
                        BankView.TransferFunds(loggedInAccount);
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

        public static void PrintAccountDetails(IEnumerable<AccountHolder> Accounts)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var account in Accounts)
            {
                sb.AppendLine($"Account Number: {account.AccountNumber}");
                sb.AppendLine($"Constants.AccountHolderName: {account.Name}");
                sb.AppendLine($"Balance: {account.Balance}");
                sb.AppendLine($"Account Type: {account.AccountType}");
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


