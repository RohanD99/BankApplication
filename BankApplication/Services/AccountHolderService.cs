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
                AccountHolder accountHolderToUpdate = GetAccountHolderById(accountHolder.Id);

                if (accountHolderToUpdate != null)
                {
                    response.IsSuccess = true;
                    response.Message = Constants.AccountUpdated;
                    response.Data = accountHolderToUpdate;
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

        public Response<string> ShowAllAccounts(Employee employee)
        {
            Response<string> response = new Response<string>();
            StringBuilder sb = new StringBuilder();

            try
            {
                if (employee == null)
                {
                    response.IsSuccess = false;
                    response.Message = Constants.EmployeeFailure;
                    return response;
                }

                var accountsOfSelectedBank = DataStorage.Accounts.Where(a => a.BankId == employee.BankId).ToList();
                if (accountsOfSelectedBank.Any())
                {
                    EmployeeView.PrintAccountDetails(accountsOfSelectedBank);
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

        public static Response<string> AddAcceptedCurrency(string currencyCode, decimal exchangeRate, Employee loggedInEmployee)
        {
            Response<string> response = new Response<string>();

            try
            {
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
                else
                {
                    Constants.acceptedCurrencies.Add(currencyCode, exchangeRate);

                    response.IsSuccess = true;
                    response.Message = Constants.NewCurrency;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public Response<string> AddServiceChargeForSameBankAccount(float rtgsCharge, float impsCharge, Employee loggedInEmployee)
        {
            Response<string> response = new Response<string>();

            try
            {
                Bank selectedBank = DataStorage.Banks.FirstOrDefault(b => b.Id == loggedInEmployee.BankId);

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

                response.IsSuccess = true;
                response.Message = Constants.ServiceChargesUpdated;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public Response<string> AddServiceChargeForOtherBankAccount(float rtgsCharge, float impsCharge, Employee loggedInEmployee)
        {
            Response<string> response = new Response<string>();

            try
            {
                Bank selectedBank = DataStorage.Banks.FirstOrDefault(b => b.Id == loggedInEmployee.BankId);

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

                response.IsSuccess = true;
                response.Message = Constants.ServiceChargesUpdated;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public Response<string> ViewAccountTransactionHistory(AccountHolder account)
        {
            Response<string> response = new Response<string>();
            StringBuilder sb = new StringBuilder();
            try
            {
                //Using LINQ 
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
                        if (account.Balance < transactionToRevert.Amount)
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
                EmployeeView.UserAccountMenu(loggedInAccountHolder);
            }
        }

        public AccountHolder GetAccountHolderById(string accountId)
        {
            return DataStorage.Accounts.FirstOrDefault(a => a.Id == accountId);
        }
    }
}


