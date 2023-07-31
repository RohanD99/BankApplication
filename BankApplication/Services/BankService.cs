using BankApplication.Common;
using BankApplication.Models;
using System;
using System.Linq;
using static BankApplication.Common.Enums;

namespace BankApplication.Services
{
    internal class BankService
    {
        public Response<string> CreateBank(Bank bank)
        {
            Response<string> response = new Response<string>();
            try
            {
                bank.Id = Utility.GenerateBankId(bank.Name);
                bank.CreatedOn = DateTime.Now;
                bank.ModifiedOn = DateTime.Now;
                DataStorage.Banks.Add(bank);

                response.IsSuccess = true;
                response.Message = Constants.BankCreation;
                response.Data = bank.Id;
            }
            catch
            {
                response.IsSuccess = false;
                response.Message = Constants.BankFailure;
            }

            return response;
        }

        public Response<string> Deposit(AccountHolder account, decimal amount)
        {
            Response<string> response = new Response<string>();
            if (amount <= 0)
            {
                response.IsSuccess = false;
               
                response.Message = Constants.InvalidAmount;
                return response;
            }

            account.Balance += amount;

            string transactionId = Utility.GenerateTransactionId(account.Id, account.AccountNumber);

            Transaction transaction = new Transaction
            {
                Id = transactionId,
                SrcAccount = account.AccountNumber,
                Type = Constants.Deposit,
                Amount = amount,
                CreatedBy = account.CreatedBy,
                CreatedOn = DateTime.Now
            };

            DataStorage.Transactions.Add(transaction);

            response.IsSuccess = true;
            response.Message = Constants.DepositSuccess;
            response.Data = account.Balance.ToString();
            return response;
        }
    
        public Response<string> Withdraw(AccountHolder account, decimal amount)
        {
            Response<string> response = new Response<string>();
            if (amount <= 0)
            {
                response.IsSuccess = false;
                response.Message = Constants.InvalidAmount;
                return response;
            }

            if (amount > account.Balance)
            {
                response.IsSuccess = false;
                response.Message = Constants.InsufficientFunds;
                return response;
            }

            account.Balance -= amount;
            string transactionId = Utility.GenerateTransactionId(account.Id, account.AccountNumber);

            Transaction transaction = new Transaction
            {               
                Id = transactionId,
                SrcAccount = account.AccountNumber,
                DstAccount = account.AccountNumber,
                Type = Constants.Withdrawal,
                Amount = -amount,
                CreatedBy = account.Name,
                CreatedOn = DateTime.Now
            };

            DataStorage.Transactions.Add(transaction);

            response.IsSuccess = true;
            response.Message = Constants.WithdrawalSuccess;
            response.Data = account.Balance.ToString();
            return response;
        }


        public Response<string> TransferFunds(AccountHolder sourceAccount, AccountHolder destinationAccount, decimal amount, TransferOptions transferType)
        {
            Response<string> response = new Response<string>();
            decimal charge = 0;

            if (transferType == TransferOptions.IMPS)
            {
                charge = 0.08m;
            }
            else if (transferType == TransferOptions.RTGS)
            {
                charge = 0.05m;
            }
            else
            {
                response.IsSuccess = false;
                response.Message = Constants.InvalidType;
                return response;
            }

            decimal transferAmount = amount + (amount * charge);
            if (transferAmount <= 0)
            {
                response.IsSuccess = false;
                response.Message = Constants.InvalidAmount;
                return response;
            }

            if (transferAmount > sourceAccount.Balance)
            {
                response.IsSuccess = false;
                response.Message = Constants.InsufficientFunds;
                return response;
            }

            sourceAccount.Balance -= transferAmount;
            destinationAccount.Balance += amount;

            // Create and store source transaction
            Transaction sourceTransaction = new Transaction
            {
                Id = Utility.GenerateTransactionId(sourceAccount.Id, sourceAccount.AccountNumber),
                SrcAccount = sourceAccount.AccountNumber,
                DstAccount = destinationAccount.AccountNumber,
                Type = Constants.TransferFunds,
                Amount = -transferAmount,
                CreatedBy = sourceAccount.Name,
                CreatedOn = DateTime.Now
            };

            // Create and store destination transaction
            Transaction destinationTransaction = new Transaction
            {
                Id = Utility.GenerateTransactionId(destinationAccount.Id, destinationAccount.AccountNumber),
                SrcAccount = sourceAccount.AccountNumber,
                DstAccount = destinationAccount.AccountNumber,
                Type = Constants.TransferFunds,
                Amount = amount,
                CreatedBy = destinationAccount.Name,
                CreatedOn = DateTime.Now
            };

            DataStorage.Transactions.Add(sourceTransaction);
            DataStorage.Transactions.Add(destinationTransaction);

            response.IsSuccess = true;
            response.Message = Constants.TransferFundsSuccess;
            return response;
        }

        public Response<string> CheckBalance(AccountHolder account)
        {
            Response<string> response = new Response<string>();
            response.IsSuccess = true;
            response.Data = account.Balance.ToString();
            return response;
        }

        public Response<string> ViewTransactionHistory(AccountHolder account)
        {
            Response<string> response = new Response<string>();
            var transactions = DataStorage.Transactions
                .Where(t => t.SrcAccount == account.AccountNumber || t.DstAccount == account.AccountNumber)
                .ToList();

            if (transactions.Any())
            {
                Utility.PrintTransactionDetails(transactions);
                response.IsSuccess = true;
                response.Message = Constants.TransactionSuccess;
              
            }
            else
            {
                response.IsSuccess = false;
                response.Message = Constants.TransactionFailure;
                response.Data = string.Empty;
            }

            return response;
        }
    }
}
