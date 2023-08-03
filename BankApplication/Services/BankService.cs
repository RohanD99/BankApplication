using BankApplication.Common;
using BankApplication.Models;
using BankApplication.Views;
using System;
using System.Linq;
using System.Text;
using static BankApplication.Common.Enums;

namespace BankApplication.Services
{
    internal class BankService
    {
        public Response<string> CreateBank(Bank bank)
        {
            Response<string> Response = new Response<string>();
            try
            {
                bank.Id = Utility.GenerateBankId(bank.Name);
                bank.CreatedOn = DateTime.Now;
                bank.ModifiedOn = DateTime.Now;
                DataStorage.Banks.Add(bank);

                Response.IsSuccess = true;
                Response.Message = Constants.BankCreation;
                Response.Data = bank.Id;
            }
            catch
            {
                Response.IsSuccess = false;
                Response.Message = Constants.BankFailure;
            }

            return Response;
        }
      
        public Response<string> Deposit(AccountHolder account, decimal amount)
        {
            Response<string> Response = new Response<string>();

            try
            {
                if (amount <= 0)
                {
                    Response.IsSuccess = false;
                    Response.Message = Constants.InvalidAmount;
                    return Response;
                }

                account.Balance += amount;

                string transactionId = Utility.GenerateTransactionId(account.Id, account.AccountNumber);

                Transaction transaction = new Transaction
                {
                    Id = transactionId,
                    SrcAccount = account.AccountNumber,
                    Type = Constants.Deposited,
                    Amount = amount,
                    CreatedBy = account.CreatedBy,
                    CreatedOn = DateTime.Now
                };

                DataStorage.Transactions.Add(transaction);

                Response.IsSuccess = true;
                Response.Message = Constants.DepositSuccess;
                Response.Data = account.Balance.ToString();
            }
            catch (Exception ex)
            {
                Response.IsSuccess = false;
                Response.Message = ex.Message;
            }

            return Response;
        }

        public Response<string> Withdraw(AccountHolder account, decimal amount)
        {
            Response<string> Response = new Response<string>();

            try
            {
                if (amount <= 0)
                {
                    Response.IsSuccess = false;
                    Response.Message = Constants.InvalidAmount;
                    return Response;
                }

                if (amount > account.Balance)
                {
                    Response.IsSuccess = false;
                    Response.Message = Constants.InsufficientFunds;
                    return Response;
                }

                account.Balance -= amount;
                string transactionId = Utility.GenerateTransactionId(account.Id, account.AccountNumber);

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

                DataStorage.Transactions.Add(transaction);

                Response.IsSuccess = true;
                Response.Message = Constants.WithdrawalSuccess;
                Response.Data = account.Balance.ToString();
            }
            catch (Exception ex)
            {
                Response.IsSuccess = false;
                Response.Message = ex.Message;
            }

            return Response;
        }

        public Response<string> TransferFunds(AccountHolder sourceAccount, string destinationAccountNumber, decimal amount, TransferOptions transferType)
        {
            Response<string> Response = new Response<string>();

            try
            {
                AccountHolder destinationAccount = DataStorage.Accounts.FirstOrDefault(a => a.AccountNumber == destinationAccountNumber);
                if (destinationAccount == null)
                {
                    Response.IsSuccess = false;
                    Response.Message = Constants.AccountNotFound;
                    return Response;
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
                    Response.IsSuccess = false;
                    Response.Message = Constants.InvalidType;
                    return Response;
                }

                decimal transferAmount = amount + (amount * charge);
                if (transferAmount <= 0)
                {
                    Response.IsSuccess = false;
                    Response.Message = Constants.InvalidAmount;
                    return Response;
                }

                if (transferAmount > sourceAccount.Balance)
                {
                    Response.IsSuccess = false;
                    Response.Message = Constants.InsufficientFunds;
                    return Response;
                }

                sourceAccount.Balance -= transferAmount;
                destinationAccount.Balance += amount;

                // Create and store source transaction
                Transaction sourceTransaction = new Transaction
                {
                    Id = Utility.GenerateTransactionId(sourceAccount.Id, sourceAccount.AccountNumber),
                    SrcAccount = sourceAccount.AccountNumber,
                    DstAccount = destinationAccountNumber,
                    Type = Constants.TransferFunds,
                    Amount = -transferAmount,
                    CreatedBy = sourceAccount.Name,
                    CreatedOn = DateTime.Now
                };

                // Create and store destination transaction
                Transaction destinationTransaction = new Transaction
                {
                    Id = Utility.GenerateTransactionId(destinationAccount.Id, destinationAccountNumber),
                    SrcAccount = sourceAccount.AccountNumber,
                    DstAccount = destinationAccountNumber,
                    Type = Constants.TransferFunds,
                    Amount = amount,
                    CreatedBy = destinationAccount.Name,
                    CreatedOn = DateTime.Now
                };

                DataStorage.Transactions.Add(sourceTransaction);
                DataStorage.Transactions.Add(destinationTransaction);

                Response.IsSuccess = true;
                Response.Message = Constants.TransferFundsSuccess;
            }
            catch (Exception ex)
            {
                Response.IsSuccess = false;
                Response.Message = ex.Message;
            }

            return Response;
        }
    
        public Response<string> CheckBalance(AccountHolder account)
        {
            Response<string> Response = new Response<string>();

            try
            {
                Response.IsSuccess = true;
                Response.Data = account.Balance.ToString();
            }
            catch (Exception ex)
            {
                Response.IsSuccess = false;
                Response.Message = ex.Message;
            }

            return Response;
        }

        public Response<string> ViewTransactionHistory(AccountHolder account)
        {
            Response<string> Response = new Response<string>();

            try
            {
                var transactions = DataStorage.Transactions
                    .Where(t => t.SrcAccount == account.AccountNumber || t.DstAccount == account.AccountNumber)
                    .ToList();

                if (transactions.Any())
                {
                    EmployeeView.PrintTransactionDetails(transactions);
                    Response.IsSuccess = true;
                    Response.Message = Constants.TransactionSuccess;
                }
                else
                {
                    Response.IsSuccess = false;
                    Response.Message = Constants.TransactionFailure;
                    Response.Data = string.Empty;
                }
            }
            catch (Exception ex)
            {
                Response.IsSuccess = false;
                Response.Message = ex.Message;
            }

            return Response;
        }

        public Employee GetEmployee()
        {
            return DataStorage.Employees.FirstOrDefault(emp => emp.Type == Enums.UserType.Employee);
        }

       
    }
}
