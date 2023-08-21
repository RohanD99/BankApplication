using BankApplication.Common;
using BankApplication.Models;
using System;
using static BankApplication.Common.Enums;

namespace BankApplication.Services
{
    internal class BankService
    {

        private TransactionService TransactionService;
        AccountHolderService AccountHolderService;

        public BankService()
        {
            this.TransactionService = new TransactionService();
            this.AccountHolderService = new AccountHolderService();
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

        public Response<string> Deposit(string accountHolderID, decimal amount)
        {
            Response<string> response = new Response<string>();
            try
            {
                var accountHolder = this.AccountHolderService.GetAccountHolderById(accountHolderID);
                if (accountHolder != null && amount > 0)
                {
                    accountHolder.Balance += amount;
                    this.AccountHolderService.Update(accountHolder);

                    string transactionId = this.TransactionService.Create(new Transaction
                    {
                        SrcAccount = accountHolder.AccountNumber,
                        Type = TransactionType.Deposit,
                        Amount = amount,
                        CreatedBy = accountHolder.CreatedBy,
                        CreatedOn = DateTime.Now,

                    });

                    response.IsSuccess = true;
                    response.Message = Constants.DepositSuccess;
                    response.Data = accountHolder.Balance.ToString();
                }
                else
                {
                    response.IsSuccess = true;
                    response.Message = Constants.DepositFailed;
                    response.Data = accountHolder.Balance.ToString();
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public Response<string> Withdraw(string accountHolderID, decimal amount)
        {
            Response<string> response = new Response<string>();
            try
            {
                var accountHolder = this.AccountHolderService.GetAccountHolderById(accountHolderID);
                if (accountHolder != null && amount > 0 && amount <= accountHolder.Balance)
                {
                    accountHolder.Balance -= amount;
                    this.AccountHolderService.Update(accountHolder);

                    string transactionId = this.TransactionService.Create(new Transaction
                    {
                        SrcAccount = accountHolder.AccountNumber,
                        Type = TransactionType.Withdraw,
                        Amount = -amount,
                        CreatedBy = accountHolder.CreatedBy,
                        CreatedOn = DateTime.Now
                    });

                    response.IsSuccess = true;
                    response.Message = Constants.WithdrawalSuccess;
                    response.Data = accountHolder.Balance.ToString();
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = Constants.WithdrawalFailed;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public Response<string> TransferFunds(string sourceAccountHolderID, string destinationAccountHolderID, decimal amount, TransferOptions transferType)
        {
            Response<string> response = new Response<string>();
            try
            {
                var sourceAccountHolder = this.AccountHolderService.GetAccountHolderById(sourceAccountHolderID);
                var destinationAccountHolder = this.AccountHolderService.GetAccountHolderById(destinationAccountHolderID);

                if (sourceAccountHolder != null && destinationAccountHolder != null && amount > 0 && amount <= sourceAccountHolder.Balance)
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
                    if (transferAmount > sourceAccountHolder.Balance)
                    {
                        response.IsSuccess = false;
                        response.Message = Constants.InsufficientFunds;
                        return response;
                    }

                    sourceAccountHolder.Balance -= transferAmount;
                    destinationAccountHolder.Balance += amount;

                    this.AccountHolderService.Update(sourceAccountHolder);
                    this.AccountHolderService.Update(destinationAccountHolder);

                    Transaction sourceTransaction = new Transaction
                    {
                        SrcAccount = sourceAccountHolder.AccountNumber,
                        DstAccount = destinationAccountHolder.AccountNumber,
                        Type = TransactionType.Transfer,
                        Amount = -transferAmount,
                        CreatedBy = sourceAccountHolder.CreatedBy,
                        CreatedOn = DateTime.Now
                    };
                    this.TransactionService.Create(sourceTransaction);

                    Transaction destinationTransaction = new Transaction
                    {
                        SrcAccount = sourceAccountHolder.AccountNumber,
                        DstAccount = destinationAccountHolder.AccountNumber,
                        Type = TransactionType.Transfer,
                        Amount = amount,
                        CreatedBy = destinationAccountHolder.CreatedBy,
                        CreatedOn = DateTime.Now
                    };
                    this.TransactionService.Create(destinationTransaction);

                    response.IsSuccess = true;
                    response.Message = Constants.TransferFundsSuccess;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = Constants.TransferFundsFailed;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public Response<string> CheckBalance(string accountHolderID)
        {
            Response<string> response = new Response<string>();

            try
            {
                var accountHolder = this.AccountHolderService.GetAccountHolderById(accountHolderID);
                if (accountHolder != null)
                {
                    response.IsSuccess = true;
                    response.Data = accountHolder.Balance.ToString();
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
            Response<string> response = new Response<string>();

            try
            {
                if (Constants.acceptedCurrencies.ContainsKey(currencyCode))
                {
                    response.IsSuccess = false;
                    response.Message = Constants.CurrencyExists;
                }
                else if (exchangeRate <= 0)
                {
                    response.IsSuccess = false;
                    response.Message = Constants.InvalidRate;
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

        public Response<string> UpdateServiceCharges(decimal rtgsCharge, decimal impsCharge, string bankID, bool isSameBankAccount)
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
