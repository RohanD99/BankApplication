﻿using BankApplication.Common;
using BankApplication.Models;
using BankApplication.Views;
using System;
using System.Linq;
using System.Text;

namespace BankApplication.Services
{
    internal class AccountHolderService
    {     
        public Response<string> Create()
        {
            BankView BankView = new BankView();
            Response<string> response = new Response<string>();
            try
            {
                Employee employee = DataStorage.Employees.FirstOrDefault(emp => emp.Type == Enums.UserType.Employee);

                if (employee != null)
                {
                    BankView.AddUser(employee);
                    response.IsSuccess = true;
                    response.Message = Constants.AccountSuccess;
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

        public Response<User> Update(User user)
        {
            Response<User> response = new Response<User>();
            Employee employee = new Employee();

            if (user is AccountHolder accountHolder)
            {
                try
                {
                    accountHolder.UserName = Utility.GetUpdatedValue(accountHolder.UserName, Constants.Username);
                    accountHolder.Password = Utility.GetUpdatedValue(accountHolder.Password, Constants.Password);
                    accountHolder.Name = Utility.GetUpdatedValue(accountHolder.Name, Constants.AccountHolderName);
                    accountHolder.AccountType = Utility.GetUpdatedValue(accountHolder.AccountType, Constants.AccountType);
                    accountHolder.ModifiedBy = Utility.GetUpdatedValue(employee.Designation, Constants.ModifiedBy);

                    response.IsSuccess = true;
                    response.Message = Constants.AccountUpdated;
                    response.Data = accountHolder;
                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.Message = ex.Message;
                }
            }
            else
            {
                response.IsSuccess = false;
                response.Message = Constants.AccountUpdateFailure;
            }

            return response;
        }

        public Response<string> Delete(string userId)
        {
            Response<string> response = new Response<string>();
            try
            {
                User userToDelete = DataStorage.Accounts.FirstOrDefault(e => e.Id == userId);

                if (userToDelete != null)
                {
                    DataStorage.Accounts = DataStorage.Accounts.Where(e => e.Id != userId).ToList();
                    response.IsSuccess = true;
                    response.Message = Constants.AccountDeleted;
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

        public Response<string> ShowAllAccounts()
        {
            Response<string> response = new Response<string>();
            StringBuilder sb = new StringBuilder();

            try
            {
                if (DataStorage.Accounts.Any())
                {
                    EmployeeView.PrintAccountDetails(DataStorage.Accounts);
                    response.IsSuccess = true;
                    response.Message = Constants.ShowAllAccounts;
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

        public static Response<string> AddAcceptedCurrency(string bankIDForCurrency, string currencyCode, decimal exchangeRate)
        {
            Response<string> response = new Response<string>();

            Bank selectedBank = DataStorage.Banks.FirstOrDefault(b => b.Id == bankIDForCurrency);

            if (selectedBank == null)
            {
                response.IsSuccess = false;
                response.Message = Constants.BankNotFound;
                return response;
            }

            if (Constants.acceptedCurrencies.ContainsKey(currencyCode))
            {
                response.IsSuccess = false;
                response.Message = Constants.CurrencyExists;
                return response;
            }

            if (exchangeRate <= 0)
            {
                response.IsSuccess = false;
                response.Message = Constants.InvalidRate;
                return response;
            }

            Constants.acceptedCurrencies.Add(currencyCode, exchangeRate);

            response.IsSuccess = true;
            response.Message = Constants.NewCurrency;
            return response;
        }


        public Response<string> AddServiceChargeForSameBankAccount(string bankId, float rtgsCharge, float impsCharge)
        {
            Response<string> response = new Response<string>();

            Bank selectedBank = DataStorage.Banks.FirstOrDefault(b => b.Id == bankId);

            if (selectedBank == null)
            {
                response.IsSuccess = false;
                response.Message = Constants.BankNotFound;
                return response;
            }

            response.Message = Constants.ServiceChargeForSameAccount;
            float previousRTGSCharge = selectedBank.RTGSforSameBank;
            float previousIMPSCharge = selectedBank.IMPSforSameBank;

            selectedBank.RTGSforSameBank = rtgsCharge;
            selectedBank.IMPSforSameBank = impsCharge;

            response.Message = Constants.ServiceChargesUpdated;
            response.IsSuccess = true;

            return response;
        }

        public Response<string> AddServiceChargeForOtherBankAccount(string bankId, float rtgsCharge, float impsCharge)
        {
            Response<string> response = new Response<string>();

            Bank selectedBank = DataStorage.Banks.FirstOrDefault(b => b.Id == bankId);

            if (selectedBank == null)
            {
                response.IsSuccess = false;
                response.Message = Constants.BankNotFound;
                return response;
            }

            response.Message = Constants.ServiceChargeForOtherAccount;
            float previousRTGSCharge = selectedBank.RTGSforOtherBank;
            float previousIMPSCharge = selectedBank.IMPSforOtherBank;

            selectedBank.RTGSforOtherBank = rtgsCharge;
            selectedBank.IMPSforOtherBank = impsCharge;

            response.Message = Constants.ServiceChargesUpdated;
            response.IsSuccess = true;

            return response;
        }

        public Response<string> ViewAccountTransactionHistory(AccountHolder account)
        {
            Response<string> response = new Response<string>();
            StringBuilder sb = new StringBuilder();
            try
            {
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
                Transaction transactionToRevert = DataStorage.Transactions.FirstOrDefault(t => t.Id == transactionId);

                if (transactionToRevert != null)
                {
                    AccountHolder account = DataStorage.Accounts.FirstOrDefault(a => a.AccountNumber == transactionToRevert.SrcAccount);

                    if (account != null)
                    {
                        if (account.Balance == null || account.Balance < transactionToRevert.Amount)
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
                        // account was not found
                        response.IsSuccess = false;
                        response.Message = Constants.AccountNotFound;
                    }
                }
                else
                {
                    // The transaction with the given ID was not found
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

        public static void LoginAsAccountHolder()
        {
            BankView BankView = new BankView();
            EmployeeView EmployeeView = new EmployeeView();
            AccountHolder loggedInAccountHolder = BankView.VerifyAccountHolderCredentials();
            StringBuilder sb = new StringBuilder();
            if (loggedInAccountHolder != null)
            {
                sb.AppendFormat("Welcome, {0}!", loggedInAccountHolder.Name);
                EmployeeView.UserAccountMenu(loggedInAccountHolder);
            }
        }
    }
}


