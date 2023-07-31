using BankApplication.Common;
using BankApplication.Models;
using BankApplication.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using static BankApplication.Common.Enums;

namespace BankApplication.Views
{
    internal class UserView
    {  
        BankService BankService = new BankService();
        public void UserAccountMenu(AccountHolder account)
        {
            UserAccountOption option;
            do
            {
                List<string> UserAccountMenuOptions = Enum.GetNames(typeof(UserAccountOption)).ToList();
                Utility.GenerateOptions(UserAccountMenuOptions);

                option = (UserAccountOption)Convert.ToInt32(Console.ReadLine());
                switch (option)
                {
                    case UserAccountOption.Deposit:
                        Console.Write("Enter the amount to deposit: ");
                        decimal depositAmount = Convert.ToDecimal(Console.ReadLine());
                        Response<string> depositResponse = BankService.Deposit(account, depositAmount);
                        Console.WriteLine(depositResponse.Message);
                        break;

                    case UserAccountOption.Withdraw:
                        Console.Write("Enter the amount to withdraw: ");
                        decimal withdrawAmount = Convert.ToDecimal(Console.ReadLine());
                        Response<string> WithdrawResponse = BankService.Withdraw(account, withdrawAmount);
                        Console.WriteLine(WithdrawResponse.Message);
                        break;

                    case UserAccountOption.Transfer:
                        Console.Write("Enter BankID:");
                        string bankId = Console.ReadLine();
                        Bank selectedBank = DataStorage.Banks.FirstOrDefault(b => b.Id == bankId);

                        if (selectedBank == null)
                        {
                            Console.WriteLine("Bank not found. Transfer failed.");
                            break;
                        }

                        Console.Write("Enter the destination account number: ");
                        string destinationAccountNumber = Console.ReadLine();
                        AccountHolder destinationAccount = DataStorage.Accounts.FirstOrDefault(a => a.AccountNumber == destinationAccountNumber);

                        if (destinationAccount == null)
                        {
                            Console.WriteLine("Destination account not found. Transfer failed.");
                            break;
                        }

                        Console.Write("Enter the transfer type (0 for IMPS, 1 for RTGS): ");
                        int transferTypeInput = Convert.ToInt32(Console.ReadLine());

                        TransferOptions transferType;
                        if (transferTypeInput == 0)
                        {
                            transferType = TransferOptions.IMPS;
                        }
                        else if (transferTypeInput == 1)
                        {
                            transferType = TransferOptions.RTGS;
                        }
                        else
                        {
                            Console.WriteLine("Invalid transfer type. Transfer failed.");
                            break;
                        }

                        Console.Write("Enter the amount to transfer: ");
                        decimal transferAmount = Convert.ToDecimal(Console.ReadLine());

                        Response<string> transferResponse = BankService.TransferFunds(account, destinationAccount, transferAmount, transferType);

                        if (transferResponse.IsSuccess)
                        {
                            Console.WriteLine(transferResponse.Message);
                            Console.WriteLine($"New balance: {account.Balance}");
                        }
                        else
                        {
                            Console.WriteLine(transferResponse.Message);
                        }
                        break;

                    case UserAccountOption.CheckBalance:
                        Response<string> BalanceResponse = BankService.CheckBalance(account);
                        Console.WriteLine($"Your account balance: {BalanceResponse.Data}");
                        break;

                    case UserAccountOption.Transactions:
                        Response<string> TransactionHistoryResponse = BankService.ViewTransactionHistory(account);
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

