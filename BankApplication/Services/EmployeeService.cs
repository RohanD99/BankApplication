using BankApplication.Common;
using BankApplication.Models;
using System.Linq;
using System;
using static BankApplication.Common.Enums;

namespace BankApplication.Services
{
    internal class EmployeeService
    {      
        static Response<string> Response = new Response<string>();
        static AccountHolderService AccountHolderService = new AccountHolderService();
        static BankService BankService = new BankService();
        public Response<string> Create(Employee employee)
        {
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
                Console.Write("Enter Account ID to update account holder: ");
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
                        response.Message = "Transaction history retrieved successfully.";
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
            Utility.GetStringInput("Enter BankID:", true);
            string bankId = Console.ReadLine();
            Bank selectedBank = DataStorage.Banks.FirstOrDefault(b => b.Id == bankId);

            if (selectedBank == null)
            {
                Console.WriteLine("Bank not found. Transfer failed.");
                return;
            }

            Utility.GetStringInput("Enter the destination account number: ", true);
            string destinationAccountNumber = Console.ReadLine();
            AccountHolder destinationAccount = DataStorage.Accounts.FirstOrDefault(a => a.AccountNumber == destinationAccountNumber);

            if (destinationAccount == null)
            {
                Console.WriteLine("Destination account not found. Transfer failed.");
                return;
            }

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
                Console.WriteLine("Invalid transfer type. Transfer failed.");
                return;
            }

            Utility.GetStringInput("Enter the amount to transfer: ", true);
            decimal transferAmount = Convert.ToDecimal(Console.ReadLine());

            Response<string> transferResponse = BankService.TransferFunds(loggedInAccount, destinationAccount, transferAmount, transferType);

            if (transferResponse.IsSuccess)
            {
                Console.WriteLine(transferResponse.Message);
                Console.WriteLine($"New balance: {loggedInAccount.Balance}");
            }
            else
            {
                Console.WriteLine(transferResponse.Message);
            }
        }
    }
}

     


