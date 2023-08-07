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
                accountHolder.AccountNumber = Utility.GenerateAccountNumber();
                accountHolder.BankId = employee.BankId;

                DataStorage.AccountHolders.Add(accountHolder);

                response.IsSuccess = true;
                response.Message = Constants.AccountCreationSuccess;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public Response<AccountHolder> Update(AccountHolder updatedAccountHolder)
        {
            Response<AccountHolder> response = new Response<AccountHolder>();

            try
            {
                AccountHolder oldAccountHolder = GetAccountHolderById(updatedAccountHolder.Id);

                if (oldAccountHolder != null)
                {
                    oldAccountHolder.UserName = updatedAccountHolder.UserName;
                    oldAccountHolder.Password = updatedAccountHolder.Password;
                    oldAccountHolder.Name = updatedAccountHolder.Name;
                    oldAccountHolder.AccountType = updatedAccountHolder.AccountType;

                    DataStorage.AccountHolders.Remove(oldAccountHolder);
                    DataStorage.AccountHolders.Add(updatedAccountHolder);

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
                    DataStorage.AccountHolders.Remove(accountHolderToDelete);
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


        public Response<List<AccountHolder>> GetAllAccountHolders(string bankID)
        {
            Response<List<AccountHolder>> response = new Response<List<AccountHolder>>();
            try
            {
                response.Data = string.IsNullOrEmpty(bankID) ? new List<AccountHolder>() : DataStorage.AccountHolders.Where(a => a.BankId == bankID).ToList();
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
            return DataStorage.AccountHolders.FirstOrDefault(a => a.Id == accountId);
        }
    }
}


