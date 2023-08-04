using BankApplication.Common;
using BankApplication.Models;
using BankApplication.Services;
using System;
using static BankApplication.Common.Enums;

namespace BankApplication.Views
{
    internal class AccountHolderView
    {
        static AccountHolderService AccountHolderService = new AccountHolderService();
        public static void BankStaffMenu()
        {
            BankStaffOption option;
            do
            {
                BankService BankService = new BankService();
                Employee loggedInEmployee = BankService.GetEmployee();
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
                        Employee employee = BankService.GetEmployee();
                        Response<string> showAllResponse = AccountHolderService.ShowAllAccounts(employee);
                        Console.WriteLine(showAllResponse.Message);
                        Console.WriteLine(showAllResponse.Data);
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
                        Response<string> response = AccountHolderService.AddAcceptedCurrency(currencyCode, exchangeRate);
                        break;

                    case BankStaffOption.UpdateServiceChargesForSameBank:
                        Utility.GetStringInput("Enter RTGS Charge for Same Bank: ", true);
                        float rtgsChargeSameBank = Convert.ToSingle(Console.ReadLine());
                        Utility.GetStringInput("Enter IMPS Charge for Same Bank: ", true);
                        float impsChargeSameBank = Convert.ToSingle(Console.ReadLine());
                        Response<string> updateSameBankChargeResponse = AccountHolderService.AddServiceChargeForSameBankAccount(rtgsChargeSameBank, impsChargeSameBank, loggedInEmployee);
                        break;

                    case BankStaffOption.UpdateServiceChargesForOtherBank:
                        Utility.GetStringInput("Enter RTGS Charge for Other Bank: ", true);
                        float rtgsChargeOtherBank = Convert.ToSingle(Console.ReadLine());
                        Utility.GetStringInput("Enter IMPS Charge for Other Bank: ", true);
                        float impsChargeOtherBank = Convert.ToSingle(Console.ReadLine());
                        Response<string> updateOtherBankChargeResponse = AccountHolderService.AddServiceChargeForOtherBankAccount(rtgsChargeOtherBank, impsChargeOtherBank, loggedInEmployee);
                        break;

                    case BankStaffOption.ShowAccountHolderTransactions:
                        EmployeeService EmployeeService = new EmployeeService();
                        Utility.GetStringInput("Enter Account Holder's Account Number: ", true);
                        string accountNumber = Console.ReadLine();
                        EmployeeService.ShowAccountTransactionHistory(accountNumber);
                        break;

                    case BankStaffOption.RevertTransaction:
                        Utility.GetStringInput("Enter Transaction ID to revert: ", true);
                        string transactionIDToRevert = Console.ReadLine();
                        Response<string> revertResponse = AccountHolderService.RevertTransaction(transactionIDToRevert);
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
            BankService BankService = new BankService();
            AccountHolderService AccountHolderService = new AccountHolderService();
            Console.Write("Enter Employee ID: ");
            string employeeId = Console.ReadLine();
            Employee Employee = BankService.GetEmployee(employeeId);

            if (Employee == null)
            {
                Console.WriteLine("Employee not found");
                return;
            }

            AccountHolder accountHolder = new AccountHolder()
            {
                UserName = Utility.GetStringInput("Enter username", true),
                Password = Utility.GetStringInput("Enter password", true),
                Name = Utility.GetStringInput("Enter account holder name", true),
                AccountType = Utility.GetStringInput("Enter account type", true),
                CreatedBy = Employee.Designation,
                CreatedOn = DateTime.Now,
                Type = Enums.UserType.AccountHolder
            };

            Response<string> Response = AccountHolderService.Create(accountHolder, Employee);

            if (Response.IsSuccess)
            {
                Console.WriteLine("Account holder added successfully.");
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
            else
            {
                Console.WriteLine(Response.Message);
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
                else
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
            Utility.GetStringInput("Enter Account ID to delete account holder: ", true);
            string accountToDelete = Console.ReadLine();
            AccountHolderService AccountHolderService = new AccountHolderService();

            Response<string> DeleteResponse = AccountHolderService.Delete(accountToDelete);

            if (DeleteResponse.IsSuccess)
            {
                Console.WriteLine(DeleteResponse.Message);
            }
            else
            {
                Console.WriteLine(DeleteResponse.Message);
            }
        }
    }
}
