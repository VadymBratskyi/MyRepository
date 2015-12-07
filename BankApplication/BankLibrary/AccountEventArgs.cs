using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankLibrary
{
    public delegate void AccountStateHandler(object sender, AccountEventArgs args);

    public class AccountEventArgs
    {
        public string Message { get; private set; }
        public decimal Sum { get; private set; }

        public AccountEventArgs(string ms, decimal sum)
        {
            Message = ms;
            Sum = sum;
        }
    }
}
