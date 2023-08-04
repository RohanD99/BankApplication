using BankApplication.Common;
using BankApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BankApplication.Services
{
    internal class AccountHolderService
    {
        public Response<string> Create(AccountHolder accountHolder, Employee employee)
        {
            Response<string> response = new Response<string>();
            try
            {
                accountHolder.Id = Utility.GenerateAccountId(accountHolder.Name);
                accountHolder.AccountNumber = Utility.GenerateAccountNumber(accountHolder.Name);
                accountHolder.BankId = employee.BankId;

                DataStorage.Accounts.Add(accountHolder);

                response.IsSuccess = true;
                response.Message = Constants.AccountSuccess;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public Response<AccountHolder> Update(AccountHolder accountHolder)
        {
            Response<AccountHolder> response = new Response<AccountHolder>();

            try
            {
                AccountHolder oldAccountHolder = GetAccountHolderById(accountHolder.Id);

                if (oldAccountHolder != null)
                {
                    oldAccountHolder.UserName = accountHolder.UserName;
                    oldAccountHolder.Password = accountHolder.Password;
                    oldAccountHolder.Name = accountHolder.Name;
                    oldAccountHolder.AccountType = accountHolder.AccountType;

                    response.IsSuccess = true;
                    response.Message = Constants.AccountHolderUpdateSuccess;
                    response.Data = oldAccountHolder;
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

        public Response<string> Delete(string accountId)
        {
            Response<string> response = new Response<string>();

            try
            {
                AccountHolder accountHolderToDelete = GetAccountHolderById(accountId);

                if (accountHolderToDelete != null)
                {
                    DataStorage.Accounts.Remove(accountHolderToDelete);
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

        public Response<List<AccountHolder>> ShowAllAccounts(string bankID)
        {
            Response<List<AccountHolder>> response = new Response<List<AccountHolder>>();
            try
            {
                response.Data = string.IsNullOrEmpty(bankID) ? new List<AccountHolder>() : DataStorage.Accounts.Where(a => a.BankId == bankID).ToList();
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public AccountHolder GetAccountHolderById(string accountId)
        {
            return DataStorage.Accounts.FirstOrDefault(a => a.Id == accountId);
        }
    }
}


