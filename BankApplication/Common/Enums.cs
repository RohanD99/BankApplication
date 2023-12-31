﻿namespace BankApplication.Common
{
    public class Enums
    {
        public enum MainMenu
        {
            CreateNewBank,
            LoginAsAccountHolder,
            LoginAsBankStaff,
            Exit
        }

        public enum AdminOption
        {
            CreateEmployee,
            UpdateEmployee,
            DeleteEmployee,
            GetEmployees,
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
        }

        public enum BankStaffOption
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
        }

        public enum UserAccountOption
        {
            Deposit,
            Withdraw,
            Transfer,
            CheckBalance,
            Transactions,
            Logout
        }

        public enum UserType
        {
            Admin,
            Employee,
            AccountHolder
        }

        public enum TransactionType
        {
            Deposit,
            Withdraw,
            Transfer,
            Revert
        }

        public enum TransferOptions
        {
            IMPS,
            RTGS
        }
    }
}
