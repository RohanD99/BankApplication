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
        static EmployeeService EmployeeService = new EmployeeService();
        public static void BankStaffMenu()
        {
            BankStaffOption option;
            do
            {
                Utility.GenerateOptions(Constants.BankStaffOption);
                option = (BankStaffOption)Convert.ToInt32(Console.ReadLine());
                switch (option)
                {
                    case BankStaffOption.CreateAccountHolder:
                        AccountHolderService.Create();
                        break;

                    case BankStaffOption.UpdateAccountHolder:
                        EmployeeService.UpdateAccountHolder();
                        break;

                    case BankStaffOption.DeleteAccountHolder:
                        Utility.GetStringInput("Enter Account ID to delete account holder: ", true);
                        string accountToDelete = Console.ReadLine();
                        Response<string> deleteResponse = AccountHolderService.Delete(accountToDelete);
                        Console.WriteLine(deleteResponse.Message);
                        break;

                    case BankStaffOption.ShowAllAccountHolders:
                        Response<string> showAllResponse = AccountHolderService.ShowAllAccounts();
                        Console.WriteLine(showAllResponse.Message);
                        Console.WriteLine(showAllResponse.Data);
                        break;

                    case BankStaffOption.AddCurrency:
                        Utility.GetStringInput("Enter Bank ID: ", true);
                        string bankIDForCurrency = Console.ReadLine();
                        Utility.GetStringInput("Enter Currency Code: ", true);
                        string currencyCode = Console.ReadLine();
                        Utility.GetStringInput("Enter Exchange Rate: ", true);
                        decimal exchangeRate = Convert.ToDecimal(Console.ReadLine());
                        AccountHolderService.AddAcceptedCurrency(currencyCode, exchangeRate);
                        break;

                    case BankStaffOption.UpdateServiceChargesForSameBank:
                        Utility.GetStringInput("Enter RTGS Charge for Same Bank: ", true);
                        float rtgsChargeSameBank = Convert.ToSingle(Console.ReadLine());
                        Utility.GetStringInput("Enter IMPS Charge for Same Bank: ", true);
                        float impsChargeSameBank = Convert.ToSingle(Console.ReadLine());
                        AccountHolderService.AddServiceChargeForSameBankAccount(rtgsChargeSameBank, impsChargeSameBank);
                        break;

                    case BankStaffOption.UpdateServiceChargesForOtherBank:
                        Utility.GetStringInput("Enter RTGS Charge for Other Bank: ", true);
                        float rtgsChargeOtherBank = Convert.ToSingle(Console.ReadLine());
                        Utility.GetStringInput("Enter IMPS Charge for Other Bank: ", true);
                        float impsChargeOtherBank = Convert.ToSingle(Console.ReadLine());
                        AccountHolderService.AddServiceChargeForOtherBankAccount(rtgsChargeOtherBank, impsChargeOtherBank);
                        break;

                    case BankStaffOption.ShowAccountHolderTransactions:
                        Console.Write("Enter Account Holder's Account Number: ");
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
