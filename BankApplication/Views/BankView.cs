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
        private BankService BankService;
        SecurityView SecurityView = new SecurityView();
        private AccountHolderView AccountHolderView = new AccountHolderView();

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
                            CreateNewBank();
                            if (AccountHolderView.LoggedInUser.Type == UserType.Admin)
                                AddEmployee();
                            break; ;

                        case MainMenu.LoginAsAccountHolder:
                            SecurityView.LoginAsAccountHolder();                            
                            break;

                        case MainMenu.LoginAsBankStaff:
                            SecurityView.LoginAsBankStaff();
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
                var response = this.BankService.Create(Bank);
                Console.WriteLine(response.Message);

                if (!response.IsSuccess)
                {
                    CreateNewBank();
                }
                else
                {
                    Console.WriteLine("Bank Details:\n" +
                                       $"Bank ID: {Bank.Id.ToUpper()}\n" +
                                       $"Bank Name: {Bank.Name}\n" +
                                       $"Location: {Bank.Location}\n" +
                                       $"IFSC Code: {Bank.IFSC}\n" +
                                       $"Created By: {Bank.CreatedBy}\n" +
                                       $"Created On: {Bank.CreatedOn}");

                    var adminName = SetupBankAdmin(Bank.Id);                    
                }            
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private bool SetupBankAdmin(string bankID)
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

                var response = EmployeeService.Create(Admin);
                Console.WriteLine(response.Message);
                Console.WriteLine($"Admin's ID : {Admin.Id}");
                Console.WriteLine($"Admin's Password : {Admin.Password}");   
                return response.IsSuccess;       
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        private void AddEmployee()
        {
            try
            {
                Employee Employee = new Employee()
                {
                    Id = Utility.GenerateEmployeeID(),
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

        public void GetTransferFunds(AccountHolder loggedInAccount)
        {
            StringBuilder sb = new StringBuilder();
            AccountHolderService accountHolderService = new AccountHolderService();

            string destinationAccountNumber = Utility.GetStringInput("Enter the destination account number: ", true);
            int transferTypeInput = Convert.ToInt32(Utility.GetStringInput("Enter the transfer type (0 for IMPS, 1 for RTGS): ", true));

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

            AccountHolder destinationAccount = accountHolderService.GetAccountHolderByAccountNumber(destinationAccountNumber);

            if (destinationAccount == null)
            {
                sb.AppendLine(Constants.AccountNotFound);
                return;
            }

            Response<string> transferResponse = this.BankService.TransferFunds(loggedInAccount, destinationAccount, transferAmount, transferType);

            if (transferResponse.IsSuccess)
            {
                sb.AppendLine(transferResponse.Message);
                sb.AppendLine($"New balance: {loggedInAccount.Balance}");
                sb.AppendLine(Constants.TransferFundsSuccess);
            }
            else
            {
                sb.AppendLine(transferResponse.Message);
            }
        }
    }
}

