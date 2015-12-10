using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankLibrary
{
    public class Bank<T> where T : Account
    {
        private T[] accounts;

        public string Name { get; private set; }

        public Bank(string name)
        {
            Name = name;
        }
        // метод создания счета
        public void Open(AccountType accountType, decimal sum,
            AccountStateHandler addSumHandler, AccountStateHandler withdrawSumHandler,
            AccountStateHandler calculationHandler, AccountStateHandler closeAccountHandler,
            AccountStateHandler openAccountHandler)
        {
            T newaccount = null;
            switch (accountType)
            {
                    case AccountType.Ordinary:
                           newaccount = new DemandAccount(sum, 1) as T;
                           break;
                    case  AccountType.Deposit:
                           newaccount = new DepositAccount(sum,40) as T;
                           break;
            }

            if (newaccount == null)
            {
               throw new Exception("Ошибка создания счета");
            }
            if (accounts == null)
            {
                accounts = new T[] {newaccount};
            }
            else
            {
                T[] tempAccount = new T[accounts.Length+1];
                for (int i = 0; i < accounts.Length; i++)
                {
                    tempAccount[i] = accounts[i];
                }
                tempAccount[tempAccount.Length - 1] = newaccount;
                accounts = tempAccount;
            }
            // установка обработчиков событий счета
            newaccount.Added += addSumHandler;
            newaccount.Withdrow += withdrawSumHandler;
            newaccount.Closed += closeAccountHandler;
            newaccount.Opened += openAccountHandler;
            newaccount.Calculated += calculationHandler;

            newaccount.OnOpened();

        }
        //добавление средств на счет
        public void Put(decimal sum, int id)
        {
            T account = FindAccount(id);
            if (account == null)
                throw new Exception("Счет не найден");
            account.Put(sum);
        }
        // вывод средств
        public void Withdraw(decimal sum, int id)
        {
            T account = FindAccount(id);
            if (account == null)
                throw new Exception("Счет не найден");
            account.Withdraw(sum);
        }
        // закрытие счета
        public void Close(int id)
        {
            int index;
            T account = FindAccount(id, out index);
            if (account == null)
                throw new Exception("Счет не найден");

            account.Close();

            if (accounts.Length <= 1)
                accounts = null;
            else
            {
                // уменьшаем массив счетов, удаляя из него закрытый счет
                T[] tempAccounts = new T[accounts.Length - 1];
                for (int i = 0; i < accounts.Length; i++)
                {
                    if (i == index)
                        continue;
                    tempAccounts[i] = accounts[i];
                }
                accounts = tempAccounts;
            }
        }

        // начисление процентов по счетам
        public void CalculatePercentage()
        {
            if (accounts == null) // если массив не создан, выходим из метода
                return;
            for (int i = 0; i < accounts.Length; i++)
            {
                T account = accounts[i];
                account.IncrementDays();
                account.Calculate();
            }
        }

        // поиск счета по id
        public T FindAccount(int id)
        {
            for (int i = 0; i < accounts.Length; i++)
            {
                if (accounts[i].Id == id)
                    return accounts[i];
            }
            return null;
        }
        // перегруженная версия поиска счета
        public T FindAccount(int id, out int index)
        {
            for (int i = 0; i < accounts.Length; i++)
            {
                if (accounts[i].Id == id)
                {
                    index = i;
                    return accounts[i];
                }
            }
            index = -1;
            return null;
        }
    }
}
