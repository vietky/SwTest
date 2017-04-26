using BankingSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem
{
    public interface IDbContextFactory
    {
        BankDbContext Create();
    }

    public class DbContextFactory : IDbContextFactory
    {
        public BankDbContext Create()
        {
            return new BankDbContext();
        }
    }
}
