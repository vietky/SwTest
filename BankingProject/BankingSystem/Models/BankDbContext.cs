using System;
using System.Data.Entity;

namespace BankingSystem.Models
{
    public class BankDbContext : DbContext
    {
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<TransactionHistory> Histories { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
