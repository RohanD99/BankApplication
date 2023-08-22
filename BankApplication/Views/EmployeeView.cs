using BankApplication.Common;
using BankApplication.Models;
using BankApplication.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using static BankApplication.Common.Enums;

namespace BankApplication.Views
{
    internal class EmployeeView
    {
        AccountHolderService AccountHolderService;
        BankService BankService; 

        public EmployeeView(User loggedInUser)
        {
            this.AccountHolderService = new AccountHolderService();
            this.BankService = new BankService();
        }

        public void Initiate()
        {
            BankStaffOption option;
            do
            {
                Utility.GenerateOptions(Constants.BankStaffOption);
                option = (BankStaffOption)Convert.ToInt32(Console.ReadLine());
                switch (option)
                {
                    case BankStaffOption.CreateAccountHolder:
                        this.AddAccountHolder();
                        break;

                    case BankStaffOption.UpdateAccountHolder:
                        this.UpdateAccountHolder();
                        break;

                    case BankStaffOption.DeleteAccountHolder:
                        this.DeleteAccountHolder();
                        break;

                    case BankStaffOption.ShowAllAccountHolders:
                        this.ShowAllAccountHolders();
                        break;

                    case BankStaffOption.AddCurrency:
                        this.AddCurrency();
                        break;

                    case BankStaffOption.UpdateServiceChargesForSameBank:
                        this.UpdateServiceChargesForBank(true);
                        break;

                    case BankStaffOption.UpdateServiceChargesForOtherBank:
                        this.UpdateServiceChargesForBank(false);
                        break;

                    case BankStaffOption.ShowAccountHolderTransactions:
                        this.ShowAccountHolderTransactionsForEmployee();
                        break;

                    case BankStaffOption.RevertTransaction:
                        this.RevertTransaction();
                        break;

                    case BankStaffOption.Logout:
                        break;

                    default:
                        Console.WriteLine("Please enter a valid input.");
                        break;
                }
            } while (option != BankStaffOption.Logout);
        }

        public void AddAccountHolder()
        {
            EmployeeService employeeService = new EmployeeService();
            AccountHolderService accountHolderService = new AccountHolderService();

            string getBankId = Utility.GetStringInput("Enter bankid", true);
            Employee employee = new Employee();
            employee = employeeService.GetEmployeeByBankId(getBankId);

            AccountHolder accountHolder = new AccountHolder()
            {
                UserName = Utility.GetStringInput("Enter username", true),
                Password = Utility.GetStringInput("Enter password", true),
                Name = Utility.GetStringInput("Enter account holder name", true),
                AccountType = Utility.GetStringInput("Enter account type", true),
                CreatedOn = DateTime.Now,
                Type = Enums.UserType.AccountHolder,
            };

            Response<string> response = accountHolderService.Create(accountHolder);
            if (response.IsSuccess)
            {
                Console.WriteLine(
                    $"Account holder added successfully.\n" +
                    $"Account holder ID: {accountHolder.Id}\n" +
                    $"Account holder Name: {accountHolder.Name}\n" +
                    $"Account holder Username: {accountHolder.UserName}\n" +
                    $"Account holder's Password: {accountHolder.Password}\n" +
                    $"Account holder's Account Number: {accountHolder.AccountNumber}\n"
                );
            }
            else
            {
                Console.WriteLine(response.Message);
            }
        }

        public void UpdateAccountHolderDetails(AccountHolder accountHolder)
        {
            AccountHolderService AccountHolderService = new AccountHolderService();
            if (accountHolder != null)
            {
                Console.WriteLine("Please enter the updated details:");
                accountHolder.UserName = Utility.GetStringInput(Constants.Username, false, accountHolder.UserName);
                accountHolder.Password = Utility.GetStringInput(Constants.Password, false, accountHolder.Password);
                accountHolder.Name = Utility.GetStringInput(Constants.AccountHolderName, false, accountHolder.Name);
                accountHolder.AccountType = Utility.GetStringInput(Constants.AccountType, false, accountHolder.AccountType);
                Response<AccountHolder> updateResponse = AccountHolderService.Update(accountHolder);
                if (updateResponse.IsSuccess)
                {
                    Console.WriteLine(updateResponse.Message);
                }
                else
                {
                    Console.WriteLine(updateResponse.Message);
                }
            }
        }

