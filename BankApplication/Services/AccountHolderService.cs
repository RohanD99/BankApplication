using BankApplication.Common;
using BankApplication.Models;
using BankApplication.Views;
using System;
using System.Linq;
using System.Text;

namespace BankApplication.Services
{
    internal class AccountHolderService
    {
        static Bank Bank = new Bank();
        static Response<string> Response = new Response<string>();
        static BankView BankView = new BankView();
        static EmployeeView EmployeeView = new EmployeeView();  

        public Response<string> CreateNewAccountHolder()
        {
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
                    Utility.PrintAccountDetails(DataStorage.Accounts);
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
            try
            {
                var transactions = DataStorage.Transactions
                    .Where(t => t.SrcAccount == account.AccountNumber || t.DstAccount == account.AccountNumber)
                    .ToList();

                response.Message = string.Format(Constants.ViewTransactionHistory, account.AccountNumber);
                response.Data = Utility.GetTransactionHistoryString(transactions);
                response.IsSuccess = true;

                Console.WriteLine(response.Data);
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
                        account.Balance += transactionToRevert.Amount;
                        response.IsSuccess = true;
                        response.Message = Constants.TransactionRevert;
                    }
                    else
                    {
                        response.IsSuccess = false;
                        response.Message = Constants.AccountNotFound;
                    }
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = Constants.TransactionFailure;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public static void LoginAsAccountHolder()
        {
            AccountHolder loggedInAccountHolder = BankView.VerifyAccountHolderCredentials();

            if (loggedInAccountHolder != null)
            {
                Console.WriteLine($"Welcome, {loggedInAccountHolder.Name}!");
                EmployeeView.UserAccountMenu(loggedInAccountHolder);
            }
            else
            {
                Console.WriteLine("Account not found");
            }
        }       
    }
}


