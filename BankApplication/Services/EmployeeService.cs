using BankApplication.Common;
using BankApplication.Models;
using System.Linq;
using System;
using static BankApplication.Common.Enums;
using System.Text;

namespace BankApplication.Services
{
    internal class EmployeeService
    {            
        static AccountHolderService AccountHolderService = new AccountHolderService();
        static BankService BankService = new BankService();
        public Response<string> Create(Employee employee)
        {
            Response<string> Response = new Response<string>();
            try
            {
                employee.Id = Utility.GenerateEmployeeID();
                DataStorage.Employees.Add(employee);
                Response.IsSuccess = true;
                Response.Message = Constants.EmployeeSuccess;
                Response.Data = employee.Id;
            }
            catch
            {
                Response.IsSuccess = false;
                Response.Message = Constants.EmployeeFailure;
            }
            return Response;
        }

        public Response<string> UpdateAccountHolder()
        {
            Response<string> response = new Response<string>();
            try
            {
                Utility.GetStringInput("Enter Account ID to update account holder: ",true);
                string accountToUpdate = Console.ReadLine();
                AccountHolder EmployeeToUpdate = DataStorage.Accounts.FirstOrDefault(e => e.Id == accountToUpdate);
                if (EmployeeToUpdate != null)
                {
                    Response<User> updateResponse = AccountHolderService.Update(EmployeeToUpdate);
                    response.IsSuccess = updateResponse.IsSuccess;
                    response.Message = updateResponse.Message;
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

        public Response<string> ShowAccountTransactionHistory(string accountNumber)
        {
            Response<string> response = new Response<string>();
            try
            {
                AccountHolder accountToShowTransactions = DataStorage.Accounts.FirstOrDefault(a => a.AccountNumber == accountNumber);
                if (accountToShowTransactions != null)
                {
                    Response<string> historyResponse = AccountHolderService.ViewAccountTransactionHistory(accountToShowTransactions);
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

        public static void TransferFundsMenu(AccountHolder loggedInAccount)
        {
            StringBuilder sb = new StringBuilder();
            Utility.GetStringInput("Enter BankID:", true);
            string bankId = Console.ReadLine();
            Bank selectedBank = DataStorage.Banks.FirstOrDefault(b => b.Id == bankId);

            Utility.GetStringInput("Enter the destination account number: ", true);
            string destinationAccountNumber = Console.ReadLine();
            AccountHolder destinationAccount = DataStorage.Accounts.FirstOrDefault(a => a.AccountNumber == destinationAccountNumber);

            Utility.GetStringInput("Enter the transfer type (0 for IMPS, 1 for RTGS): ", true);
            int transferTypeInput = Convert.ToInt32(Console.ReadLine());

            TransferOptions transferType;
            if (transferTypeInput == 0)
            {
                transferType = TransferOptions.IMPS;
            }
            else if (transferTypeInput == 1)
            {
                transferType = TransferOptions.RTGS;
            }
            else
            {
                sb.AppendLine(Constants.InvalidType);
                return;
            }

            Utility.GetStringInput("Enter the amount to transfer: ", true);
            decimal transferAmount = Convert.ToDecimal(Console.ReadLine());

            Response<string> transferResponse = BankService.TransferFunds(loggedInAccount, destinationAccount, transferAmount, transferType);

            if (transferResponse.IsSuccess)
            {
                sb.AppendLine(transferResponse.Message);
                sb.AppendLine($"New balance: {loggedInAccount.Balance}");
            }
            else
            {
                sb.AppendLine(transferResponse.Message);
            }
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

     


