using BankApplication.Common;
using BankApplication.Models;
using BankApplication.Services;
using System;
using System.Collections.Generic;
using static BankApplication.Common.Enums;

namespace BankApplication.Views
{
    internal class AccountHolderView
    {
        BankService BankService;

        public User LoggedInUser { get; set; }

        public AccountHolderView(User loggedInUser)
        {
            this.LoggedInUser = loggedInUser;
            this.BankService = new BankService();
        }

        public void Initiate()
        {
            UserAccountOption option;
            do
            {
                Utility.GenerateOptions(Constants.UserAccountOption);
                option = (UserAccountOption)Convert.ToInt32(Console.ReadLine());
                switch (option)
                {
                    case UserAccountOption.Deposit:
                        this.Deposit();
                        break;

                    case UserAccountOption.Withdraw:
                        this.Withdraw();
                        break;

                    case UserAccountOption.Transfer:
                        this.Transfer();
                        break;

                    case UserAccountOption.CheckBalance:
                        this.CheckBalance();
                        break;

                    case UserAccountOption.Transactions:
                        if (LoggedInUser is AccountHolder transactionAccountHolder)
                            this.GetTransaction(transactionAccountHolder);
                        break;

                    case UserAccountOption.Logout:
                        break;

                    default:
                        Console.WriteLine("Please enter a valid input.");
                        break;
                }
            } while (option != UserAccountOption.Logout);
        }

        public void Transfer()
        {
            string srcAccHolderID = Utility.GetStringInput("Enter source account holder ID: ", true);
            string dstAccHolderID = Utility.GetStringInput("Enter destination account holder ID: ", true);
            decimal transferAmount = Utility.GetDecimalInput("Enter the amount to transfer: ", true);
            string bankID = Utility.GetStringInput("Enter Bank ID", true);

            Console.WriteLine("Choose transfer type:\n1. IMPS\n2. RTGS");
            decimal transferTypeChoice = Utility.GetDecimalInput("Enter your choice: ", true);

            TransferOptions transferType;
            if (transferTypeChoice == 2)
                transferType = TransferOptions.RTGS;
            else
                transferType = TransferOptions.IMPS;
            
            Response<string> transferResponse = BankService.TransferFunds(bankID, srcAccHolderID, dstAccHolderID, transferAmount, transferType);
            Console.WriteLine(transferResponse.Message);
        }

        public void GetTransaction(AccountHolder accountHolder)
        {
            string accountNumber = accountHolder.AccountNumber;
            string bankId = accountHolder.BankId;

            TransactionService transactionService = new TransactionService();
            Response<List<Transaction>> transactionHistoryResponse = transactionService.GetTransactionHistory(bankId, accountNumber);

            if (transactionHistoryResponse.IsSuccess)
            {
                Console.WriteLine(transactionHistoryResponse.Message);
                string transactionDetails = Utility.GetTransactionDetails(transactionHistoryResponse.Data);
                Console.WriteLine(transactionDetails);
            }
            else
            {
                Console.WriteLine(transactionHistoryResponse.Message);
            }
        }

        public void Deposit()
        {
            string accountHolderID = Utility.GetStringInput("Enter your account holder ID: ", true);
            decimal depositAmount = Utility.GetDecimalInput("Enter amount to deposit", true);
            Response<string> depositResponse = BankService.Deposit(accountHolderID, depositAmount);
            Console.WriteLine(depositResponse.Message);
        }

        public void Withdraw()
        {
            string accountID = Utility.GetStringInput("Enter your account holder ID: ", true);
            decimal withdrawAmount = Utility.GetDecimalInput("Enter the amount to withdraw: ", true);
            Response<string> withdrawResponse = BankService.Withdraw(accountID, withdrawAmount);
            Console.WriteLine(withdrawResponse.Message);
        }

        public void CheckBalance()
        {
            if (LoggedInUser is AccountHolder checkBalanceAccountHolder)
            {
                Response<string> checkBalanceResponse = BankService.CheckBalance(checkBalanceAccountHolder.Id);
                Console.WriteLine($"Your account balance: {checkBalanceResponse.Data}");
            }
        }
    }
}
