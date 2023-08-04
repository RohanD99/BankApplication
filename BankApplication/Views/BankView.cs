using System;
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
                            LoginAsAccountHolder();
                            break;

                        case MainMenu.LoginAsBankStaff:
                            LoginAsBankStaff();
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
                Employee Admin = new Employee()
                {
                    BankId = bankID,
                    Name = Utility.GetStringInput("Enter Admin Name", true),
                    UserName = Utility.GetStringInput("Enter User Name", true),
                    Password = Utility.GetStringInput("Enter Password", true),
                    Type = Enums.UserType.Admin
                };

                EmployeeService.Create(Admin);
                Console.WriteLine("Admin added successfully");
                CurrentEmployee = Admin;
                AddEmployee();
                Console.WriteLine($"Employee's ID : {Admin.Id}");
                Console.WriteLine($"Employee's BankID : {Admin.BankId}");
                Console.WriteLine("----------------------------------------");
                return Admin.Id;
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
                Employee Employee = new Employee()
                {
                    Name = Utility.GetStringInput("Enter Employee Name", true),
                    UserName = Utility.GetStringInput("Enter UserName", true),
                    Password = Utility.GetStringInput("Enter Password", true),
                    Email = Utility.GetStringInput("Enter Email", true),
                    Designation = Utility.GetStringInput("Enter Designation", true),
                    Type = Enums.UserType.Employee
                };

                DataStorage.Employees.Add(Employee);
                Console.WriteLine("Employee added successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public static void LoginAsBankStaff()
        {
            string username = Utility.GetStringInput("Username", true);
            string password = Utility.GetStringInput("Password", true);
            Employee loggedInEmployee = EmployeeService.GetEmployeeByUsernameAndPassword(username, password);

            if (loggedInEmployee != null)
            {
                AccountHolderView.BankStaffMenu();
            }
            else
            {
                Console.WriteLine("Account not found");
            }
        }

        public static void LoginAsAccountHolder()
        {
            EmployeeView EmployeeView = new EmployeeView();
            string username = Utility.GetStringInput("Username", true);
            string password = Utility.GetStringInput("Password", true);
            AccountHolder loggedInAccountHolder = EmployeeService.GetAccountHolderByUsernameAndPassword(username, password);
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

