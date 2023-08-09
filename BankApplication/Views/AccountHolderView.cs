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
        User loggedInUser { get; set; }

        public AccountHolderView()
        {
            loggedInUser = new User();
        }
        public static void InitiateBankStaff(string bankId)
        {
            BankStaffOption option;
            do
            {
                BankService bankService = new BankService();               
                AccountHolderView accountHolderView = new AccountHolderView();
       
                Utility.GenerateOptions(Constants.BankStaffOption);
                option = (BankStaffOption)Convert.ToInt32(Console.ReadLine());
                switch (option)
                {
                    case BankStaffOption.CreateAccountHolder:
                        accountHolderView.AddAccountHolder();
                        break;

                    case BankStaffOption.UpdateAccountHolder:
                        string accountToUpdate = Utility.GetStringInput("Enter Account ID to update account holder: ", true);
                        AccountHolderService AccountService = new AccountHolderService();
                        AccountHolder accountHolderToUpdate = AccountService.GetAccountHolderById(accountToUpdate);
                        accountHolderView.UpdateAccountHolder(accountHolderToUpdate);
                        break;

                    case BankStaffOption.DeleteAccountHolder:
                        accountHolderView.DeleteAccountHolder();
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
                                                      $"Account holder's Account Number: {accountHolder.AccountNumber}\n");
                                }
                            }
                            else                           
                                Console.WriteLine("No account holders found for the specified bank.");                            
                        }
                        else                      
                            Console.WriteLine(showAllResponse.Message);
                        break;

                    case BankStaffOption.AddCurrency:
                        Utility.GetStringInput("Enter Currency Code: ", true);
                        string currencyCode = Console.ReadLine().ToUpper();
                        Utility.GetStringInput("Enter Exchange Rate: ", true);
                        decimal exchangeRate;
                        if (!decimal.TryParse(Console.ReadLine(), out exchangeRate))
                            Console.WriteLine("Invalid exchange rate. Please enter a valid decimal number.");
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
                        TransactionService transactionService = new TransactionService();
                        string accountNumber = Utility.GetStringInput("Enter Account Holder's Account Number: ", true);
                        Response<List<Transaction>> transactionHistoryResponse = transactionService.GetTransactionHistory(null, accountHolderView.loggedInUser.BankId, accountNumber);

                        if (transactionHistoryResponse.IsSuccess)
                        {
                            Console.WriteLine(transactionHistoryResponse.Message);
                            Utility.GetTransactionDetails(transactionHistoryResponse.Data);
                        }
                        else
                            Console.WriteLine(transactionHistoryResponse.Message);
                        break;
                        
                    case BankStaffOption.RevertTransaction:
                        Utility.GetStringInput("Enter Transaction ID to revert: ", true);
                        string transactionIDToRevert = Console.ReadLine();
                        Response<string> revertResponse = bankService.RevertTransaction(transactionIDToRevert);
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

        public void AddAccountHolder()
        {
            EmployeeService employeeService = new EmployeeService();
            AccountHolderService accountHolderService = new AccountHolderService();
            string employeeId = Utility.GetStringInput("Enter account Id", true);
            Employee employee = employeeService.GetEmployee(employeeId);

            AccountHolder accountHolder = new AccountHolder()
            {
                UserName = Utility.GetStringInput("Enter username", true),
                Password = Utility.GetStringInput("Enter password", true),
                Name = Utility.GetStringInput("Enter account holder name", true),
                AccountType = Utility.GetStringInput("Enter account type", true),
                CreatedBy = employee.Id,
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
                    $"Account holder's Account Number: {accountHolder.AccountNumber}\n"
                );
            }
            else
                Console.WriteLine(response.Message);
        }

        public void UpdateAccountHolder(AccountHolder accountHolder)
        {
            AccountHolderService accountHolderService = new AccountHolderService();
           
            if (accountHolder != null)
            {
                AccountHolder oldAccountHolder = accountHolderService.GetAccountHolderByAccountNumber(accountHolder.AccountNumber);
                oldAccountHolder.UserName = accountHolder.UserName;
                oldAccountHolder.Password = accountHolder.Password;
                oldAccountHolder.Name = accountHolder.Name;
                oldAccountHolder.AccountType = accountHolder.AccountType;

                Response<AccountHolder> updateResponse = accountHolderService.Update(accountHolder);
                if (updateResponse.IsSuccess)
                    Console.WriteLine(updateResponse.Message);              
            }
            else
                Console.WriteLine(Constants.AccountUpdateFailure);
        }

        public void DeleteAccountHolder()
        {
            string accountToDelete = Utility.GetStringInput("Enter Account ID to delete account holder: ", true);
            AccountHolderService accountHolderService = new AccountHolderService();

            Response<string> deleteResponse = accountHolderService.Delete(accountToDelete);

            Console.WriteLine(deleteResponse.Message);
        }

        public void LoginAsAccountHolder()
        {
            SecurityService securityService = new SecurityService();
            EmployeeView EmployeeView = new EmployeeView();
            string username = Utility.GetStringInput("Username", true);
            string password = Utility.GetStringInput("Password", true);
            AccountHolder loggedInAccountHolder = securityService.Login<AccountHolder>(username, password, UserType.AccountHolder);
            if (loggedInAccountHolder != null)
                EmployeeView.InitiateUserAccount(loggedInAccountHolder);
            else
                Console.WriteLine("Account not found");
        }
    }
}
