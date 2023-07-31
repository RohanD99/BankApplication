using BankApplication.Common;
using BankApplication.Models;
using System;
using System.Linq;
using System.Text;

namespace BankApplication.Services
{
    internal class AccountHolderService
    {
        static Bank Bank = new Bank();
        static Response<string> Response = new Response<string>();
        public Response<string> Delete(string userId)
        {
            Response<string> response = new Response<string>();
            User userToDelete = DataStorage.Accounts.FirstOrDefault(e => e.Id == userId);

            if (userToDelete != null)
            {
                DataStorage.Accounts = DataStorage.Accounts.Where(e => e.Id != userId).ToList();
                response.IsSuccess = true;
                response.Message = Constants.UserDeleted;
            }
            else
            {
                response.IsSuccess = false;
                response.Message = Constants.UserNotFound;
            }

            return response;
        }

        public Response<User> Update(User user)
        {
            Employee Employee = new Employee();
            Response<User> response = new Response<User>();
            if (user is AccountHolder accountHolder)
            {
                accountHolder.UserName = Utility.GetUpdatedValue(accountHolder.UserName, Constants.Username);
                accountHolder.Password = Utility.GetUpdatedValue(accountHolder.Password, Constants.Password);
                accountHolder.Name = Utility.GetUpdatedValue(accountHolder.Name, Constants.AccountHolderName);
                accountHolder.AccountType = Utility.GetUpdatedValue(accountHolder.AccountType, Constants.AccountType);
                accountHolder.ModifiedBy = Utility.GetUpdatedValue(Employee.Designation, Constants.ModifiedBy);

                response.IsSuccess = true;
                response.Message = Constants.UserUpdated;
                response.Data = accountHolder;
            }
            else
            {
                response.IsSuccess = false;
                response.Message = Constants.UserUpdateFailure;
            }
            return response;
        }

        public Response<string> ShowAllAccounts()
        {
            Response<string> response = new Response<string>();
            StringBuilder sb = new StringBuilder();

            if (DataStorage.Accounts.Any())
            {
                Utility.PrintAccountDetails(DataStorage.Accounts);
                response.IsSuccess = true;
                response.Message = Constants.ShowAllAccounts;
            }
            else
            {
                response.IsSuccess = false;
                response.Message = Constants.AccountNotFound;
            }

            return response;
        }

        public Response<string> AddAcceptedCurrency(string currencyCode, decimal exchangeRate)
        {

            if (Constants.acceptedCurrencies.ContainsKey(currencyCode))
            {
                Response.IsSuccess = false;
                Response.Message = Constants.CurrencyExists;
                return Response;
            }

            if (exchangeRate <= 0)
            {
                Response.IsSuccess = false;
                Response.Message = Constants.InvalidRate;
                return Response;
            }

            Constants.acceptedCurrencies.Add(currencyCode, exchangeRate);
            Response.IsSuccess = true;
            Response.Message = Constants.NewCurrency;
            return Response;
        }

        public Response<string> AddServiceChargeForSameBankAccount(float rtgsCharge, float impsCharge)
        {
            Response<string> response = new Response<string>();
            response.Message = Constants.ServiceChargeForSameAccount;
            float previousRTGSCharge = Bank.RTGSforSameBank;
            float previousIMPSCharge = Bank.IMPSforSameBank;

            Bank.RTGSforSameBank = rtgsCharge;
            Bank.IMPSforSameBank = impsCharge;
            response.Message = Constants.ServiceChargesUpdated;
            response.IsSuccess = true;

            return response;
        }


        public Response<string> AddServiceChargeForOtherBankAccount(float rtgsCharge, float impsCharge)
        {
            Response<string> response = new Response<string>();
            response.Message = Constants.ServiceChargeForOtherAccount;
            float previousRTGSCharge = Bank.RTGSforOtherBank;
            float previousIMPSCharge = Bank.IMPSforOtherBank;

            Bank.RTGSforOtherBank = rtgsCharge;
            Bank.IMPSforOtherBank = impsCharge;
            response.Message = Constants.ServiceChargesUpdated;
            response.IsSuccess = true;

            return response;
        }


        public Response<string> ViewAccountTransactionHistory(AccountHolder account)
        {
            Response<string> response = new Response<string>();
            var transactions = DataStorage.Transactions
                .Where(t => t.SrcAccount == account.AccountNumber || t.DstAccount == account.AccountNumber)
                .ToList();

            response.Message = string.Format(Constants.ViewTransactionHistory, account.AccountNumber);
            response.Data = Utility.GetTransactionHistoryString(transactions);
            response.IsSuccess = true;

            Console.WriteLine(response.Data); 

            return response;
        }


        public Response<string> RevertTransaction(string transactionId)
        {
            Response = new Response<string>();
            Transaction transactionToRevert = DataStorage.Transactions.FirstOrDefault(t => t.Id == transactionId);

            if (transactionToRevert != null)
            {
                AccountHolder account = DataStorage.Accounts.FirstOrDefault(a => a.AccountNumber == transactionToRevert.SrcAccount);
                if (account != null)
                {
                    account.Balance += transactionToRevert.Amount;
                    Response.IsSuccess = true;
                    Response.Message = Constants.TransactionRevert;
                }
                else
                {
                    Response.IsSuccess = false;
                    Response.Message = Constants.AccountNotFound;
                }
            }
            else
            {
                Response.IsSuccess = false;
                Response.Message = Constants.TransactionFailure;
            }

            return Response;
        }
    }
}


