using System;
using System.Collections.Generic;
using System.Linq;
using BankApplication.Common;
using BankApplication.Models;
using BankApplication.Services;
using static BankApplication.Common.Enums;

namespace BankApplication.Views
{
    public class BankView
    {
        private Employee currentEmployee;
        BankService BankService = new BankService();
        EmployeeService EmployeeService = new EmployeeService(); 
        UserView UserView = new UserView();
        private Action<string> WriteLineDelegate;
        public void Initialize()
        {
            WriteLineDelegate = Utility.GetConsoleWriteLineDelegate();
            try
            {
                MainMenuOption option;
                do
                {
                    List<string> mainMenuOptions = Enum.GetNames(typeof(MainMenuOption)).ToList();
                    Utility.GenerateOptions(mainMenuOptions);
                    option = (MainMenuOption)Convert.ToInt32(Console.ReadLine());
                    switch (option)
                    {
                        case MainMenuOption.CreateNewBank:
                            CreateNewBank();
                            break;
                        case MainMenuOption.LoginAsAccountHolder:
                            AccountHolder loggedInAccountHolder = VerifyAccountHolderCredentials();

                            if (loggedInAccountHolder != null)
                            {
                                WriteLineDelegate($"Welcome, {loggedInAccountHolder.Name}!");
                                UserAccountOption selectedOption;
                                AccountHolder Account = new AccountHolder();
                                UserView.UserAccountMenu(Account);
                            }
                            else
                            {
                                WriteLineDelegate("Account not found");
                            }
                            break;                         
                        case MainMenuOption.LoginAsBankStaff:
                            Employee loggedInEmployee = VerifyEmployeeCredentials();
                            if (loggedInEmployee != null)
                            {
                                WriteLineDelegate($"Welcome, {loggedInEmployee.Name}!");
                                BankStaffOption selectedOption;
                                AccountHolderView.BankStaffMenu();
                            }
                            else
                            {
                                WriteLineDelegate("Employee not found");
                            }
                            break;
                         
                        case MainMenuOption.Exit:
                            WriteLineDelegate("Thank you for Visiting...");
                            Environment.Exit(Environment.ExitCode);
                            break;
                        default:
                            WriteLineDelegate("Please enter a valid input.");
                            this.Initialize();
                            break;
                    }
                } while (option != MainMenuOption.Exit);
            }
            catch (Exception ex)
            {
                WriteLineDelegate("Please enter valid input.");
                this.Initialize();
            }
        }

        private void CreateNewBank()
        {           
            Bank Bank = new Bank()
            {
                Name = Utility.GetStringInput("Enter Bank Name", true),
                Location = Utility.GetStringInput("Enter Location", true),
                IFSC = Utility.GetStringInput("Enter IFSC code", true),
                IMPSforOtherBank = 6,
                IMPSforSameBank = 5,
                RTGSforOtherBank = 2,
                RTGSforSameBank = 0
            };

            var response = BankService.CreateBank(Bank);
            WriteLineDelegate(response.Message);
           
            if (!response.IsSuccess)
            {
                CreateNewBank();
            }
            else
            {  
                WriteLineDelegate("Bank Details:");
                WriteLineDelegate($"Bank ID: {Bank.Id.ToUpper()}");
                WriteLineDelegate($"Bank Name: {Bank.Name}");
                WriteLineDelegate($"Location: {Bank.Location}");
                WriteLineDelegate($"IFSC Code: {Bank.IFSC}");
                WriteLineDelegate($"Created By: {Bank.CreatedBy}");
                WriteLineDelegate($"Created On: {Bank.CreatedOn}");
            }
            var adminName = SetupBankAdmin(Bank.Id);
            Bank.CreatedBy = adminName;
        }

        private string SetupBankAdmin(string bankID)
        {
            Employee Employee = new Employee()
            {
                BankId = bankID,
                Name = Utility.GetStringInput("Enter Admin Name", true),
                UserName = Utility.GetStringInput("Enter User Name", true),
                Password = Utility.GetStringInput("Enter Password", true),
                Type = Enums.UserType.Admin
            };
            this.EmployeeService.Create(Employee);
            WriteLineDelegate("Admin added successfully");
            currentEmployee = Employee;
            AddEmployee();            
            WriteLineDelegate($"Employee's ID : {Employee.Id}");            
            WriteLineDelegate("----------------------------------------");
            return "Director"; 
        }

        private void AddEmployee()
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

            WriteLineDelegate("Employee added successfully");

            // Display only the employee details
            WriteLineDelegate("Employee List:");
            foreach (var emp in DataStorage.Employees.Where(emp => emp.Type == Enums.UserType.Employee))
            {
                WriteLineDelegate($"Employee ID: {emp.Id}, Employee Name: {emp.Name}");
            }
        }

        public void AddUser(Employee employee)
        {
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

            Console.Write("Enter BankID: ");
            string bankId = Console.ReadLine();
            Bank selectedBank = DataStorage.Banks.FirstOrDefault(b => b.Id == bankId);

            if (selectedBank == null)
            {
                WriteLineDelegate("Bank not found. Account holder not added.");
                return;
            }

            accountHolder.Id = bankId;
            accountHolder.AccountNumber = Utility.GenerateAccountNumber(accountHolder.Name);
            accountHolder.Id = Utility.GenerateAccountId(accountHolder.Name);

            DataStorage.Accounts.Add(accountHolder);

            WriteLineDelegate("Account holder added successfully.");
            WriteLineDelegate($"Account holder ID: {accountHolder.Id}");
            WriteLineDelegate($"Account holder Name: {accountHolder.Name}");
            WriteLineDelegate($"Account holder Username: {accountHolder.UserName}");
            WriteLineDelegate($"Account holder's Password: {accountHolder.Password}");
            WriteLineDelegate($"Account holder's Account Number: {accountHolder.AccountNumber}");
            WriteLineDelegate($"Account holder's Acc type: {accountHolder.AccountType}");
            WriteLineDelegate($"Created by: {accountHolder.CreatedBy}");
            WriteLineDelegate($"Created on: {accountHolder.CreatedOn}");
            WriteLineDelegate("----------------------------------------");
        }

        private Employee VerifyEmployeeCredentials()
        {
            WriteLineDelegate("Enter your username: ");
            string username = Console.ReadLine();

            WriteLineDelegate("Enter your password: ");
            string password = Console.ReadLine();

            Employee employee = DataStorage.Employees.FirstOrDefault(e => e.UserName == username && e.Password == password);
            return employee;
        }

        private AccountHolder VerifyAccountHolderCredentials()
        {
            WriteLineDelegate("Enter your username: ");
            string username = Console.ReadLine();

            WriteLineDelegate("Enter your password: ");
            string password = Console.ReadLine();

            AccountHolder accountHolder = DataStorage.Accounts.FirstOrDefault(a => a.UserName == username && a.Password == password);
            return accountHolder;
        }



    }
}

