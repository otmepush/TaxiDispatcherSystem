using TaxiDispatcherSystem.Models;
using System;
using System.Collections.Generic;

namespace TaxiDispatcherSystem.Patterns
{
    // 1. Інтерфейс Спостерігача (IObserver)
    public interface IOrderObserver
    {
        void Update(Order order);
    }

    // 2. Інтерфейс Спостережуваного об'єкта (IObservable)
    public interface IOrderObservable
    {
        void AddObserver(IOrderObserver observer);
        void RemoveObserver(IOrderObserver observer);
        void NotifyObservers(Order order);
    }

    // 3. Конкретний Спостережуваний об'єкт (ConcreteObservable)
    public class OrderStatusTracker : IOrderObservable
    {
        private readonly List<IOrderObserver> _observers = new List<IOrderObserver>();

        public void AddObserver(IOrderObserver observer) => _observers.Add(observer);
        public void RemoveObserver(IOrderObserver observer) => _observers.Remove(observer);

        public void NotifyObservers(Order order)
        {
            foreach (var observer in _observers)
            {
                observer.Update(order);
            }
        }

        // Метод, який буде змінювати статус і відразу сповіщати всіх
        public void ChangeStatus(Order order, string newStatus)
        {
            order.Статус = newStatus;
            NotifyObservers(order);
        }
    }

    // 4. Конкретний Спостерігач 1: Симуляція SMS-сповіщення клієнту
    public class ClientSmsNotifierObserver : IOrderObserver
    {
        public void Update(Order order)
        {
            // Для лабораторної роботи виводимо повідомлення в консоль сервера.
            // У реальному проекті тут був би код відправки SMS.
            Console.WriteLine($"[SMS Клієнту {order.КлієнтId}]: Ваше замовлення #{order.Id} отримало новий статус -> {order.Статус}");
        }
    }

    // 5. Конкретний Спостерігач 2: Системний логер
    public class SystemLogObserver : IOrderObserver
    {
        public void Update(Order order)
        {
            Console.WriteLine($"[СИСТЕМА ЛОГІВ] Час: {DateTime.Now}. Замовлення #{order.Id} змінило статус на {order.Статус}");
        }
    }
}
