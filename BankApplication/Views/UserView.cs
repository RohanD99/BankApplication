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
        private Action<string> WriteLineDelegate;
        public UserView()
        {
            WriteLineDelegate = Utility.GetConsoleWriteLineDelegate();
        }
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
                        WriteLineDelegate("Enter the amount to deposit: ");
                        decimal depositAmount = Convert.ToDecimal(Console.ReadLine());
                        Response<string> depositResponse = BankService.Deposit(account, depositAmount);
                        WriteLineDelegate(depositResponse.Message);
                        break;

                    case UserAccountOption.Withdraw:
                        WriteLineDelegate("Enter the amount to withdraw: ");
                        decimal withdrawAmount = Convert.ToDecimal(Console.ReadLine());
                        Response<string> WithdrawResponse = BankService.Withdraw(account, withdrawAmount);
                        WriteLineDelegate(WithdrawResponse.Message);
                        break;

                    case UserAccountOption.Transfer:
                        WriteLineDelegate("Enter BankID:");
                        string bankId = Console.ReadLine();
                        Bank selectedBank = DataStorage.Banks.FirstOrDefault(b => b.Id == bankId);

                        if (selectedBank == null)
                        {
                            WriteLineDelegate("Bank not found. Transfer failed.");
                            break;
                        }

                        WriteLineDelegate("Enter the destination account number: ");
                        string destinationAccountNumber = Console.ReadLine();
                        AccountHolder destinationAccount = DataStorage.Accounts.FirstOrDefault(a => a.AccountNumber == destinationAccountNumber);

                        if (destinationAccount == null)
                        {
                            WriteLineDelegate("Destination account not found. Transfer failed.");
                            break;
                        }

                        WriteLineDelegate("Enter the transfer type (0 for IMPS, 1 for RTGS): ");
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
                            WriteLineDelegate("Invalid transfer type. Transfer failed.");
                            break;
                        }

                        WriteLineDelegate("Enter the amount to transfer: ");
                        decimal transferAmount = Convert.ToDecimal(Console.ReadLine());

                        Response<string> transferResponse = BankService.TransferFunds(account, destinationAccount, transferAmount, transferType);

                        if (transferResponse.IsSuccess)
                        {
                            WriteLineDelegate(transferResponse.Message);
                            WriteLineDelegate($"New balance: {account.Balance}");
                        }
                        else
                        {
                            WriteLineDelegate(transferResponse.Message);
                        }
                        break;

                    case UserAccountOption.CheckBalance:
                        Response<string> BalanceResponse = BankService.CheckBalance(account);
                        WriteLineDelegate($"Your account balance: {BalanceResponse.Data}");
                        break;

                    case UserAccountOption.Transactions:
                        Response<string> TransactionHistoryResponse = BankService.ViewTransactionHistory(account);
                        WriteLineDelegate(TransactionHistoryResponse.Message);
                        WriteLineDelegate(TransactionHistoryResponse.Data);
                        break;

                    case UserAccountOption.Logout:
                        break;

                    default:
                        WriteLineDelegate("Please enter a valid input.");
                        break;
                }
            } while (option != UserAccountOption.Logout);
        }
    }
}

