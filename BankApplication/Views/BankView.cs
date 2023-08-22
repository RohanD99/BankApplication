using BankApplication.Common;
using BankApplication.Models;
using BankApplication.Services;
using System;
using static BankApplication.Common.Enums;

namespace BankApplication.Views
{
    public class BankView
    {
        private BankService BankService;

        public BankView()
        {
            this.BankService = new BankService();
        }

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
                            this.SetupBank();
                            break;

                        case MainMenu.LoginAsAccountHolder:
                            this.Login(UserType.AccountHolder);
                            break;

                        case MainMenu.LoginAsBankStaff:
                            this.Login(UserType.Employee);
                            break;

                        case MainMenu.Exit:
                            Console.WriteLine("Thank you for Visiting...");
                            Environment.Exit(Environment.ExitCode);
                            break;

                        default:
                            Console.WriteLine("Please enter a valid input.");
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

        private void SetupBank()
        {
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
                var response = this.BankService.Create(bank);
                Console.WriteLine(response.Message);

                if (response.IsSuccess)
                {
                    Console.WriteLine("Bank Details:\n" +
                                      $"Bank ID: {bank.Id.ToUpper()}\n" +
                                      $"Bank Name: {bank.Name}\n" +
                                      $"Location: {bank.Location}\n" +
                                      $"IFSC Code: {bank.IFSC}\n" +
                                      $"Created By: {bank.CreatedBy}\n" +
                                      $"Created On: {bank.CreatedOn}");

                    SetupBankAdmin(bank.Id);
                }
                else
                {
                    Console.WriteLine("Failed to create bank. Please try again.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void SetupBankAdmin(string bankID)
        {
            EmployeeService employeeService = new EmployeeService();
            try
            {
                Employee admin = new Employee()
                {
                    BankId = bankID,
                    Name = Utility.GetStringInput("Enter Admin Name", true),
                    UserName = Utility.GetStringInput("Enter User Name", true),
                    Password = Utility.GetStringInput("Enter Password", true),
                    Type = Enums.UserType.Admin
                };

                var response = employeeService.Create(admin);
                if (!response.IsSuccess)
                    SetupBankAdmin(bankID);

                Console.WriteLine(response.Message);
                Console.WriteLine($"Admin's ID : {admin.Id}");
                Console.WriteLine($"Admin's Password : {admin.Password}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void Login(UserType userType)
        {
            SecurityService securityService = new SecurityService();
            string username = Utility.GetStringInput("Username", true);
            string password = Utility.GetStringInput("Password", true);
            User loggedinUser = securityService.Login(username, password, userType);

            if (loggedinUser != null)
            {
                if (userType == UserType.Employee && loggedinUser is Employee employee)
                {
                    if (employee.Type == UserType.Admin) 
                        new AdminView(employee).Initiate();
                    else
                        new EmployeeView(employee).Initiate();
                }
                else if (userType == UserType.AccountHolder && loggedinUser is AccountHolder accountHolder)
                    new AccountHolderView(accountHolder).Initiate();
            }
            else
            {
                Console.WriteLine("Login failed. Please check your username and password.");
                Login(userType);
            }
        }
    }
}
