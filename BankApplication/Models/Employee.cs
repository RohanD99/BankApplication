namespace BankApplication.Models
{
    public class Employee : User
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string BankId { get; set; }

        public string Designation { get; set; }
    }
}
