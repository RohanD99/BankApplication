using BankApplication.Common;
using BankApplication.Models;
using System;
using static BankApplication.Common.Enums;

namespace BankApplication.Services
{
    internal class BankService
    {
        private TransactionService TransactionService;

        public BankService()
        {
            this.TransactionService = new TransactionService();
        }

        public Response<string> Create(Bank bank)
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
                string transactionId = this.TransactionService.AddTransactionAndGetId(new Transaction
                {
                    SrcAccount = account.AccountNumber,
                    Type = Constants.Deposited,
                    Amount = amount,
                    CreatedBy = account.CreatedBy,
                    CreatedOn = DateTime.Now
                }, account);

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
                string transactionId = this.TransactionService.AddTransactionAndGetId(new Transaction
                {
                    SrcAccount = account.AccountNumber,
                    DstAccount = account.AccountNumber,
                    Type = Constants.Withdrawal,
                    Amount = -amount,
                    CreatedBy = account.Name,
                    CreatedOn = DateTime.Now
                }, account);

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
                this.TransactionService.AddTransactionAndGetId(new Transaction
                {
                    SrcAccount = sourceAccount.AccountNumber,
                    DstAccount = destinationAccount.AccountNumber,
                    Type = Constants.TransferFunds,
                    Amount = -transferAmount,
                    CreatedBy = sourceAccount.Name,
                    CreatedOn = DateTime.Now
                }, sourceAccount);

                // destination transaction
                this.TransactionService.AddTransactionAndGetId(new Transaction
                {
                    SrcAccount = sourceAccount.AccountNumber,
                    DstAccount = destinationAccount.AccountNumber,
                    Type = Constants.TransferFunds,
                    Amount = amount,
                    CreatedBy = destinationAccount.Name,
                    CreatedOn = DateTime.Now
                }, destinationAccount);

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
                Bank bank = DataStorage.Banks.Find(b => b.Id == bankID);
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

        public Response<string> RevertTransaction(string transactionId)
        {
            Response<string> response = new Response<string>();

            try
            {
                Transaction transactionToRevert = DataStorage.Transactions.Find(t => t.Id == transactionId);

                if (transactionToRevert != null)
                {
                    AccountHolder account = DataStorage.AccountHolders.Find(a => a.AccountNumber == transactionToRevert.SrcAccount);

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
                        //account was not found
                        response.IsSuccess = false;
                        response.Message = Constants.AccountNotFound;
                    }
                }
                else
                {
                    //The transaction with the given ID was not found
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
    }
}
