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

        //Employees
        public static string EmployeeSuccess = "Employee created successfully";

        public static string EmployeeFailure = "Employee not created";

        //Accounts 
        public static string AccountCreationSuccess = "Account created successfully";

        public static string AccountCreationFailure = "Account not created";

        public static string AccountDeleted = "Account deleted";

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
        public static string CreateNewBank = "Create New Bank";
        public static string LoginAsAccountHolder = "Login as Account Holder";
        public static string LoginAsBankStaff = "Login as Bank Staff";
        public static string Exit = "Exit";

        public static readonly List<string> MainMenu = new List<string>
        {
            CreateNewBank,
            LoginAsAccountHolder,
            LoginAsBankStaff,
            Exit
        };

        // Bank Staff Menu Options
        public static string CreateAccountHolder = "Create Account Holder";
        public static string UpdateAccountHolder = "Update Account Holder";
        public static string DeleteAccountHolder = "Delete Account Holder";
        public static string ShowAllAccountHolders = "Show All Account Holders";
        public static string AddCurrency = "Add Currency";
        public static string UpdateServiceChargesForSameBank = "Update Service Charges for Same Bank";
        public static string UpdateServiceChargesForOtherBank = "Update Service Charges for Other Bank";
        public static string ShowAccountHolderTransactions = "Show Account Holder Transactions";
        public static string RevertTransaction = "Revert Transaction";
        public static string Logout = "Logout";

        public static readonly List<string> BankStaffOption = new List<string>
        {
            CreateAccountHolder,
            UpdateAccountHolder,
            DeleteAccountHolder,
            ShowAllAccountHolders,
            AddCurrency,
            UpdateServiceChargesForSameBank,
            UpdateServiceChargesForOtherBank,
            ShowAccountHolderTransactions,
            RevertTransaction,
            Logout
        };

        // User Account Options
        public static string Deposit = "Deposit";
        public static string Withdraw = "Withdraw";
        public static string Transfer = "Transfer";
        public static string CheckBalance = "Check Balance";
        public static string Transactions = "Transactions";
        public static string UserLogout = "Logout";

        public static readonly List<string> UserAccountOption = new List<string>
        {
            Deposit,
            Withdraw,
            Transfer,
            CheckBalance,
            Transactions,
            Logout
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
