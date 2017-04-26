using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Index("IX_Users_AccountNumber", IsClustered = false, IsUnique = true)]
        public long AccountNumber { get; set; }

        public string AccountName { get; set; }
        public string Password { get; set; }
        public decimal Balance { get; set; }

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

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
