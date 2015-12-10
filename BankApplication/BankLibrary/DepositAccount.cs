using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankLibrary
{
    public class DepositAccount : Account
    {
        protected internal override event AccountStateHandler Added;
        protected internal override event AccountStateHandler Withdrow;
        protected internal override event AccountStateHandler Opened;
       
        public DepositAccount(decimal sum, int percent) : base(sum, percent)
        {
        }

        protected internal override void OnOpened()
        {
            if (Opened != null)
            {
                Opened(this, new AccountEventArgs("Открыт новый депозитный счет! Id счета: " + this.Id, _sum));
            }
        }

        public override void Put(decimal sum)
        {
            if (_days%30 == 0)
            {
                base.Put(sum);
            }
            else if(Added != null)
            {
                Added(this,new AccountEventArgs("На счет можно положить только после 30-ти дневного периода", 0));
            }
        }

        public override decimal Withdraw(decimal sum)
        {
            if (_days % 30 == 0)
            {
                return base.Withdraw(sum);
            }else if (Withdrow != null)
            {
                Withdrow(this, new AccountEventArgs("Вывести средства можно только после 30-ти дневного периода", 0));
            }
            return 0;
        }

        protected internal override void Calculate()
        {
            if (_days % 30 == 0)
                base.Calculate();
        }
    }
}
