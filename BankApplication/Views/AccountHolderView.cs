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
                BankView bankView = new BankView();
                switch (option)
                {
                    case BankStaffOption.CreateAccountHolder:                      
                        bankView.AddAccountHolder();
                        break;

                    case BankStaffOption.UpdateAccountHolder:
                        string accountToUpdate = Utility.GetStringInput("Enter Account ID to update account holder: ", true);
                        AccountHolderService AccountService = new AccountHolderService();
                        AccountHolder accountHolderToUpdate = AccountService.GetAccountHolderById(accountToUpdate);
                        bankView.UpdateAccountHolder(accountHolderToUpdate);
                        break;

                    case BankStaffOption.DeleteAccountHolder:
                        bankView.DeleteAccountHolder();
                        break;

                    case BankStaffOption.ShowAllAccountHolders:
                        Employee employee = BankService.GetEmployee();
                        Response<string> showAllResponse = AccountHolderService.ShowAllAccounts(employee);
                        Console.WriteLine(showAllResponse.Message);
                        Console.WriteLine(showAllResponse.Data);
                        break;

                    case BankStaffOption.AddCurrency:
                        Console.Write("Enter Currency Code: ");
                        string currencyCode = Console.ReadLine().ToUpper();
                        Console.Write("Enter Exchange Rate: ");
                        decimal exchangeRate;
                        if (!decimal.TryParse(Console.ReadLine(), out exchangeRate))
                        {
                            Console.WriteLine("Invalid exchange rate. Please enter a valid decimal number.");
                        }
                        Response<string> response = AccountHolderService.AddAcceptedCurrency(currencyCode, exchangeRate, loggedInEmployee);
                        break;

                    case BankStaffOption.UpdateServiceChargesForSameBank:
                        Console.Write("Enter RTGS Charge for Same Bank: ");
                        float rtgsChargeSameBank = Convert.ToSingle(Console.ReadLine());
                        Console.Write("Enter IMPS Charge for Same Bank: ");
                        float impsChargeSameBank = Convert.ToSingle(Console.ReadLine());
                        Response<string> updateSameBankChargeResponse = AccountHolderService.AddServiceChargeForSameBankAccount(rtgsChargeSameBank, impsChargeSameBank, loggedInEmployee);
                        break;

                    case BankStaffOption.UpdateServiceChargesForOtherBank:
                        Console.Write("Enter RTGS Charge for Other Bank: ");
                        float rtgsChargeOtherBank = Convert.ToSingle(Console.ReadLine());
                        Console.Write("Enter IMPS Charge for Other Bank: ");
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
    }
}
