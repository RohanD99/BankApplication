using BankApplication.Common;
using BankApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BankApplication.Builders;

namespace BankApplication.Services
{
    internal class TransactionService
    {

        public void AddTransactionToDataStorage(Transaction transaction)
        {
            try
            {
                DataStorage.Transactions.Add(transaction);
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }

        public string GetTransactionId(AccountHolder account)
        {
            try
            {
                return Utility.GenerateTransactionId(account.Id, account.AccountNumber);
            }
            catch (Exception ex)
            {
                return string.Empty; 
            }
        }

        public Response<string> GetTransactionHistory(AccountHolder account, string accountNumber = null)
        {
            Response<string> response = new Response<string>();
            StringBuilder sb = new StringBuilder();

            try
            {
                List<Transaction> transactions;

                if (!string.IsNullOrEmpty(accountNumber))
                {
                    //from accountholder's view
                    AccountHolder accountToShowTransactions = DataStorage.AccountHolders.Find(a => a.AccountNumber == accountNumber);
                    if (accountToShowTransactions == null)
                    {
                        response.IsSuccess = false;
                        response.Message = Constants.AccountNotFound;
                        return response;
                    }

                    transactions = DataStorage.Transactions
                        .Where(t => t.SrcAccount == accountToShowTransactions.AccountNumber || t.DstAccount == accountToShowTransactions.AccountNumber)
                        .ToList();
                }
                else
                {
                    transactions = DataStorage.Transactions
                        .Where(t => t.SrcAccount == account.AccountNumber || t.DstAccount == account.AccountNumber)
                        .ToList();
                }

                //from bankstaff's view
                if (transactions.Any())
                {
                    response.IsSuccess = true;
                    response.Message = Constants.TransactionSuccess;

                    if (accountNumber != null)
                    {
                        string transactionDetails = TransactionBuilder.GetTransactionDetails(transactions);
                        response.Data = transactionDetails;
                        sb.AppendLine(response.Data);
                    }
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
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }        
    }
}
