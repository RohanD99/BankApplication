namespace BankApplication.Common
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

        public enum TransferOptions
        {
            IMPS,
            RTGS
        }
    }
}
