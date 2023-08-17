using System.Collections.Generic;

namespace BankApplication.Common
{
    internal class Constants
    {
        //Bank
        public static string BankCreationSuccess = "Bank created successfully.";

        public static string BankCreationFailure = "Bank creation failed.";

        public static string BankNotFound = "Bank not Found";
     
        public static string InvalidAmount = "Invalid amount. It should be greater than zero.";

        public static string EmptyBankInput = "Bank ID cannot be empty.";

        //Deposit
        public static string DepositSuccess = "Deposit successful";

        public static string Deposited = "Deposit";

        //Withdrawal
        public static string WithdrawalSuccess = "Withdrawal successful";

        public static string Withdrawal = "Withdrawal";

        public static string InsufficientFunds = "Insufficient balance. You cannot withdraw more than the available balance";

        //Transfer
        public static string TransferFundsSuccess = "Transferred Funds successfully";

        public static string TransferFunds = "Funds Transfer";

        //Transaction
        public static string TransactionSuccess = "Transaction history retrieved successfully";

        public static string TransactionFailure = "No transaction history found for this account.";

        public static string TransactionRevert = "Transaction reverted successfully.";

        public static string ViewTransactionHistory = "Viewing transaction history for account number: {0}";

        public static string InvalidTransactionInput = "Account number and BankID should not be empty";

        //Employees
        public static string EmployeeCreationSuccess = "Employee created successfully";

        public static string EmployeeCreationFailure = "Employee not created";

        //Accounts 
        public static string AccountCreationSuccess = "Account created successfully";

        public static string AccountCreationFailure = "Account not created";

        public static string AccountDeleted = "Account deleted successfully";

        public static string AccountHolderUpdateSuccess = "Account updated successfully";

        public static string AccountUpdateFailure = "Account not updated.";

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

        // Main Menu Options
        public static readonly List<string> MainMenu = new List<string>
        {
        "Create New Bank",
        "Login as Account Holder",
        "Login as Bank Staff",
        "Exit"
        };


        // Bank Staff Menu Options
        public static readonly List<string> BankStaffOption = new List<string>
        {
            "Create Account Holder",
            "Update Account Holder",
            "Delete Account Holder",
            "Show All Account Holders",
            "Add Currency",
            "Update Service Charges for Same Bank",
            "Update Service Charges for Other Bank",
            "Show Account Holder Transactions",
            "Revert Transaction",
            "Logout"
        };

        //User account Menu Options
        public static readonly List<string> UserAccountOption = new List<string>
        {
            "Deposit",
            "Withdraw",
            "Transfer",
            "Check Balance",
            "Transactions",
            "Logout"
        };



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
