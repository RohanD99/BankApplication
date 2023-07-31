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
      
        public void Initialize()
        {          
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
                                Console.WriteLine($"Welcome, {loggedInAccountHolder.Name}!");
                                UserView.UserAccountMenu(loggedInAccountHolder); // Pass the logged-in account holder here
                            }
                            else
                            {
                                Console.WriteLine("Account not found");
                            }
                            break;
                                                
                        case MainMenuOption.LoginAsBankStaff:
                            Employee loggedInEmployee = VerifyEmployeeCredentials();
                            if (loggedInEmployee != null)
                            {
                                Console.WriteLine($"Welcome, {loggedInEmployee.Name}!");
                                AccountHolderView.BankStaffMenu();
                            }
                            else
                            {
                                Console.WriteLine("Employee not found");
                            }
                            break;
                         
                        case MainMenuOption.Exit:
                            Console.WriteLine("Thank you for Visiting...");
                            Environment.Exit(Environment.ExitCode);
                            break;
                        default:
                            Console.WriteLine("Please enter a valid input.");
                            this.Initialize();
                            break;
                    }
                } while (option != MainMenuOption.Exit);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Please enter valid input.");
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
            Console.WriteLine(response.Message);
           
            if (!response.IsSuccess)
            {
                CreateNewBank();
            }
            else
            {  
                Console.WriteLine("Bank Details:");
                Console.WriteLine($"Bank ID: {Bank.Id.ToUpper()}");
                Console.WriteLine($"Bank Name: {Bank.Name}");
                Console.WriteLine($"Location: {Bank.Location}");
                Console.WriteLine($"IFSC Code: {Bank.IFSC}");
                Console.WriteLine($"Created By: {Bank.CreatedBy}");
                Console.WriteLine($"Created On: {Bank.CreatedOn}");
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
            Console.WriteLine("Admin added successfully");
            currentEmployee = Employee;
            AddEmployee();            
            Console.WriteLine($"Employee's ID : {Employee.Id}");            
            Console.WriteLine("----------------------------------------");
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

            Console.WriteLine("Employee added successfully");

            // Display only the employee details
            Console.WriteLine("Employee List:");
            foreach (var emp in DataStorage.Employees.Where(emp => emp.Type == Enums.UserType.Employee))
            {
                Console.WriteLine($"Employee ID: {emp.Id}, Employee Name: {emp.Name}");
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

            try
            {
                Console.Write("Enter BankID: ");
                string bankId = Console.ReadLine();
                Bank selectedBank = DataStorage.Banks.FirstOrDefault(b => b.Id == bankId);

                if (selectedBank == null)
                {
                    throw new Exception("Bank not found. Account holder not added.");
                }

                accountHolder.Id = Utility.GenerateAccountId(accountHolder.Name);
                accountHolder.AccountNumber = Utility.GenerateAccountNumber(accountHolder.Name);

                DataStorage.Accounts.Add(accountHolder);

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
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private Employee VerifyEmployeeCredentials()
        {
            Console.WriteLine("Enter your username: ");
            string username = Console.ReadLine();

            Console.WriteLine("Enter your password: ");
            string password = Console.ReadLine();
           
            Employee employee = DataStorage.Employees.FirstOrDefault(e => e.UserName == username && e.Password == password);
            return employee;
        }

        private AccountHolder VerifyAccountHolderCredentials()
        {
            Console.WriteLine("Enter your username: ");
            string username = Console.ReadLine();

            Console.WriteLine("Enter your password: ");
            string password = Console.ReadLine();

            AccountHolder accountHolder = DataStorage.Accounts.FirstOrDefault(a => a.UserName == username && a.Password == password);
            return accountHolder;
        }
    }
}

