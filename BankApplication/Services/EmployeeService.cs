using BankApplication.Common;
using BankApplication.Models;
using System.Linq;
using System;

namespace BankApplication.Services
{
    internal class EmployeeService
    {            
        public Response<string> Create(Employee employee)
        {
            Response<string> response = new Response<string>();
            try
            {
                employee.Id = Utility.GenerateEmployeeID();
                DataStorage.Employees.Add(employee);
                response.IsSuccess = true;
                response.Message = Constants.EmployeeSuccess;
                response.Data = employee.Id;
            }
            catch
            {
                response.IsSuccess = false;
                response.Message = Constants.EmployeeFailure;
            }
            return response;
        }
     
        public Response<string> ShowAccountTransactionHistory(string accountNumber)
        {
            TransactionService transactionService = new TransactionService();
            Response<string> response = new Response<string>();
            try
            {
                AccountHolder accountToShowTransactions = DataStorage.Accounts.FirstOrDefault(a => a.AccountNumber == accountNumber);
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

        public static Employee GetEmployeeByUsernameAndPassword(string username, string password)
        {
            return DataStorage.Employees.FirstOrDefault(e => e.UserName == username && e.Password == password);
        }

        public static AccountHolder GetAccountHolderByUsernameAndPassword(string username, string password)
        {
            return DataStorage.Accounts.FirstOrDefault(a => a.UserName == username && a.Password == password);
        }
    }
}

     


