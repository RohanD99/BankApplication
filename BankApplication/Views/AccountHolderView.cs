using BankApplication.Common;
using BankApplication.Models;
using BankApplication.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using static BankApplication.Common.Enums;

namespace BankApplication.Views
{
    internal class AccountHolderView
    {
        static AccountHolderService accountHolderService = new AccountHolderService();
        public static void BankStaffMenu()
        {
            BankStaffOption option;
            do
            {
                BankService bankService = new BankService();
                TransactionService transactionService = new TransactionService();
                Employee loggedInEmployee = bankService.GetEmployee();
                Utility.GenerateOptions(Constants.BankStaffOption);
                option = (BankStaffOption)Convert.ToInt32(Console.ReadLine());
                switch (option)
                {
                    case BankStaffOption.CreateAccountHolder:
                        AddAccountHolder();
                        break;

                    case BankStaffOption.UpdateAccountHolder:
                        string accountToUpdate = Utility.GetStringInput("Enter Account ID to update account holder: ", true);
                        AccountHolderService AccountService = new AccountHolderService();
                        AccountHolder accountHolderToUpdate = AccountService.GetAccountHolderById(accountToUpdate);
                        UpdateAccountHolder(accountHolderToUpdate);
                        break;

                    case BankStaffOption.DeleteAccountHolder:
                        DeleteAccountHolder();
                        break;

                    case BankStaffOption.ShowAllAccountHolders:
                        Console.Write("Enter Bank ID to view account holders: ");
                        string bankIdToView = Console.ReadLine();  
                        Response<List<AccountHolder>> showAllResponse = accountHolderService.GetAllAccountHolders(bankIdToView);

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
                                                      $"Account holder's Account Number: {accountHolder.AccountNumber}\n" +
                                                      $"Account holder's Acc type: {accountHolder.AccountType}\n" +
                                                      $"Created by: {accountHolder.CreatedBy}\n" +
                                                      $"Created on: {accountHolder.CreatedOn}\n" +
                                                      "----------------------------------------");
                                }
                            }
                            else
                            {
                                Console.WriteLine("No account holders found for the specified bank.");
                            }
                        }
                        else
                        {
                            Console.WriteLine(showAllResponse.Message);
                        }
                        break;

                    case BankStaffOption.AddCurrency:
                        Utility.GetStringInput("Enter Currency Code: ", true);
                        string currencyCode = Console.ReadLine().ToUpper();
                        Utility.GetStringInput("Enter Exchange Rate: ", true);
                        decimal exchangeRate;
                        if (!decimal.TryParse(Console.ReadLine(), out exchangeRate))
                        {
                            Console.WriteLine("Invalid exchange rate. Please enter a valid decimal number.");
                        }
                        Response<string> response = bankService.AddAcceptedCurrency(currencyCode, exchangeRate);
                        break;

                    case BankStaffOption.UpdateServiceChargesForSameBank:
                        string bankIdForSameBank = Utility.GetStringInput("Enter Bank ID: ", true);
                        Utility.GetStringInput("Enter RTGS Charge for Same Bank: ", true);
                        float rtgsChargeSameBank = Convert.ToSingle(Console.ReadLine());
                        Utility.GetStringInput("Enter IMPS Charge for Same Bank: ", true);
                        float impsChargeSameBank = Convert.ToSingle(Console.ReadLine());
                        Response<string> updateSameBankChargeResponse = bankService.UpdateServiceCharges(rtgsChargeSameBank, impsChargeSameBank, bankIdForSameBank, true);
                        Console.WriteLine(updateSameBankChargeResponse.Message);
                        break;

                    case BankStaffOption.UpdateServiceChargesForOtherBank:
                        string bankIdForOtherBank = Utility.GetStringInput("Enter Bank ID: ", true);
                        Utility.GetStringInput("Enter RTGS Charge for Other Bank: ", true);
                        float rtgsChargeOtherBank = Convert.ToSingle(Console.ReadLine());
                        Utility.GetStringInput("Enter IMPS Charge for Other Bank: ", true);
                        float impsChargeOtherBank = Convert.ToSingle(Console.ReadLine());
                        Response<string> updateOtherBankChargeResponse = bankService.UpdateServiceCharges(rtgsChargeOtherBank, impsChargeOtherBank, bankIdForOtherBank, false);
                        Console.WriteLine(updateOtherBankChargeResponse.Message);
                        break;

                    case BankStaffOption.ShowAccountHolderTransactions:                       
                        string accountNumber = Utility.GetStringInput("Enter Account Holder's Account Number: ", true);
                        Response<string> transactionHistoryResponse = transactionService.GetTransactionHistory(null, accountNumber);

                        if (transactionHistoryResponse.IsSuccess)
                        {
                            Console.WriteLine(transactionHistoryResponse.Message);
                            Console.WriteLine(transactionHistoryResponse.Data);
                        }
                        else
                        {
                            Console.WriteLine(transactionHistoryResponse.Message);
                        }
                        break;

                    case BankStaffOption.RevertTransaction:
                        Utility.GetStringInput("Enter Transaction ID to revert: ", true);
                        string transactionIDToRevert = Console.ReadLine();
                        Response<string> revertResponse = transactionService.RevertTransaction(transactionIDToRevert);
                        Console.WriteLine(revertResponse.Message);
                        break;

                    case BankStaffOption.Logout:
                        break;

                    default:
                        Console.WriteLine("Please enter a valid input.");
                        break;
                }
            } while (option != BankStaffOption.Logout);
        }

        public static void AddAccountHolder()
        {
            BankService bankService = new BankService();
            AccountHolderService accountHolderService = new AccountHolderService();
            Employee employee = bankService.GetEmployee();

            AccountHolder accountHolder = new AccountHolder()
            {
                UserName = Utility.GetStringInput("Enter username", true),
                Password = Utility.GetStringInput("Enter password", true),
                Name = Utility.GetStringInput("Enter account holder name", true),
                AccountType = Utility.GetStringInput("Enter account type", true),
                CreatedBy = employee.Designation,
                CreatedOn = DateTime.Now,
                Type = Enums.UserType.AccountHolder,
                BankId = employee.BankId
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
                    $"Account holder's Account Number: {accountHolder.AccountNumber}\n" +
                    $"Account holder's Acc type: {accountHolder.AccountType}\n" +
                    $"Created by: {accountHolder.CreatedBy}\n" +
                    $"Created on: {accountHolder.CreatedOn}\n" +
                    $"----------------------------------------"
                );
            }
            else
            {
                Console.WriteLine(response.Message);
            }
        }

        public static void UpdateAccountHolder(AccountHolder accountHolder)
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
            }
            else
            {
                Console.WriteLine(Constants.AccountUpdateFailure);
            }
        }

        public static void DeleteAccountHolder()
        {
            string accountToDelete = Utility.GetStringInput("Enter Account ID to delete account holder: ", true);
            AccountHolderService accountHolderService = new AccountHolderService();

            Response<string> deleteResponse = accountHolderService.Delete(accountToDelete);

            Console.WriteLine(deleteResponse.Message);
        }

        public static void LoginAsAccountHolder()
        {
            EmployeeView EmployeeView = new EmployeeView();
            string username = Utility.GetStringInput("Username", true);
            string password = Utility.GetStringInput("Password", true);
            AccountHolder loggedInAccountHolder = SecurityService.Login<AccountHolder>(username, password, typeof(AccountHolder));
            if (loggedInAccountHolder != null)
            {
                EmployeeView.UserAccountMenu(loggedInAccountHolder);
            }
            else
            {
                Console.WriteLine("Account not found");
            }
        }
    }
}
