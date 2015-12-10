using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankLibrary
{
    public abstract class Account : IAccount
    {
        //Событие, возникающее при выводе денег
        protected internal virtual event AccountStateHandler Withdrow;
        // Событие возникающее при добавлении на счет
        protected internal virtual event AccountStateHandler Added;
        // Событие возникающее при открытии счета
        protected internal virtual event AccountStateHandler Opened;
        // Событие возникающее при закрытии счета
        protected internal virtual event AccountStateHandler Closed;
        // Событие возникающее при начислении процентов
        protected internal virtual event AccountStateHandler Calculated;

        protected int _id ;// уникальный id счета
        
        private static int _counter = 0; // статический счетчик
        
        protected decimal _sum;// Переменная для хранения суммы
        
        protected int _percentage; // Переменная для хранения процента 
        
        protected int _days = 0; // время с момента открытия счета


        public Account(decimal sum, int percent)
        {
            _sum = sum;
            _percentage = percent;
            _id = ++_counter;// увеличиваем счетчик и присваиваем его значение id
        }

        // Текущая сумма на счету
        public decimal CurrentSum {
            get { return _sum; }
        }
        //  процент счета
        public int Percentage {
            get { return _percentage; }
        }

         //id
        public int Id {
            get { return _id; }
        }
        // метод, вызываемый после открытия счета
        protected internal abstract void OnOpened();
        // метод добавления средств на счет
        public virtual void Put(decimal sum)
        {
            _sum += sum;
            if (Added!=null)// вызываем событие добавления денег на счет
            {
                Added(this, new AccountEventArgs("На счет поступило " + sum, sum));
            }
        }
        // метод изъятия денег со счета
        public virtual decimal Withdraw(decimal sum)
        {
            decimal result = 0;
            if (sum <= _sum)
            {
                _sum -= sum;
                result = sum;
                if (Withdrow != null)
                {
                    Withdrow(this, new AccountEventArgs("Сумма " + sum + " снята со счета " + _id, sum));
                }
            }
            else
            {
                if (Withdrow != null)
                {
                    Withdrow(this,new AccountEventArgs("Недостаточно денег на счете " + _id, sum));
                }
            }
            return result;
        }


        protected internal virtual void  Close()
        {
            if (Closed != null)
            {
                Closed(this, new AccountEventArgs("Счет " + _id + " закрыт.  Итоговая сумма: " + CurrentSum, CurrentSum));
            }
        }

        // увеличиваем кол-во дней
        protected internal void IncrementDays()
        {
            _days++;
        }
        // метод подсчета процентов
        protected internal virtual void Calculate()
        {
            decimal increment = _sum * _percentage / 100;
            _sum = _sum + increment;
            if (Calculated != null)
                Calculated(this, new AccountEventArgs("Начислены проценты в размере: " + increment, increment));
        }
    }
}
