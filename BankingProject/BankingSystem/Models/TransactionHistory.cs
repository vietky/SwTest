using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Models
{
    public enum BankAction
    {
        Deposit,
        Transfer
    };

    public class TransactionHistory
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Index("IX_TransactionHistories_AccountNumber", IsClustered = false)]
        public long AccountNumber { get; set; }
        public BankAction Action { get; set; }
        public decimal Amount { get; set; }
        public decimal OldBalance { get; set; }
        public decimal NewBalance { get; set; }
        public string Description { get; set; }
        [Index("IX_TransactionHistories_CreatedDate", IsClustered = false)]

        public DateTime CreatedDate
        {
            get
            {
                return createdDate.HasValue ? createdDate.Value : DateTime.UtcNow;
            }
            set
            {
                createdDate = value;
            }
        }

        private DateTime? createdDate = null;
    }
}
