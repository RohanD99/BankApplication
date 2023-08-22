using System;

namespace BankApplication.Models
{
    public class Bank
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Location { get; set; }

        public string IFSC { get; set; }

        public decimal IMPSforSameBank { get; set; }

        public decimal IMPSforOtherBank { get; set; }

        public decimal RTGSforSameBank { get; set; }

        public decimal RTGSforOtherBank { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime ModifiedOn { get; set; }

        public string ModifiedBy { get; set; }
    }
}
