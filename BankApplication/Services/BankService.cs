using BankApplication.Common;
using BankApplication.Models;
using System;
using System.Linq;
using static BankApplication.Common.Enums;

namespace BankApplication.Services
{
    internal class BankService
    {
        User LoggedInUser { get; set; }
        private TransactionService TransactionService;
        private AccountHolderService AccountHolderService;

        public BankService()
        {
            this.TransactionService = new TransactionService();
            this.AccountHolderService = new AccountHolderService();
            this.LoggedInUser = new User();
        }

        public Response<string> Create(Bank bank)
        {
            Response<string> response = new Response<string>();
            Employee employee = new Employee();
            try
            {
                bank.Id = Utility.GenerateBankId(bank.Name);
                bank.CreatedOn = DateTime.Now;
                bank.CreatedBy = this.LoggedInUser.Id;
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

                    Transaction transaction = new Transaction
                    {
                        SrcAccount = accountHolder.AccountNumber,
                        SrcBankId = accountHolder.BankId,
                        Type = TransactionType.Deposit,
                        Amount = amount,
                        CreatedBy = this.LoggedInUser.Id,
                        CreatedOn = DateTime.Now,
                    };

                    string transactionId = this.TransactionService.Create(transaction);

                    response.IsSuccess = true;
                    response.Message = Constants.DepositSuccess;
                    response.Data = accountHolder.Balance.ToString();
                }
                else
                {
                    response.IsSuccess = false;
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

                    Transaction transaction = new Transaction
                    {
                        SrcAccount = accountHolder.AccountNumber,
                        SrcBankId = accountHolder.BankId,
                        Type = TransactionType.Withdraw,
                        Amount = amount,
                        CreatedBy = this.LoggedInUser.Id,
                        CreatedOn = DateTime.Now,
                    };

                    string transactionId = this.TransactionService.Create(transaction);
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

        public Response<string> TransferFunds(string bankId, string sourceAccountHolderID, string destinationAccountHolderID, decimal amount, TransferOptions transferType)
        {
            Response<string> response = new Response<string>();
            try
            {
                var srcAccHdr = this.AccountHolderService.GetAccountHolderById(sourceAccountHolderID);
                var dstAccHdr = this.AccountHolderService.GetAccountHolderById(destinationAccountHolderID);

                if (srcAccHdr != null && dstAccHdr != null && amount > 0 && amount <= srcAccHdr.Balance)
                {
                    Bank bank = this.GetBankById(bankId);

                    if (bank != null)
                    {
                        decimal charge = 0;
                        if (transferType == TransferOptions.IMPS)
                        {
                            charge = bank.IMPSforOtherBank > 0 ? bank.IMPSforSameBank : 0;
                        }
                        else if (transferType == TransferOptions.RTGS)
                        {
                            charge = bank.RTGSforOtherBank > 0 ? bank.RTGSforSameBank : 0;
                        }
                        else
                        {
                            response.IsSuccess = false;
                            response.Message = Constants.InvalidType;
                            return response;
                        }

                        decimal transferAmount = amount + (amount * charge);
                        if (transferAmount > srcAccHdr.Balance)
                        {
                            response.IsSuccess = false;
                            response.Message = Constants.InsufficientFunds;
                            return response;
                        }

                        srcAccHdr.Balance -= transferAmount;
                        dstAccHdr.Balance += amount;

                        this.AccountHolderService.Update(srcAccHdr);
                        this.AccountHolderService.Update(dstAccHdr);

                        Transaction transaction = new Transaction
                        {
                            SrcAccount = srcAccHdr.AccountNumber,
                            DstAccount = dstAccHdr.AccountNumber,
                            SrcBankId = srcAccHdr.BankId,
                            DstBankId = dstAccHdr.BankId,
                            Type = TransactionType.Transfer,
                            Amount = amount,
                            CreatedBy = this.LoggedInUser.Id,
                            CreatedOn = DateTime.Now
                        };

                        this.TransactionService.Create(transaction);
                        response.IsSuccess = true;
                        response.Message = Constants.TransferFundsSuccess;
                    }
                    else
                    {
                        response.IsSuccess = false;
                        response.Message = Constants.BankNotFound;
                    }
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

        public Response<string> AddAcceptedCurrency(string bankId, string currencyCode, decimal exchangeRate)
        {
            Response<string> response = new Response<string>();

            try
            {
                Bank bank = DataStorage.Banks.Find(b => b.Id == bankId);

                if (bank == null)
                {
                    response.IsSuccess = false;
                    response.Message = Constants.BankNotFound;
                }
                else if (Constants.acceptedCurrencies.Any(c => c.Currency == currencyCode))
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
                    Constants.acceptedCurrencies.Add((currencyCode, exchangeRate));
                    bank.ModifiedOn = DateTime.Now;
                    bank.ModifiedBy = this.LoggedInUser.Id;
                    this.UpdateBank(bank);

                    response.IsSuccess = true;
                    response.Message = Constants.CurrencyAdded;
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
                Bank bank = this.GetBankById(bankID);
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

                bank.ModifiedOn = DateTime.Now;
                bank.ModifiedBy = this.LoggedInUser.Id;
                this.UpdateBank(bank);

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
                    AccountHolder srcAcc = DataStorage.AccountHolders.Find(a => a.Id == transactionToRevert.SrcAccount);
                    AccountHolder dstAcc = DataStorage.AccountHolders.Find(a => a.Id == transactionToRevert.DstAccount);

                    if (dstAcc != null && srcAcc != null && dstAcc.Balance >= transactionToRevert.Amount)
                    {
                        dstAcc.Balance -= transactionToRevert.Amount;
                        srcAcc.Balance += transactionToRevert.Amount;
                  
                        Transaction transaction = new Transaction
                        {
                            SrcAccount = dstAcc.AccountNumber,
                            DstAccount = srcAcc.AccountNumber,
                            SrcBankId = dstAcc.BankId,
                            DstBankId = srcAcc.BankId,
                            Type = TransactionType.Revert,
                            Amount = transactionToRevert.Amount,
                            CreatedBy = this.LoggedInUser.Id,
                            CreatedOn = DateTime.Now
                        };

                        this.TransactionService.Create(transaction);
                        this.AccountHolderService.Update(dstAcc);
                        this.AccountHolderService.Update(srcAcc);

                        response.IsSuccess = true;
                        response.Message = Constants.TransactionRevert;
                    }
                    else
                    {
                        response.IsSuccess = false;
                        response.Message = dstAcc == null || srcAcc == null ? Constants.AccountNotFound : Constants.InsufficientFunds;
                    }
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = Constants.TransactionNotFound;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public Bank GetBankById(string bankId)
        {
            return DataStorage.Banks.Find(bank => bank.Id == bankId);
        }

        public void UpdateBank(Bank bank)
        {
            int index = DataStorage.Banks.FindIndex(b => b.Id == bank.Id);
            if (index != -1)
            {
                DataStorage.Banks[index] = bank;
            }
        }
    }
}
