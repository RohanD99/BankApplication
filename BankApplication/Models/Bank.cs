using System;

namespace BankApplication.Models
{
    public class Bank
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Location { get; set; }

        public string IFSC { get; set; }

        public float IMPSforSameBank { get; set; }

        public float IMPSforOtherBank { get; set; }

        public float RTGSforSameBank { get; set; }

        public float RTGSforOtherBank { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime ModifiedOn { get; set; }
    }
}
