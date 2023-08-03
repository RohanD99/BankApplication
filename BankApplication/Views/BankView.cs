using System;
using System.Text;
using BankApplication.Common;
using BankApplication.Models;
using BankApplication.Services;
using static BankApplication.Common.Enums;

namespace BankApplication.Views
{
    public class BankView
    {
        private Employee CurrentEmployee;       
        public void Initialize()
        {
            try
            {
                MainMenu option;
                do
                {
                    Utility.GenerateOptions(Constants.MainMenu);
                    option = (MainMenu)Convert.ToInt32(Console.ReadLine());
                    switch (option)
                    {
                        case MainMenu.CreateNewBank:
                            CreateNewBank();
                            break;

                        case MainMenu.LoginAsAccountHolder:
                            AccountHolderService.LoginAsAccountHolder();
                            break;
                                                
                        case MainMenu.LoginAsBankStaff:
                            BankService.LoginAsBankStaff();
                            break;
                         
                        case MainMenu.Exit:
                            Console.WriteLine("Thank you for Visiting...");
                            Environment.Exit(Environment.ExitCode);
                            break;
                        default:
                            Console.WriteLine("Please enter a valid input.");
                            this.Initialize();
                            break;
                    }
                } while (option != MainMenu.Exit);
            }
            catch (Exception)
            {
                Console.WriteLine("Please enter valid input.");
                this.Initialize();
            }
        }

