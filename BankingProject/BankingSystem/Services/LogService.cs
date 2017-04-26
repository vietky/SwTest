using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Services
{
    public interface ILogService
    {
        void Error(string message);
    }

    public class LogService : ILogService
    {
        public void Error(string message)
        {
            Console.WriteLine("Error: {0}", message);
        }
    }
}
