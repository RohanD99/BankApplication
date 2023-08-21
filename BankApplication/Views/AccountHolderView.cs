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
                        this.PerformDeposit();
                        break;

                    case UserAccountOption.Withdraw:
                        this.PerformWithdraw();
                        break;

                    case UserAccountOption.Transfer:
                        this.PerformTransfer();
                        break;

                    case UserAccountOption.CheckBalance:
                        this.PerformCheckBalance();
                        break;

                    case UserAccountOption.Transactions:
                        if (LoggedInUser is AccountHolder transactionAccountHolder)                       
                        this.ProcessTransaction(transactionAccountHolder);
                        break;

                    case UserAccountOption.Logout:
                        break;

                    default:
                        Console.WriteLine("Please enter a valid input.");
                        break;
                }
            } while (option != UserAccountOption.Logout);
        }

        private void PerformTransfer()
        {
            string sourceAccountHolderID = Utility.GetStringInput("Enter source account holder ID: ", true);
            string destinationAccountHolderID = Utility.GetStringInput("Enter destination account holder ID: ", true);
            decimal transferAmount = Utility.GetDecimalInput("Enter the amount to transfer: ", true);

            Console.WriteLine("Choose transfer type:\n1. IMPS\n2. RTGS");
            decimal transferTypeChoice = Utility.GetDecimalInput("Enter your choice: ", true);

            TransferOptions transferType;
            if (transferTypeChoice == 2)
                transferType = TransferOptions.RTGS;
            else
                transferType = TransferOptions.IMPS;
            
            Response<string> transferResponse = BankService.TransferFunds(sourceAccountHolderID, destinationAccountHolderID, transferAmount, transferType);
            Console.WriteLine(transferResponse.Message);
        }

        private void ProcessTransaction(AccountHolder accountHolder)
        {
            TransactionService transactionService = new TransactionService();
            Response<List<Transaction>> transactionHistoryResponse = transactionService.GetTransactionHistory(accountHolder.BankId, accountHolder.AccountNumber);

            if (transactionHistoryResponse.IsSuccess)
            {
                Console.WriteLine(transactionHistoryResponse.Message);
                Utility.GetTransactionDetails(transactionHistoryResponse.Data);
            }
            else
            {
                Console.WriteLine(transactionHistoryResponse.Message);
            }
        }

        public void PerformDeposit()
        {
            string accountHolderID = Utility.GetStringInput("Enter your account holder ID: ", true);
            decimal depositAmount = Utility.GetDecimalInput("Enter amount to deposit", true);
            Response<string> depositResponse = BankService.Deposit(accountHolderID, depositAmount);
            Console.WriteLine(depositResponse.Message);
        }

        public void PerformWithdraw()
        {
            string accountID = Utility.GetStringInput("Enter your account holder ID: ", true);
            decimal withdrawAmount = Utility.GetDecimalInput("Enter the amount to withdraw: ", true);
            Response<string> withdrawResponse = BankService.Withdraw(accountID, withdrawAmount);
            Console.WriteLine(withdrawResponse.Message);
        }

        public void PerformCheckBalance()
        {
            if (LoggedInUser is AccountHolder checkBalanceAccountHolder)
            {
                Response<string> checkBalanceResponse = BankService.CheckBalance(checkBalanceAccountHolder.Id);
                Console.WriteLine($"Your account balance: {checkBalanceResponse.Data}");
            }
        }
    }
}
