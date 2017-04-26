using BankingSystem.Dtos;
using BankingSystem.Models;
using BankingSystem.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Test.Services
{
    [TestClass]
    public class UserServiceTest
    {
        private Mock<IDbContextFactory> _mockFactory;
        private Mock<ILogService> _mockLogService;
        private Mock<BankDbContext> _mockBankDbContext;
        private Mock<DbSet<User>> _mockUserSet;
        private Mock<DbSet<TransactionHistory>> _mockHistorySet;

        public void Init()
        {
            var userList = new List<User>
            {
                new User() { AccountName = "viet", AccountNumber = 123, Balance = 1000, CreatedDate = DateTime.UtcNow, Password = "abc", Id = 1},
                new User() { AccountName = "minh", AccountNumber = 125, Balance = 700, CreatedDate = DateTime.UtcNow, Password = "abc", Id = 2}
            }.AsQueryable();

            _mockBankDbContext = new Mock<BankDbContext>();
            _mockUserSet = new Mock<DbSet<User>>();
            _mockUserSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(userList.Provider);
            _mockUserSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(userList.Expression);
            _mockUserSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(userList.ElementType);
            _mockUserSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(userList.GetEnumerator());
            _mockBankDbContext.Setup(m => m.Users).Returns(_mockUserSet.Object);

            var historyList = new List<TransactionHistory>
            {
                new TransactionHistory() { AccountNumber = 123, Action = BankAction.Deposit, Amount = 1000, CreatedDate = DateTime.UtcNow, Description ="none", NewBalance=1000, OldBalance=0, Id =1},
                new TransactionHistory() { AccountNumber = 125, Action = BankAction.Deposit, Amount = 700, CreatedDate = DateTime.UtcNow, Description ="none", NewBalance=700, OldBalance=0, Id =2}
            }.AsQueryable();
            _mockHistorySet = new Mock<DbSet<TransactionHistory>>();
            _mockHistorySet.As<IQueryable<TransactionHistory>>().Setup(m => m.Provider).Returns(historyList.Provider);
            _mockHistorySet.As<IQueryable<TransactionHistory>>().Setup(m => m.Expression).Returns(historyList.Expression);
            _mockHistorySet.As<IQueryable<TransactionHistory>>().Setup(m => m.ElementType).Returns(historyList.ElementType);
            _mockHistorySet.As<IQueryable<TransactionHistory>>().Setup(m => m.GetEnumerator()).Returns(historyList.GetEnumerator());
            _mockBankDbContext.Setup(m => m.Histories).Returns(_mockHistorySet.Object);

            _mockFactory = new Mock<IDbContextFactory>();
            _mockFactory.Setup(m => m.Create()).Returns(_mockBankDbContext.Object);

            _mockLogService = new Mock<ILogService>();
        }

        [TestMethod]
        public void Create()
        {
            Init();
            var userService = new UserService(_mockFactory.Object, _mockLogService.Object);
            UserDto result = userService.Create(new UserDto()
            {
                AccountName = "son",
                Balance = 100,
                Password = "alo"
            });
            _mockUserSet.Verify(m => m.Add(It.IsAny<User>()), Times.Once);
            _mockHistorySet.Verify(m => m.Add(It.IsAny<TransactionHistory>()), Times.Once);
            _mockBankDbContext.Verify(m => m.SaveChanges(), Times.Once);
            Assert.IsTrue(result.AccountNumber == 3001);
        }

        [TestMethod]
        public void GetByAccountNumber_success()
        {
            Init();
            var userService = new UserService(_mockFactory.Object, _mockLogService.Object);
            UserDto result = userService.GetByAccountNumber(123, "abc");

            Assert.IsTrue(result != null);
            Assert.IsTrue(result.Balance == 1000);
        }

        [TestMethod]
        public void GetByAccountNumber_failure()
        {
            Init();
            var userService = new UserService(_mockFactory.Object, _mockLogService.Object);
            UserDto result = userService.GetByAccountNumber(123, "abc3");

            Assert.IsTrue(result == null);
        }

        [TestMethod]
        public void Transfer_success()
        {
            Init();
            var userService = new UserService(_mockFactory.Object, _mockLogService.Object);
            decimal result = userService.Transfer(125, 123, 300);

            _mockHistorySet.Verify(m => m.Add(It.IsAny<TransactionHistory>()), Times.Exactly(2));
            _mockBankDbContext.Verify(m => m.SaveChanges(), Times.Once);
            Assert.IsTrue(result == 400);
        }

        [TestMethod]
        public void Transfer_failure_wrong_balance()
        {
            Init();
            var userService = new UserService(_mockFactory.Object, _mockLogService.Object);
            try
            {
                userService.Transfer(125, 123, 800);
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType().Name == "ArgumentOutOfRangeException");
            }
        }

        [TestMethod]
        public void Transfer_failure_wrong_account_number_from()
        {
            Init();
            var userService = new UserService(_mockFactory.Object, _mockLogService.Object);
            try
            {
                userService.Transfer(127, 123, 200);
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType().Name == "ArgumentException");
            }
        }

        [TestMethod]
        public void Transfer_wrong_account_number_to()
        {
            Init();
            var userService = new UserService(_mockFactory.Object, _mockLogService.Object);
            try
            {
                userService.Transfer(125, 129, 200);
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType().Name == "ArgumentException");
            }
        }

        [TestMethod]
        public void GetTransactionHistory()
        {
            Init();
            var userService = new UserService(_mockFactory.Object, _mockLogService.Object);
            var result = userService.GetTransactionHistory(125);

            Assert.IsTrue(result.Count() == 1);
        }

        [TestMethod]
        public void Deposit_success()
        {
            Init();
            var userService = new UserService(_mockFactory.Object, _mockLogService.Object);
            decimal result = userService.Deposit(125, 100);

            Assert.IsTrue(result == 800);
        }

        [TestMethod]
        public void Deposit_failure_wrong_account_number()
        {
            Init();
            var userService = new UserService(_mockFactory.Object, _mockLogService.Object);
            try
            {
                userService.Deposit(127, 100);
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType().Name == "ArgumentException");
            }
        }
    }
}
