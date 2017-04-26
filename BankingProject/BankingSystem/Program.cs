using BankingSystem.Dtos;
using BankingSystem.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem
{
    public class Program
    {
        public IUserService UserService { get; set; }
        private UserDto user = null;

        public Program()
        {
            user = null;
        }

        private void ShowLoginScreen()
        {
            if (user != null)
            {
                Console.WriteLine("You have been logined as {0}, account number", user.AccountName, user.AccountNumber);
                Console.WriteLine("Press enter to go back to main screen");
                Console.ReadLine();
                return;
            }
            Console.WriteLine("Enter your account number:");
            long accountNumber;
            if (!long.TryParse(Console.ReadLine(), out accountNumber))
            {
                Console.WriteLine("Invalid account number");
                return;
            }
            Console.WriteLine("Enter your password:");
            string password = Console.ReadLine();
            user = UserService.GetByAccountNumber(accountNumber, password);
            if (user != null)
            {
                Console.WriteLine("You have been logined as {0}, account number {1}", user.AccountName, user.AccountNumber);
            }
            else
            {
                Console.WriteLine("Wrong Password. You might have to register an account?");
            }
            Console.ReadLine();
        }

        private void ShowNewUserScreen()
        {
            if (user != null)
            {
                Console.WriteLine("You have been logined as {0}, account number", user.AccountName, user.AccountNumber);
                Console.WriteLine("Press enter to go back to main screen");
                Console.ReadLine();
                return;
            }
            Console.WriteLine("Enter your account name:");
            string accountName = Console.ReadLine();
            Console.WriteLine("Enter your password:");
            string password = Console.ReadLine();
            Console.WriteLine("Enter your balance:");
            decimal balance;
            if (!decimal.TryParse(Console.ReadLine(), out balance))
            {
                balance = 0;
            }
            user = UserService.Create(new UserDto()
            {
                AccountName = accountName,
                Balance = balance,
                Password = password
            });
            Console.WriteLine("You have been logined as {0}, account number {1}", user.AccountName, user.AccountNumber);
            Console.ReadLine();
        }

        private void ShowTransactionScreen()
        {
            if (user == null)
            {
                Console.WriteLine("Please login.");
                Console.ReadLine();
                return;
            }
            Console.WriteLine("All your transaction here:");
            foreach (TransactionHistoryDto history in UserService.GetTransactionHistory(user.AccountNumber))
            {
                Console.WriteLine("Account number {0} {1} {2}$. Old Balance: {3}. New Balance: ${4}. Date: {5}", history.AccountNumber, history.Action, history.Amount, history.OldBalance, history.NewBalance, history.CreatedDate);
            }
            Console.ReadLine();
        }

        private void ShowTransferScreen()
        {
            Console.WriteLine("Enter your friend's account number:");
            long accountNumber;
            if (!long.TryParse(Console.ReadLine(), out accountNumber))
            {
                Console.WriteLine("Invalid account number");
                return;
            }
            Console.WriteLine("Enter your amount of money:");
            decimal money;
            if (!Decimal.TryParse(Console.ReadLine(), out money))
            {
                money = 0;
            }
            try
            {
                user.Balance = UserService.Transfer(user.AccountNumber, accountNumber, money);
            }
            catch (Exception e)
            {
                Console.WriteLine("An error has been occurred: {0}", e.Message);
                Console.ReadLine();
                return;
            }
            Console.WriteLine("Done. Your current balance is {0}", user.Balance);
            Console.ReadLine();
        }

        private void ShowDepositScreen()
        {
            if (user == null)
            {
                Console.WriteLine("Please login.");
                Console.ReadLine();
                return;
            }
            Console.WriteLine("Enter your deposit:");
            decimal deposit;
            if (!decimal.TryParse(Console.ReadLine(), out deposit))
            {
                Console.WriteLine("Please enter a valid number");
                return;
            }
            try
            {
                user.Balance = UserService.Deposit(user.AccountNumber, deposit);
                Console.WriteLine("Done. Your current balance is {0}", user.Balance);
                Console.ReadLine();
                return;
            }
            catch (Exception e)
            {
                Console.WriteLine("An error has been occurred: {0}", e.Message);
                Console.ReadLine();
                return;
            }
        }

        public void Show()
        {
            byte answer = 0;
            do
            {
                Console.Clear();
                Console.WriteLine("Welcome to our banking system. Make your choice here:");
                Console.WriteLine("Choose your choice:");
                Console.WriteLine("1. Login");
                Console.WriteLine("2. Register new account");
                Console.WriteLine("3. Show transaction");
                Console.WriteLine("4. Transfer");
                Console.WriteLine("5. Deposit");
                Console.WriteLine("6. Logout");
                Console.WriteLine("0. Exit");
                if (!byte.TryParse(Console.ReadLine(), out answer))
                {
                    answer = 0;
                }
                Console.Clear();
                switch (answer)
                {
                    case 1:
                        ShowLoginScreen();
                        break;
                    case 2:
                        ShowNewUserScreen();
                        break;
                    case 3:
                        ShowTransactionScreen();
                        break;
                    case 4:
                        ShowTransferScreen();
                        break;
                    case 5:
                        ShowDepositScreen();
                        break;
                    case 6:
                        user = null;
                        Console.WriteLine("You have been logged out.");
                        break;
                }
            }
            while (answer != 0);
            Console.WriteLine("You chose to exit! Thank you");
        }

        static void Main(string[] args)
        {
            var program = new Program();
            program.UserService = new UserService(new DbContextFactory(), new LogService());
            program.Show();
        }
    }
}
