namespace BankApplication.Models
{
    public class AccountHolder : User
    {
        public string AccountNumber { get; set; }

        public string Name { get; set; }

        public decimal Balance { get; set; }

        public string AccountType { get; set; }

    }
}