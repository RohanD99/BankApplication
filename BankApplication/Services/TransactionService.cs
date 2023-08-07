using BankApplication.Common;
using BankApplication.Models;
using BankApplication.Views;
using System;
using System.Linq;
using System.Text;
using static BankApplication.Common.Enums;

namespace BankApplication.Services
{
    internal class TransactionService
    {
        private Transaction transaction;
        public Response<string> PerformDepositTransaction(AccountHolder accountHolder, decimal amount)
        {
            Response<string> response = new Response<string>();

            try
            {
                string transactionId = Utility.GenerateTransactionId(accountHolder.Id, accountHolder.AccountNumber);

                transaction = new Transaction
                {
                    Id = transactionId,
                    SrcAccount = accountHolder.AccountNumber,
                    Type = Constants.Deposited,
                    Amount = amount,
                    CreatedBy = accountHolder.CreatedBy,
                    CreatedOn = DateTime.Now
                };

                DataStorage.Transactions.Add(transaction);

                response.IsSuccess = true;
                response.Message = Constants.DepositSuccess;
                response.Data = accountHolder.Balance.ToString();
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public Response<string> PerformWithdrawTransaction(AccountHolder account, decimal amount)
        {
            Response<string> response = new Response<string>();

            try
            {               
                string transactionId = Utility.GenerateTransactionId(account.Id, account.AccountNumber);

                transaction = new Transaction
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
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public Response<string> PerformTransferFundsTransaction(AccountHolder sourceAccount, string destinationAccountNumber, decimal amount, TransferOptions transferType)
        {
            Response<string> response = new Response<string>();
            try
            {
                AccountHolder destinationAccount = DataStorage.AccountHolders.FirstOrDefault(a => a.AccountNumber == destinationAccountNumber);
                decimal charge = 0;
                decimal transferAmount = amount + (amount * charge);
                // Create and store source transaction
                Transaction sourceTransaction = new Transaction
                {
                    Id = Utility.GenerateTransactionId(sourceAccount.Id, sourceAccount.AccountNumber),
                    SrcAccount = sourceAccount.AccountNumber,
                    DstAccount = destinationAccountNumber,
                    Type = Constants.TransferFunds,
                    Amount = -transferAmount,
                    CreatedBy = sourceAccount.Name,
                    CreatedOn = DateTime.Now
                };

                // Create and store destination transaction
                Transaction destinationTransaction = new Transaction
                {
                    Id = Utility.GenerateTransactionId(destinationAccount.Id, destinationAccountNumber),
                    SrcAccount = sourceAccount.AccountNumber,
                    DstAccount = destinationAccountNumber,
                    Type = Constants.TransferFunds,
                    Amount = amount,
                    CreatedBy = destinationAccount.Name,
                    CreatedOn = DateTime.Now
                };

                DataStorage.Transactions.Add(sourceTransaction);
                DataStorage.Transactions.Add(destinationTransaction);

                response.IsSuccess = true;
                response.Message = Constants.TransferFundsSuccess;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public Response<string> ViewAccountTransactionHistory(AccountHolder account)
        {
            Response<string> response = new Response<string>();
            StringBuilder sb = new StringBuilder();
            try
            {
                //Using LINQ 
                var transactions = DataStorage.Transactions
                    .Where(t => t.SrcAccount == account.AccountNumber || t.DstAccount == account.AccountNumber)
                    .ToList();

                response.Message = string.Format(Constants.ViewTransactionHistory, account.AccountNumber);
                response.Data = EmployeeView.GetTransactionHistoryString(transactions);
                response.IsSuccess = true;

                sb.AppendLine(response.Data);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                response.Data = string.Empty;
            }

            return response;
        }

        public Response<string> RevertTransaction(string transactionId)
        {
            Response<string> response = new Response<string>();

            try
            {
                Transaction transactionToRevert = DataStorage.Transactions.Find(t => t.Id == transactionId);

                if (transactionToRevert != null)
                {
                    AccountHolder account = DataStorage.AccountHolders.Find(a => a.AccountNumber == transactionToRevert.SrcAccount);

                    if (account != null)
                    {
                        if (account.Balance < transactionToRevert.Amount)
                        {
                            response.IsSuccess = false;
                            response.Message = Constants.TransactionFailure;
                        }
                        else
                        {
                            // Revert the transaction by adding the transaction amount back to the account balance
                            account.Balance += transactionToRevert.Amount;
                            response.IsSuccess = true;
                            response.Message = Constants.TransactionRevert;
                        }
                    }
                    else
                    {
                        //account was not found
                        response.IsSuccess = false;
                        response.Message = Constants.AccountNotFound;
                    }
                }
                else
                {
                    //The transaction with the given ID was not found
                    response.IsSuccess = false;
                    response.Message = Constants.TransactionFailure;
                }
            }
            catch (Exception ex)
            {
                // An error occurred during the process
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public Response<string> ShowAccountTransactionHistory(string accountNumber)
        {
            TransactionService transactionService = new TransactionService();
            Response<string> response = new Response<string>();
            try
            {
                AccountHolder accountToShowTransactions = DataStorage.AccountHolders.Find(a => a.AccountNumber == accountNumber);
                if (accountToShowTransactions != null)
                {
                    Response<string> historyResponse = transactionService.ViewAccountTransactionHistory(accountToShowTransactions);
                    if (historyResponse.IsSuccess)
                    {
                        response.IsSuccess = true;
                        response.Message = Constants.TransactionSuccess;
                        response.Data = historyResponse.Data;
                    }
                    else
                    {
                        response.IsSuccess = false;
                        response.Message = historyResponse.Message;
                    }
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = Constants.AccountNotFound;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }
    

         public Response<string> ViewTransactionHistory(AccountHolder account)
         {
            Response<string> response = new Response<string>();

            try
            {
                var transactions = DataStorage.Transactions
                    .Where(t => t.SrcAccount == account.AccountNumber || t.DstAccount == account.AccountNumber)
                    .ToList();

                if (transactions.Any())
                {
                    EmployeeView.PrintTransactionDetails(transactions);
                    response.IsSuccess = true;
                    response.Message = Constants.TransactionSuccess;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = Constants.TransactionFailure;
                    response.Data = string.Empty;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
         }
    }
}