        public void UpdateAccountHolder()
        {
            string accountToUpdate = Utility.GetStringInput("Enter Account ID to update account holder: ", true);
            AccountHolderService AccountService = new AccountHolderService();
            AccountHolder accountHolderToUpdate = AccountService.GetAccountHolderById(accountToUpdate);
            UpdateAccountHolderDetails(accountHolderToUpdate);
        }


        public void DeleteAccountHolder()
        {
            string accountToDelete = Utility.GetStringInput("Enter Account ID to delete account holder: ", true);
            AccountHolderService accountHolderService = new AccountHolderService();

            Response<string> deleteResponse = accountHolderService.Delete(accountToDelete);

            Console.WriteLine(deleteResponse.Message);
        }

        public void ShowAllAccountHolders()
        {
            string bankIdToView = Utility.GetStringInput("Enter Bank ID to view account holders: ", true);
            Response <List<AccountHolder>> showAllResponse = AccountHolderService.GetAllAccountHolders(bankIdToView);

            if (showAllResponse.IsSuccess)
            {
                if (showAllResponse.Data.Any())
                {
                    Console.WriteLine(showAllResponse.Message);
                    foreach (AccountHolder accountHolder in showAllResponse.Data)
                    {
                        Console.WriteLine($"Account holder ID: {accountHolder.Id}\n" +
                                          $"Account holder Name: {accountHolder.Name}\n" +
                                          $"Account holder Username: {accountHolder.UserName}\n" +
                                          $"Account holder's Password: {accountHolder.Password}\n" +
                                          $"Account holder's Account Number: {accountHolder.AccountNumber}\n");
                    }
                }
                else              
                    Console.WriteLine("No account holders found for the specified bank.");               
            }
            else       
                Console.WriteLine(showAllResponse.Message);          
        }

        private void ShowAccountHolderTransactions(string bankId)
        {
            TransactionService transactionService = new TransactionService();
            string accountNumber = Utility.GetStringInput("Enter Account Holder's Account Number: ", true);
            Response<List<Transaction>> transactionHistoryResponse = transactionService.GetTransactionHistory(bankId, accountNumber);

            if (transactionHistoryResponse.IsSuccess)
            {
                Console.WriteLine(transactionHistoryResponse.Message);
                string transactionDetails = Utility.GetTransactionDetails(transactionHistoryResponse.Data);
                Console.WriteLine(transactionDetails);
            }
            else
            {
                Console.WriteLine(transactionHistoryResponse.Message);
            }
        }

        private void ShowAccountHolderTransactionsForEmployee()
        {
            string bankId = Utility.GetStringInput("Enter Account Holder's BankId: ", true);
            ShowAccountHolderTransactions(bankId);
        }

        private void UpdateServiceChargesForBank(bool isSameBankAccount)
        {
            string bankID = Utility.GetStringInput("Bank ID", true);
            decimal rtgsCharge = Utility.GetDecimalInput("RTGS Charge", true);
            decimal impsCharge = Utility.GetDecimalInput("IMPS Charge", true);

            Response<string> updateResponse = BankService.UpdateServiceCharges(rtgsCharge, impsCharge, bankID, isSameBankAccount);
            Console.WriteLine(updateResponse.Message);
        }

        public void AddCurrency()
        {
            string bankId = Utility.GetStringInput("Enter Bank ID: ", true);
            string currencyCode = Utility.GetStringInput("Enter Currency Code: ", true);
            decimal exchangeRate = Utility.GetDecimalInput("Enter Exchange Rate: ", true);
            Response<string> response = this.BankService.AddAcceptedCurrency(bankId, currencyCode, exchangeRate);
            Console.WriteLine(response.Message);
        }

        public void RevertTransaction()
        {
            Utility.GetStringInput("Enter Transaction ID to revert: ", true);
            string transactionIDToRevert = Console.ReadLine();
            Response<string> revertResponse = this.BankService.RevertTransaction(transactionIDToRevert);
            Console.WriteLine(revertResponse.Message);
        }
    }
}
