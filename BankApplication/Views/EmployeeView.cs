using BankApplication.Common;
using BankApplication.Models;
using BankApplication.Services;
using System;
using System.Collections.Generic;
using static BankApplication.Common.Enums;

namespace BankApplication.Views
{
    internal class EmployeeView
    {  
        BankService BankService = new BankService();
        AccountHolderService AccountHolderService = new AccountHolderService();
        public void InitiateUserAccount(AccountHolder loggedInAccount)
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

                        AccountHolder accountHolder = AccountHolderService.GetAccountHolderById(accountHolderID);

                        if (accountHolder != null)
                        {
                            Response<string> depositResponse = BankService.Deposit(accountHolder, depositAmount);
                            Console.WriteLine(depositResponse.Message);
                        }
                        else
                            Console.WriteLine("Account holder not found.");
                        break;

                    case UserAccountOption.Withdraw:
                        Utility.GetStringInput("Enter the amount to withdraw: ", true);
                        decimal withdrawAmount = Convert.ToDecimal(Console.ReadLine());
                        Response<string> WithdrawResponse = BankService.Withdraw(loggedInAccount, withdrawAmount);
                        Console.WriteLine(WithdrawResponse.Message);
                        break;

                    case UserAccountOption.Transfer:
                        BankView bankView = new BankView();
                        bankView.GetTransferFunds(loggedInAccount);
                        break;

                    case UserAccountOption.CheckBalance:
                        Response<string> balanceResponse = BankService.CheckBalance(loggedInAccount);
                        Console.WriteLine($"Your account balance: {balanceResponse.Data}");
                        break;

                    case UserAccountOption.Transactions:
                        TransactionService transactionService = new TransactionService();
                        Response<List<Transaction>> transactionHistoryResponse = transactionService.GetTransactionHistory(loggedInAccount, loggedInAccount.BankId, loggedInAccount.AccountNumber);

                        if (transactionHistoryResponse.IsSuccess)
                        {
                            Console.WriteLine(transactionHistoryResponse.Message);
                            Utility.GetTransactionDetails(transactionHistoryResponse.Data);
                        }
                        else
                            Console.WriteLine(transactionHistoryResponse.Message);
                        break;

                    case UserAccountOption.Logout:
                        break;

                    default:
                        Console.WriteLine("Please enter a valid input.");
                        break;
                }
            } while (option != UserAccountOption.Logout);
        }

        public void AddEmployee()
        {
            try
            {
                Employee employee = new Employee()
                {
                    Id = Utility.GenerateEmployeeID(),
                    Name = Utility.GetStringInput("Enter Employee Name", true),
                    UserName = Utility.GetStringInput("Enter UserName", true),
                    Password = Utility.GetStringInput("Enter Password", true),
                    Email = Utility.GetStringInput("Enter Email", true),
                    Designation = Utility.GetStringInput("Enter Designation", true),
                    Type = Enums.UserType.Employee
                };

                DataStorage.Employees.Add(employee);
                Console.WriteLine("Employee added successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
