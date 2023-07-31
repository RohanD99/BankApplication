using BankApplication.Common;
using BankApplication.Models;
using BankApplication.Services;
using System;
using System.Collections.Generic;
using System.Linq;using static BankApplication.Common.Enums;

namespace BankApplication.Views
{
    internal class AccountHolderView
    {
        static AccountHolderService AccountHolderService = new AccountHolderService();
        private Action<string> WriteLineDelegate;

        public AccountHolderView(Action<string> writeLineDelegate)
        {
            WriteLineDelegate = writeLineDelegate;
        }
        public static void BankStaffMenu()
        {
            BankStaffOption option;
            do
            {
                List<string> BankStaffMenuOptions = Enum.GetNames(typeof(BankStaffOption)).ToList();
                Utility.GenerateOptions(BankStaffMenuOptions);

                option = (BankStaffOption)Convert.ToInt32(Console.ReadLine());
                switch (option)
                {
                    case BankStaffOption.CreateAccountHolder:
                        BankView bankView = new BankView();
                        AccountHolder newAccountHolder = new AccountHolder();

                        Employee employee = DataStorage.Employees.FirstOrDefault(emp => emp.Type == Enums.UserType.Employee);

                        if (employee != null)
                        {
                            bankView.AddUser(employee);
                        }
                        else
                        {
                            Console.WriteLine("Employee not found.");
                        }

                        break;

                    case BankStaffOption.UpdateAccountHolder:
                        Console.Write("Enter Account ID to update account holder: ");
                        string accountToUpdate = Console.ReadLine();
                        AccountHolder EmployeeToUpdate = DataStorage.Accounts.FirstOrDefault(e => e.Id == accountToUpdate);
                        if (EmployeeToUpdate != null)
                        {
                            AccountHolderService.Update(EmployeeToUpdate);
                        }
                        else
                        {
                            Console.WriteLine("User Accound not found.");
                        }
                        break;

                    case BankStaffOption.DeleteAccountHolder:
                        Console.Write("Enter Account ID to delete account holder: ");
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
                        Console.Write("Enter Bank ID: ");
                        string bankIDForCurrency = Console.ReadLine();
                        Console.Write("Enter Currency Code: ");
                        string currencyCode = Console.ReadLine();
                        Console.Write("Enter Exchange Rate: ");
                        decimal exchangeRate = Convert.ToDecimal(Console.ReadLine());
                        AccountHolderService.AddAcceptedCurrency(currencyCode, exchangeRate);
                        break;

                    case BankStaffOption.UpdateServiceChargesForSameBank:
                        Console.Write("Enter RTGS Charge for Same Bank: ");
                        float rtgsChargeSameBank = Convert.ToSingle(Console.ReadLine());
                        Console.Write("Enter IMPS Charge for Same Bank: ");
                        float impsChargeSameBank = Convert.ToSingle(Console.ReadLine());
                        AccountHolderService.AddServiceChargeForSameBankAccount(rtgsChargeSameBank, impsChargeSameBank);
                        break;

                    case BankStaffOption.UpdateServiceChargesForOtherBank:
                        Console.Write("Enter RTGS Charge for Other Bank: ");
                        float rtgsChargeOtherBank = Convert.ToSingle(Console.ReadLine());
                        Console.Write("Enter IMPS Charge for Other Bank: ");
                        float impsChargeOtherBank = Convert.ToSingle(Console.ReadLine());
                        AccountHolderService.AddServiceChargeForOtherBankAccount(rtgsChargeOtherBank, impsChargeOtherBank);
                        break;

                    case BankStaffOption.ShowAccountHolderTransactions:
                        Console.Write("Enter Account Holder's Account Number: ");
                        string accountNumber = Console.ReadLine();
                        AccountHolder accountToShowTransactions = DataStorage.Accounts.FirstOrDefault(a => a.AccountNumber == accountNumber);
                        if (accountToShowTransactions != null)
                        {
                            AccountHolderService.ViewAccountTransactionHistory(accountToShowTransactions);
                        }
                        else
                        {
                            Console.WriteLine("Account not found.");
                        }
                        break;

                    case BankStaffOption.RevertTransaction:
                        Console.Write("Enter Transaction ID to revert: ");
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
