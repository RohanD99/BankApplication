using BankApplication.Common;
using BankApplication.Models;
using System;
using System.Linq;
using System.Security.Principal;
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

        public Response<string> Deposit(AccountHolder account, decimal amount)
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
                account.Balance += amount;
                string transactionId = transactionService.GetTransactionId(account);

                Transaction transaction = new Transaction
                {
                    Id = transactionId,
                    SrcAccount = account.AccountNumber,
                    Type = Constants.Deposited,
                    Amount = amount,
                    CreatedBy = account.CreatedBy,
                    CreatedOn = DateTime.Now
                };

                transactionService.AddTransactionToDataStorage(transaction);
                response.IsSuccess = true;
                response.Message = Constants.DepositSuccess;
                response.Data = account.Balance.ToString();
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
                if (amount > account.Balance)
                {
                    response.IsSuccess = false;
                    response.Message = Constants.InsufficientFunds;
                    return response;
                }
                account.Balance -= amount;
                string transactionId = transactionService.GetTransactionId(account);
                Transaction transaction = new Transaction
                {
                    Id = transactionId,
                    SrcAccount = account.AccountNumber,
                    DstAccount = account.AccountNumber,
                    Type = Constants.Withdrawal,
                    Amount = -amount,
                    CreatedBy = account.Name,
                    CreatedOn = DateTime.Now
                };

                transactionService.AddTransactionToDataStorage(transaction);
                response.IsSuccess = true;
                response.Message = Constants.WithdrawalSuccess;
                response.Data = account.Balance.ToString();
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public Response<string> TransferFunds(AccountHolder sourceAccount, AccountHolder destinationAccount, decimal amount, TransferOptions transferType)
        {
            Response<string> response = new Response<string>();
            try
            {
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
                if (transferAmount > sourceAccount.Balance)
                {
                    response.IsSuccess = false;
                    response.Message = Constants.InsufficientFunds;
                    return response;
                }
                sourceAccount.Balance -= transferAmount;
                destinationAccount.Balance += amount;
                // source transaction
                Transaction sourceTransaction = new Transaction
                {
                    Id = transactionService.GetTransactionId(sourceAccount),
                    SrcAccount = sourceAccount.AccountNumber,
                    DstAccount = destinationAccount.AccountNumber,
                    Type = Constants.TransferFunds,
                    Amount = -transferAmount,
                    CreatedBy = sourceAccount.Name,
                    CreatedOn = DateTime.Now
                };
                // destination transaction
                Transaction destinationTransaction = new Transaction
                {
                    Id = transactionService.GetTransactionId(destinationAccount),
                    SrcAccount = sourceAccount.AccountNumber,
                    DstAccount = destinationAccount.AccountNumber,
                    Type = Constants.TransferFunds,
                    Amount = amount,
                    CreatedBy = destinationAccount.Name,
                    CreatedOn = DateTime.Now
                };

                transactionService.AddTransactionToDataStorage(sourceTransaction);
                transactionService.AddTransactionToDataStorage(destinationTransaction);
                response.IsSuccess = true;
                response.Message = Constants.TransferFundsSuccess;
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

        public Response<string> AddAcceptedCurrency(string currencyCode, decimal exchangeRate)
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
                else if (exchangeRate <= 0)
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
