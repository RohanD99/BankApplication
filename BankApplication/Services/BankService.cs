using BankApplication.Common;
using BankApplication.Models;
using System;
using System.Linq;
using static BankApplication.Common.Enums;

namespace BankApplication.Services
{
    internal class BankService
    {
        private TransactionService transactionService;

        public BankService()
        {
            this.transactionService = new TransactionService();
        }
        public Response<string> CreateBank(Bank bank)
        {
            Response<string> response = new Response<string>();
            try
            {
                bank.Id = Utility.GenerateBankId(bank.Name);
                bank.CreatedOn = DateTime.Now;
                bank.ModifiedOn = DateTime.Now;
                DataStorage.Banks.Add(bank);

                response.IsSuccess = true;
                response.Message = Constants.BankCreationSuccess;
                response.Data = bank.Id;
            }
            catch
            {
                response.IsSuccess = false;
                response.Message = Constants.BankCreationFailure;
            }

            return response;
        }

        public Response<string> Deposit(string accountHolderID, decimal amount)
        {
            Response<string> response = new Response<string>();

            try
            {
                AccountHolder accountHolder = DataStorage.AccountHolders.Find(account => account.Id == accountHolderID);
                if (accountHolder != null)
                {
                    accountHolder.Balance += amount;
                    response = this.transactionService.PerformDepositTransaction(accountHolder, amount);
                }
                else if (amount <= 0)
                {
                    response.IsSuccess = false;
                    response.Message = Constants.InvalidAmount;
                    return response;
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


        public Response<string> Withdraw(AccountHolder account, decimal amount)
        {
            Response<string> response = new Response<string>();

            try
            {
                if (amount <= 0)
                {
                    response.IsSuccess = false;
                    response.Message = Constants.InvalidAmount;
                    return response;
                }

                else if (amount > account.Balance)
                {
                    response.IsSuccess = false;
                    response.Message = Constants.InsufficientFunds;
                    return response;
                }
                else
                {
                    account.Balance -= amount;
                    response = this.transactionService.PerformWithdrawTransaction(account, amount);
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public Response<string> TransferFunds(AccountHolder sourceAccount, string destinationAccountNumber, decimal amount, TransferOptions transferType)
        {
            Response<string> response = new Response<string>();

            try
            {
                AccountHolder destinationAccount = DataStorage.AccountHolders.FirstOrDefault(a => a.AccountNumber == destinationAccountNumber);
                if (destinationAccount == null)
                {
                    response.IsSuccess = false;
                    response.Message = Constants.AccountNotFound;
                    return response;
                }

                decimal charge = 0;
                if (transferType == TransferOptions.IMPS)
                {
                    charge = 0.08m;
                }
                else if (transferType == TransferOptions.RTGS)
                {
                    charge = 0.05m;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = Constants.InvalidType;
                    return response;
                }

                decimal transferAmount = amount + (amount * charge);
                if (transferAmount <= 0)
                {
                    response.IsSuccess = false;
                    response.Message = Constants.InvalidAmount;
                    return response;
                }

                else if (transferAmount > sourceAccount.Balance)
                {
                    response.IsSuccess = false;
                    response.Message = Constants.InsufficientFunds;
                    return response;
                }
                else
                {
                    sourceAccount.Balance -= transferAmount;
                    destinationAccount.Balance += amount;
                    response = this.transactionService.PerformTransferFundsTransaction(sourceAccount, destinationAccountNumber, amount, transferType);
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public Response<string> CheckBalance(AccountHolder account)
        {
            Response<string> response = new Response<string>();

            try
            {
                response.IsSuccess = true;
                response.Data = account.Balance.ToString();
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public static Response<string> AddAcceptedCurrency(string currencyCode, decimal exchangeRate)
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

        public Response<string> UpdateServiceCharges(float rtgsCharge, float impsCharge, string bankID, bool isSameBankAccount)
        {
            Response<string> response = new Response<string>();

            try
            {
                Bank bank = DataStorage.Banks.FirstOrDefault(b => b.Id == bankID);
                if (bank == null)
                {
                    response.IsSuccess = false;
                    response.Message = Constants.BankNotFound;
                    return response;
                }

                if (isSameBankAccount)
                {
                    response.Message = Constants.ServiceChargeForSameAccount;
                    bank.RTGSforSameBank = rtgsCharge;
                    bank.IMPSforSameBank = impsCharge;
                }
                else
                {
                    response.Message = Constants.ServiceChargeForOtherAccount;
                    bank.RTGSforOtherBank = rtgsCharge;
                    bank.IMPSforOtherBank = impsCharge;
                }

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

        public Employee GetEmployee()
        {
            return DataStorage.Employees.FirstOrDefault(emp => emp.Type == Enums.UserType.Employee);
        }
    }
}
