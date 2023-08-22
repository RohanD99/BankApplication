using BankApplication.Common;
using BankApplication.Models;
using BankApplication.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using static BankApplication.Common.Enums;

namespace BankApplication.Views
{
    internal class AdminView
    {
        AccountHolderService AccountHolderService;
        BankService BankService;
        EmployeeService EmployeeService;

        public AdminView(Employee loggedInUser)
        {
            this.AccountHolderService = new AccountHolderService();
            this.BankService = new BankService();
            this.EmployeeService = new EmployeeService();
        }

        public void Initiate()
        {
            AdminOption option;
            do
            {
                Utility.GenerateOptions(Constants.AdminOption);
                option = (AdminOption)Convert.ToInt32(Console.ReadLine());
                switch (option)
                {
                    case AdminOption.CreateEmployee:
                        this.AddEmployee();
                        break;

                    case AdminOption.UpdateEmployee:
                        this.UpdateEmployee();
                        break;

                    case AdminOption.DeleteEmployee:
                        this.DeleteEmployee();
                        break;

                    case AdminOption.GetEmployees:
                        this.GetAllEmployees();
                        break;

                    case AdminOption.CreateAccountHolder:
                        this.AddAccountHolder();
                        break;

                    case AdminOption.UpdateAccountHolder:
                        this.UpdateAccountHolder();
                        break;

                    case AdminOption.DeleteAccountHolder:
                        this.DeleteAccountHolder();
                        break;

                    case AdminOption.ShowAllAccountHolders:
                        this.ShowAllAccountHolders();
                        break;

                    case AdminOption.AddCurrency:
                        this.AddCurrency();
                        break;

                    case AdminOption.UpdateServiceChargesForSameBank:
                        this.UpdateServiceChargesForBank(true);
                        break;

                    case AdminOption.UpdateServiceChargesForOtherBank:
                        this.UpdateServiceChargesForBank(false);
                        break;

                    case AdminOption.ShowAccountHolderTransactions:
                        this.ShowAccountHolderTransactionsForAdmin();
                        break;

                    case AdminOption.RevertTransaction:
                        this.RevertTransaction();
                        break;

                    case AdminOption.Logout:
                        break;

                    default:
                        Console.WriteLine("Please enter a valid input.");
                        break;
                }
            } while (option != AdminOption.Logout);
        }

        public void AddAccountHolder()
        {
            EmployeeService employeeService = new EmployeeService();
            AccountHolderService accountHolderService = new AccountHolderService();

            AccountHolder accountHolder = new AccountHolder()
            {
                UserName = Utility.GetStringInput("Enter username", true),
                Password = Utility.GetStringInput("Enter password", true),
                Name = Utility.GetStringInput("Enter account holder name", true),
                AccountType = Utility.GetStringInput("Enter account type", true),
                BankId = Utility.GetStringInput("Enter bankId", true),
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
            Response<List<AccountHolder>> showAllResponse = AccountHolderService.GetAllAccountHolders(bankIdToView);

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

        public void ShowAccountHolderTransactions(string bankId)
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

        public void UpdateServiceChargesForBank(bool isSameBankAccount)
        {
            string bankID = Utility.GetStringInput("Bank ID", true);
            decimal rtgsCharge = Utility.GetDecimalInput("RTGS Charge", true);
            decimal impsCharge = Utility.GetDecimalInput("IMPS Charge", true);

            Response<string> updateResponse = BankService.UpdateServiceCharges(rtgsCharge, impsCharge, bankID, isSameBankAccount);
            Console.WriteLine(updateResponse.Message);
        }

        public void AddEmployee()
        {
            try
            {
                Employee employee = new Employee()
                {
                    Id = Utility.GenerateEmployeeID(),
                    Name = Utility.GetStringInput("Enter Employee Name", true),
                    UserName = Utility.GetStringInput("Enter UserName", true),
                    Password = Utility.GetStringInput("Enter Password", true),
                    Email = Utility.GetStringInput("Enter Email", true),
                    Designation = Utility.GetStringInput("Enter Designation", true),
                    Type = Enums.UserType.Employee
                };

                DataStorage.Employees.Add(employee);
                Console.WriteLine("Employee added successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void UpdateEmployee()
        {
            string employeeIdToUpdate = Utility.GetStringInput("Enter Employee ID to update: ", true);
            Employee updatedEmployee = new Employee();
            updatedEmployee.Id = employeeIdToUpdate;
            updatedEmployee.Name = Utility.GetStringInput("Enter updated Employee Name: ", true);
            updatedEmployee.UserName = Utility.GetStringInput("Enter updated User Name: ", true);
            updatedEmployee.Password = Utility.GetStringInput("Enter updated Password: ", true);
            updatedEmployee.Email = Utility.GetStringInput("Enter updated Email: ", true);

            Response<string> updateResponse = EmployeeService.Update(updatedEmployee);
            Console.WriteLine(updateResponse.Message);
        }

        public void DeleteEmployee()
        {
            string employeeIdToDelete = Utility.GetStringInput("Enter Employee ID to delete: ", true);
            Response<string> deleteResponse = EmployeeService.Delete(employeeIdToDelete);
            Console.WriteLine(deleteResponse.Message);
        }

        public void GetAllEmployees()
        {
            List<Employee> employees = EmployeeService.GetAllEmployees();
            foreach (Employee emp in employees)
            {
                Console.WriteLine($"Employee ID: {emp.Id}\n" +
                                  $"Employee Name: {emp.Name}\n" +
                                  $"Employee UserName: {emp.UserName}\n" +
                                  $"Employee Password: {emp.Password}\n");
            }
        }

        public void ShowAccountHolderTransactionsForAdmin()
        {
            string bankId = Utility.GetStringInput("Enter Account Holder's BankId: ", true);
            ShowAccountHolderTransactions(bankId);
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
