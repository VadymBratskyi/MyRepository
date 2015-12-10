using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankLibrary
{
    public class DemandAccount : Account
    {
        protected internal override event AccountStateHandler Opened;

        public DemandAccount(decimal sum, int percent) : base(sum, percent)
        {
        }

        protected internal override void OnOpened()
        {
            Opened(this, new AccountEventArgs("Открыт новый счет до востребования!Id счета: " + this.Id, _sum));
        }
    }
}
