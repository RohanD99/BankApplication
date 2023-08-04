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
            Response<string> Response = new Response<string>();
            try
            {
                accountHolder.Id = Utility.GenerateAccountId(accountHolder.Name);
                accountHolder.AccountNumber = Utility.GenerateAccountNumber(accountHolder.Name);
                accountHolder.BankId = employee.BankId;

                DataStorage.Accounts.Add(accountHolder);

                Response.IsSuccess = true;
                Response.Message = Constants.AccountSuccess;
            }
            catch (Exception ex)
            {
                Response.IsSuccess = false;
                Response.Message = ex.Message;
            }

            return Response;
        }

        public Response<AccountHolder> Update(AccountHolder accountHolder)
        {
            Response<AccountHolder> Response = new Response<AccountHolder>();

            try
            {
                AccountHolder accountHolderToUpdate = GetAccountHolderById(accountHolder.Id);

                if (accountHolderToUpdate != null)
                {
                    Response.IsSuccess = true;
                    Response.Message = Constants.AccountUpdated;
                    Response.Data = accountHolderToUpdate;
                }
                else
                {
                    Response.IsSuccess = false;
                    Response.Message = Constants.AccountNotFound;
                }
            }
            catch (Exception ex)
            {
                Response.IsSuccess = false;
                Response.Message = ex.Message;
            }

            return Response;
        }

        public Response<string> Delete(string accountId)
        {
            Response<string> Response = new Response<string>();

            try
            {
                AccountHolder accountHolderToDelete = GetAccountHolderById(accountId);

                if (accountHolderToDelete != null)
                {
                    DataStorage.Accounts.Remove(accountHolderToDelete);
                    Response.IsSuccess = true;
                    Response.Message = Constants.AccountDeleted;
                }
                else
                {
                    Response.IsSuccess = false;
                    Response.Message = Constants.AccountNotFound;
                }
            }
            catch (Exception ex)
            {
                Response.IsSuccess = false;
                Response.Message = ex.Message;
            }

            return Response;
        }

        public Response<string> ShowAllAccounts(Employee employee)
        {
            Response<string> Response = new Response<string>();
            try
            {
                if (employee == null)
                {
                    Response.IsSuccess = false;
                    Response.Message = Constants.EmployeeFailure;
                    return Response;
                }

                var accountsOfSelectedBank = DataStorage.Accounts.Where(a => a.BankId == employee.BankId).ToList();
                if (accountsOfSelectedBank.Any())
                {
                    EmployeeView.PrintAccountDetails(accountsOfSelectedBank);
                    Response.IsSuccess = true;
                    Response.Message = Constants.ShowAllAccounts;
                }
                else
                {
                    Response.IsSuccess = false;
                    Response.Message = Constants.AccountNotFound;
                }
            }
            catch (Exception ex)
            {
                Response.IsSuccess = false;
                Response.Message = ex.Message;
            }

            return Response;
        }

        public static Response<string> AddAcceptedCurrency(string currencyCode, decimal exchangeRate)
        {
            var Response = new Response<string>();

            try
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
                else
                {
                    Constants.acceptedCurrencies.Add(currencyCode, exchangeRate);

                    Response.IsSuccess = true;
                    Response.Message = Constants.NewCurrency;
                }
            }
            catch (Exception ex)
            {
                Response.IsSuccess = false;
                Response.Message = ex.Message;
            }

            return Response;
        }

        public Response<string> AddServiceChargeForSameBankAccount(float rtgsCharge, float impsCharge, Employee loggedInEmployee)
        {
            Response<string> Response = new Response<string>();

            try
            {
                Bank selectedBank = DataStorage.Banks.FirstOrDefault(b => b.Id == loggedInEmployee.BankId);
                if (selectedBank == null)
                {
                    Response.IsSuccess = false;
                    Response.Message = Constants.BankNotFound;
                    return Response;
                }

                Response.Message = Constants.ServiceChargeForSameAccount;
                float previousRTGSCharge = selectedBank.RTGSforSameBank;
                float previousIMPSCharge = selectedBank.IMPSforSameBank;

                selectedBank.RTGSforSameBank = rtgsCharge;
                selectedBank.IMPSforSameBank = impsCharge;

                Response.IsSuccess = true;
                Response.Message = Constants.ServiceChargesUpdated;
            }
            catch (Exception ex)
            {
                Response.IsSuccess = false;
                Response.Message = ex.Message;
            }

            return Response;
        }

        public Response<string> AddServiceChargeForOtherBankAccount(float rtgsCharge, float impsCharge, Employee loggedInEmployee)
        {
            Response<string> Response = new Response<string>();

            try
            {
                Bank selectedBank = DataStorage.Banks.FirstOrDefault(b => b.Id == loggedInEmployee.BankId);

                if (selectedBank == null)
                {
                    Response.IsSuccess = false;
                    Response.Message = Constants.BankNotFound;
                    return Response;
                }

                Response.Message = Constants.ServiceChargeForOtherAccount;
                float previousRTGSCharge = selectedBank.RTGSforOtherBank;
                float previousIMPSCharge = selectedBank.IMPSforOtherBank;

                selectedBank.RTGSforOtherBank = rtgsCharge;
                selectedBank.IMPSforOtherBank = impsCharge;

                Response.IsSuccess = true;
                Response.Message = Constants.ServiceChargesUpdated;
            }
            catch (Exception ex)
            {
                Response.IsSuccess = false;
                Response.Message = ex.Message;
            }

            return Response;
        }

        public Response<string> ViewAccountTransactionHistory(AccountHolder account)
        {
            Response<string> Response = new Response<string>();
            StringBuilder sb = new StringBuilder();
            try
            {
                //Using LINQ 
                var transactions = DataStorage.Transactions
                    .Where(t => t.SrcAccount == account.AccountNumber || t.DstAccount == account.AccountNumber)
                    .ToList();

                Response.Message = string.Format(Constants.ViewTransactionHistory, account.AccountNumber);
                Response.Data = EmployeeView.GetTransactionHistoryString(transactions);
                Response.IsSuccess = true;

                sb.AppendLine(Response.Data);
            }
            catch (Exception ex)
            {
                Response.IsSuccess = false;
                Response.Message = ex.Message;
                Response.Data = string.Empty;
            }

            return Response;
        }

        public Response<string> RevertTransaction(string transactionId)
        {
            Response<string> Response = new Response<string>();

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
                            Response.IsSuccess = false;
                            Response.Message = Constants.TransactionFailure;
                        }
                        else
                        {
                            // Revert the transaction by adding the transaction amount back to the account balance
                            account.Balance += transactionToRevert.Amount;
                            Response.IsSuccess = true;
                            Response.Message = Constants.TransactionRevert;
                        }
                    }
                    else
                    {
                        // account was not found
                        Response.IsSuccess = false;
                        Response.Message = Constants.AccountNotFound;
                    }
                }
                else
                {
                    // The transaction with the given ID was not found
                    Response.IsSuccess = false;
                    Response.Message = Constants.TransactionFailure;
                }
            }
            catch (Exception ex)
            {
                // An error occurred during the process
                Response.IsSuccess = false;
                Response.Message = ex.Message;
            }

            return Response;
        }

        public AccountHolder GetAccountHolderById(string accountId)
        {
            return DataStorage.Accounts.FirstOrDefault(a => a.Id == accountId);
        }
    }
}


