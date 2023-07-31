using System.Collections.Generic;

namespace BankApplication.Common
{
    internal class Constants
    {
        //Bank
        public static string BankCreation = "Bank created successfully.";

        public static string BankFailure = "Bank creation failed.";
     
        public static string InvalidAmount = "Invalid amount. It should be greater than zero.";

        //Deposit
        public static string DepositSuccess = "Deposit successful";

        public static string Deposit = "Deposit";

        //Withdrawal
        public static string WithdrawalSuccess = "Withdrawal successful";

        public static string Withdrawal = "Withdrawal";

        public static string InsufficientFunds = "Insufficient balance. You cannot withdraw more than the available balance";

        //Transfer
        public static string TransferFundsSuccess = "Transferred Funds successfully";

        public static string TransferFunds = "Funds Transfer";

        //Transaction
        public static string TransactionnSuccess = "Transaction history retrieved successfully";

        public static string TransactionFailure = "No transaction history found for this account.";

        public static string TransactionRevert = "Transaction reverted successfully.";

        public static string ViewTransactionHistory = "Viewing transaction history for account number: {0}";

        //Employees
        public static string EmployeeSuccess = "Employee created successfully";

        public static string EmployeeFailure = "Employee not created";

        //User
        public static string UserSuccess = "User created successfully";

        public static string UserFailure = "User not created";

        public static string UserDeleted = "User deleted";

        public static string UserNotFound = "User not Found";

        public static string UserUpdated = "User updated successfully";

        public static string UserUpdateFailure = "User not updated.";

        //Accounts
        public static string ShowAllAccounts = "List all of accounts";

        public static string AccountNotFound = "Acount not found";

        public static string InvalidType = "Invalid transfer type.";

        public static string ModifiedBy = "Account Modified by";

        //Currency
        public static string NewCurrency = "New currency added successfully";

        public static string InvalidRate = "Invalid exchange rate. Exchange rate should be greater than zero.";

        public static string CurrencyExists = "Currency code already exists.";

        //Service charge
        public static string ServiceChargeForSameAccount = "Adding service charge for same bank account (RTGS and IMPS):";

        public static string ServiceChargeForOtherAccount = "Adding service charge for other bank account (RTGS and IMPS):";

        public static string ServiceChargesUpdated = "Service charges updated";

        //Credentials
        public static string Username = "Enter Username";

        public static string Password = "Enter Password";

        public static string AccountHolderName = "Enter Account holder's name";

        public static string AccountType = "Enter Account type";

        //Styles
        public static string Style = "-----------------------------";

        public static Dictionary<string, decimal> acceptedCurrencies = new Dictionary<string, decimal>()
        {
            { "INR", 1 },
            { "USD", 0.014m },
            { "EUR", 0.012m },
        };
    }
}