        private void CreateNewBank()
        {
            BankService BankService = new BankService();
            try
            {
                Bank bank = new Bank()
                {
                    Name = Utility.GetStringInput("Enter Bank Name", true),
                    Location = Utility.GetStringInput("Enter Location", true),
                    IFSC = Utility.GetStringInput("Enter IFSC code", true),
                    IMPSforOtherBank = 6,
                    IMPSforSameBank = 5,
                    RTGSforOtherBank = 2,
                    RTGSforSameBank = 0
                };

                var response = BankService.CreateBank(bank);
                Console.WriteLine(response.Message);

                if (!response.IsSuccess)
                {
                    CreateNewBank();
                }
                else
                {
                    Console.WriteLine("Bank Details:");
                    Console.WriteLine($"Bank ID: {bank.Id.ToUpper()}");
                    Console.WriteLine($"Bank Name: {bank.Name}");
                    Console.WriteLine($"Location: {bank.Location}");
                    Console.WriteLine($"IFSC Code: {bank.IFSC}");
                    Console.WriteLine($"Created By: {bank.CreatedBy}");
                    Console.WriteLine($"Created On: {bank.CreatedOn}");
                }

                var adminName = SetupBankAdmin(bank.Id);
                bank.CreatedBy = adminName;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private string SetupBankAdmin(string bankID)
        {
            EmployeeService EmployeeService = new EmployeeService();
            try
            {
                Employee employee = new Employee()
                {
                    BankId = bankID,
                    Name = Utility.GetStringInput("Enter Admin Name", true),
                    UserName = Utility.GetStringInput("Enter User Name", true),
                    Password = Utility.GetStringInput("Enter Password", true),
                    Type = Enums.UserType.Admin
                };

                EmployeeService.Create(employee);
                Console.WriteLine("Admin added successfully");
                CurrentEmployee = employee;
                AddEmployee();
                Console.WriteLine($"Employee's ID : {employee.Id}");
                Console.WriteLine($"Employee's BankID : {employee.BankId}");
                Console.WriteLine("----------------------------------------");
                return employee.Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        private void AddEmployee()
        {
            try
            {
                Employee employee = new Employee()
                {
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

        public void AddAccountHolder()
        {
            BankService bankService = new BankService();
            AccountHolderService accountHolderService = new AccountHolderService();
            Employee employee = bankService.GetEmployee();

            if (employee == null)
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
                CreatedBy = employee.Designation,
                CreatedOn = DateTime.Now,
                Type = Enums.UserType.AccountHolder
            };

            Response<string> response = accountHolderService.Create(accountHolder, employee);

            if (response.IsSuccess)
            {
                Console.WriteLine("Account holder added successfully.");
                Console.WriteLine($"Account holder ID: {accountHolder.Id}");
                Console.WriteLine($"Account holder Name: {accountHolder.Name}");
                Console.WriteLine($"Account holder Username: {accountHolder.UserName}");
                Console.WriteLine($"Account holder's Password: {accountHolder.Password}");
                Console.WriteLine($"Account holder's Account Number: {accountHolder.AccountNumber}");
                Console.WriteLine($"Account holder's Acc type: {accountHolder.AccountType}");
                Console.WriteLine($"Created by: {accountHolder.CreatedBy}");
                Console.WriteLine($"Created on: {accountHolder.CreatedOn}");
                Console.WriteLine("----------------------------------------");
            }
            else
            {
                Console.WriteLine(response.Message);
            }
        }

        public void UpdateAccountHolder(AccountHolder accountHolder)
        {
            AccountHolderService accountHolderService = new AccountHolderService();

            if (accountHolder != null)
            {
                Console.WriteLine("Please enter the updated details:");
                accountHolder.UserName = Utility.GetStringInput(Constants.Username, false, accountHolder.UserName);
                accountHolder.Password = Utility.GetStringInput(Constants.Password, false, accountHolder.Password);
                accountHolder.Name = Utility.GetStringInput(Constants.AccountHolderName, false, accountHolder.Name);
                accountHolder.AccountType = Utility.GetStringInput(Constants.AccountType, false, accountHolder.AccountType);

                Response<AccountHolder> updateResponse = accountHolderService.Update(accountHolder);

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

        public void DeleteAccountHolder()
        {
            Utility.GetStringInput("Enter Account ID to delete account holder: ",true);
            string accountToDelete = Console.ReadLine();
            AccountHolderService accountHolderService = new AccountHolderService();

            Response<string> deleteResponse = accountHolderService.Delete(accountToDelete);

            if (deleteResponse.IsSuccess)
            {
                Console.WriteLine(deleteResponse.Message);
            }
            else
            {
                Console.WriteLine(deleteResponse.Message);
            }
        }

        public Employee VerifyEmployeeCredentials()
        {
        string username = Utility.GetStringInput("Username", true);
        string password = Utility.GetStringInput("Password", true);

            Employee employee = EmployeeService.GetEmployeeByUsernameAndPassword(username, password);
            return employee;
        }

        public AccountHolder VerifyAccountHolderCredentials()
        {
            string username = Utility.GetStringInput("Username", true);
            string password = Utility.GetStringInput("Password", true);

            AccountHolder accountHolder = EmployeeService.GetAccountHolderByUsernameAndPassword(username, password);
            return accountHolder;
        }

        public void TransferFunds(AccountHolder loggedInAccount)
        {
            BankService BankService = new BankService();
            StringBuilder sb = new StringBuilder();

            Utility.GetStringInput("Enter the destination account number: ", true);
            string destinationAccountNumber = Console.ReadLine();

            Utility.GetStringInput("Enter the transfer type (0 for IMPS, 1 for RTGS): ", true);
            int transferTypeInput = Convert.ToInt32(Console.ReadLine());

            TransferOptions transferType;
            if (transferTypeInput == 0)
            {
                transferType = TransferOptions.IMPS;
            }
            else if (transferTypeInput == 1)
            {
                transferType = TransferOptions.RTGS;
            }
            else
            {
                sb.AppendLine(Constants.InvalidType);
                return;
            }

            Utility.GetStringInput("Enter the amount to transfer: ", true);
            decimal transferAmount = Convert.ToDecimal(Console.ReadLine());

            Response<string> transferResponse = BankService.TransferFunds(loggedInAccount, destinationAccountNumber, transferAmount, transferType);

            if (transferResponse.IsSuccess)
            {
                sb.AppendLine(transferResponse.Message);
                sb.AppendLine($"New balance: {loggedInAccount.Balance}");
            }
            else
            {
                sb.AppendLine(transferResponse.Message);
            }
        }
    }
}

