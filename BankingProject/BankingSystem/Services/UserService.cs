using BankingSystem.Dtos;
using BankingSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Services
{
    public interface IUserService
    {
        UserDto Create(UserDto userDto);
        UserDto GetByAccountNumber(long accountNumber, string password);
        decimal Transfer(long accountNumber1, long accountNumber2, decimal amount);
        IEnumerable<TransactionHistoryDto> GetTransactionHistory(long accountNumber);
        decimal Deposit(long accountNumber, decimal deposit);
    }

    public class UserService : IUserService
    {
        public ILogService Log;
        public IDbContextFactory DbContextFactory;

        public UserService(IDbContextFactory factory, ILogService log)
        {
            DbContextFactory = factory;
            Log = log;
        }

        public UserDto Create(UserDto userDto)
        {
            using (var context = DbContextFactory.Create())
            {
                int maxId = context.Users.Count() + 1;
                int accountNumber = AppConstants.InitialAccountNumber * maxId + 1;
                User user = context.Users.Add(new User()
                {
                    AccountNumber = accountNumber,
                    AccountName = userDto.AccountName,
                    Balance = userDto.Balance,
                    Password = userDto.Password
                });
                context.Histories.Add(new TransactionHistory()
                {
                    AccountNumber = accountNumber,
                    Action = BankAction.Deposit,
                    OldBalance = 0,
                    NewBalance = userDto.Balance,
                    Amount = userDto.Balance,
                    Description = string.Format("Deposited {0}$", userDto.Balance)
                });
                context.SaveChanges();
                return new UserDto()
                {
                    AccountNumber = accountNumber,
                    AccountName = userDto.AccountName,
                    Balance = userDto.Balance,
                    Password = userDto.Password // should encode password
                };
            }
        }

        public UserDto GetByAccountNumber(long accountNumber, string password)
        {
            using (var context = DbContextFactory.Create())
            {
                User user = context.Users.FirstOrDefault(u => u.AccountNumber == accountNumber && u.Password == password);
                if (user == null)
                {
                    return null;
                }
                return new UserDto()
                {
                    AccountName = user.AccountName,
                    AccountNumber = user.AccountNumber,
                    Balance = user.Balance,
                    Password = user.Password
                };
            }
        }

        public decimal Transfer(long accountNumber1, long accountNumber2, decimal amount)
        {
            using (var context = DbContextFactory.Create())
            {
                User userFrom = context.Users.FirstOrDefault(u => u.AccountNumber == accountNumber1);
                if (userFrom == null)
                {
                    throw new ArgumentException(string.Format("Invalid account number {0}", accountNumber1));
                }
                if (userFrom.Balance < amount)
                {
                    throw new ArgumentOutOfRangeException(string.Format("Your balance is not sufficient to make a transfer"));
                }
                User userTo = context.Users.FirstOrDefault(u => u.AccountNumber == accountNumber2);
                if (userTo == null)
                {
                    throw new ArgumentException(string.Format("Invalid account number {0}", accountNumber2));
                }
                decimal userFromBalance = userFrom.Balance;
                decimal userToBalance = userTo.Balance;
                userFrom.Balance -= amount;
                userTo.Balance += amount;

                context.Users.Attach(userFrom);
                context.Entry(userFrom).State = EntityState.Modified;

                context.Users.Attach(userTo);
                context.Entry(userTo).State = EntityState.Modified;

                context.Histories.Add(new TransactionHistory()
                {
                    AccountNumber = userFrom.AccountNumber,
                    Action = BankAction.Transfer,
                    Amount = amount,
                    OldBalance = userFromBalance,
                    NewBalance = userFrom.Balance,
                    Description = string.Format("Account Name {0} sent {1}$ to Account Name {2}", userFrom.AccountName, amount, userTo.AccountName)
                });

                context.Histories.Add(new TransactionHistory()
                {
                    AccountNumber = userTo.AccountNumber,
                    Action = BankAction.Transfer,
                    Amount = amount,
                    OldBalance = userToBalance,
                    NewBalance = userTo.Balance,
                    Description = string.Format("Account Name {0} sent {1}$ to Account Name {2}", userFrom.AccountName, amount, userTo.AccountName)
                });

                try
                {
                    context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    Log.Error(string.Format("A concurrency has been occurred on Account Number {0} who was tranferring {1}$ to Account Number {2} on {3}. Exception: {4}"
                        , accountNumber1, amount, accountNumber2, DateTime.UtcNow, ex));
                    throw new Exception("Unable to make a transfer. Please come back later");
                }
                return userFrom.Balance;
            }
        }

        public IEnumerable<TransactionHistoryDto> GetTransactionHistory(long accountNumber)
        {
            using (var context = DbContextFactory.Create())
            {
                return context.Histories.Where(u => u.AccountNumber == accountNumber)
                    .Select(h => new TransactionHistoryDto()
                    {
                        AccountNumber = h.AccountNumber,
                        Action = h.Action.ToString(),
                        Description = h.Description,
                        Amount = h.Amount,
                        NewBalance = h.NewBalance,
                        OldBalance = h.OldBalance,
                        CreatedDate = h.CreatedDate
                    })
                    .ToList();
            }
        }

        public decimal Deposit(long accountNumber, decimal deposit)
        {
            using (var context = DbContextFactory.Create())
            {
                User user = context.Users.FirstOrDefault(u => u.AccountNumber == accountNumber);
                if (user == null)
                {
                    throw new ArgumentException(string.Format("Invalid account number {0}", accountNumber));
                }
                user.Balance += deposit;
                context.Users.Attach(user);
                context.Entry(user).State = EntityState.Modified;
                try
                {
                    context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    Log.Error(string.Format("A concurrency has been occurred on Account Number {0} who was depositing {1}$ on {2}. Exception: {3}"
                        , accountNumber, deposit, DateTime.UtcNow, ex));
                    throw new Exception("Unable to make a transfer. Please come back later");
                }
                return user.Balance;
            }
        }
    }
}