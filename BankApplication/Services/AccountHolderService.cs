using BankApplication.Common;
using BankApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BankApplication.Services
{
    internal class AccountHolderService
    {
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
                AccountHolder oldAccountHolder = GetAccountHolderByAccountNumber(accountHolder.AccountNumber);

                if (oldAccountHolder != null)
                {
                    oldAccountHolder.UserName = Utility.GetStringInput(Constants.Username, false, accountHolder.UserName);
                    oldAccountHolder.Password = Utility.GetStringInput(Constants.Password, false, accountHolder.Password);
                    oldAccountHolder.Name = Utility.GetStringInput(Constants.AccountHolderName, false, accountHolder.Name);
                    oldAccountHolder.AccountType = Utility.GetStringInput(Constants.AccountType, false, accountHolder.AccountType);

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
            try
            {
                return DataStorage.AccountHolders.FirstOrDefault(a => a.Id == accountId);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public AccountHolder GetAccountHolderByAccountNumber(string accountNumber)
        {
            try
            {
                return DataStorage.AccountHolders.Find(a => a.AccountNumber == accountNumber);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}


