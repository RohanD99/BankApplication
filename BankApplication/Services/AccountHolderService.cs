using BankApplication.Common;
using BankApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BankApplication.Services
{
    internal class AccountHolderService
    {
        public User LoggedInUser { get; set; }

        public AccountHolderService() 
        {
            this.LoggedInUser = new User();
        }

        public Response<string> Create(AccountHolder accountHolder)
        {
            Response<string> response = new Response<string>();
            try
            {
                accountHolder.Id = Utility.GenerateAccountId(accountHolder.Name);
                accountHolder.AccountNumber = Utility.GenerateAccountNumber();

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

        public Response<AccountHolder> Update(AccountHolder accountHolder)
        {
            Response<AccountHolder> response = new Response<AccountHolder>();

            try
            {
                AccountHolder oldAccountHolder = GetAccountHolderById(accountHolder.Id);

                if (oldAccountHolder != null)
                {
                    oldAccountHolder.UserName = string.IsNullOrEmpty(oldAccountHolder.UserName) ? accountHolder.UserName : oldAccountHolder.UserName;
                    oldAccountHolder.Password = string.IsNullOrEmpty(oldAccountHolder.Password) ? accountHolder.Password : oldAccountHolder.Password;
                    oldAccountHolder.Name = string.IsNullOrEmpty(oldAccountHolder.Name) ? accountHolder.Name : oldAccountHolder.Name;
                    oldAccountHolder.AccountType = string.IsNullOrEmpty(oldAccountHolder.AccountType) ? accountHolder.AccountType : oldAccountHolder.AccountType;
                    oldAccountHolder.ModifiedBy = this.LoggedInUser.Id;
                    oldAccountHolder.ModifiedOn = DateTime.Now;

                    int index = DataStorage.AccountHolders.IndexOf(oldAccountHolder);
                    if (index >= 0)
                    {
                        DataStorage.AccountHolders[index] = oldAccountHolder;
                    }

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
                if (string.IsNullOrEmpty(bankID))
                {
                    response.IsSuccess = false;
                    response.Message = Constants.EmptyBankInput;
                }
                else
                {
                    List<AccountHolder> accountHolders = DataStorage.AccountHolders.Where(a => a.BankId == bankID).ToList();
                    response.Data = accountHolders;
                    response.IsSuccess = true;
                }
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
            try
            {
                return DataStorage.AccountHolders.Find(a => a.Id == accountId);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}


