using System;
using static BankApplication.Common.Enums;

namespace BankApplication.Models
{
    public class Transaction
    {
        public string Id { get; set; }

        public string SrcBankId { get; set; }

        public string SrcAccount { get; set; }

        public string DstAccount { get; set; }

        public TransactionType Type { get; set; }

        public decimal Amount { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
