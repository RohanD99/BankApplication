using BankApplication.Common;
using BankApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BankApplication.Services
{
    internal class TransactionService
    {

        public string AddTransactionAndGetId(Transaction transaction, AccountHolder account)
        {
            try
            {
                DataStorage.Transactions.Add(transaction);
                return Utility.GenerateTransactionId(account.Id, account.AccountNumber);
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public Response<List<Transaction>> GetTransactionHistory(AccountHolder account, string bankId, string accountNumber)
        {
            Response<List<Transaction>> response = new Response<List<Transaction>>();

            try
            {
                if (string.IsNullOrEmpty(bankId) || string.IsNullOrEmpty(accountNumber))
                {
                    response.IsSuccess = false;
                    response.Message = Constants.InvalidTransactionInput;
                    return response;
                }

                List<Transaction> transactions = DataStorage.Transactions.Where(t => t.SrcAccount == accountNumber || t.DstAccount == accountNumber).ToList();

                if (transactions.Any())
                {
                    response.IsSuccess = true;
                    response.Message = Constants.TransactionSuccess;
                    response.Data = transactions;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = Constants.TransactionFailure;
                    response.Data = new List<Transaction>();
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                response.Data = new List<Transaction>();
            }

            return response;
        }
    }
}
