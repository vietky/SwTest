using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Dtos
{
    public class TransactionHistoryDto
    {
        public long AccountNumber { get; set; }
        public string Action { get; set; }
        public decimal Amount { get; set; }
        public decimal OldBalance { get; set; }
        public decimal NewBalance { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
