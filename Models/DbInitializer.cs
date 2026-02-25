namespace TaxiDispatcherSystem.Models
{
    public static class DbInitializer
    {
        public static void Initialize(TaxiContext context)
        {
            context.Database.EnsureCreated();

            if (context.Users.Any())
            {
                return;  
            }

            var client1 = new Client { Ім_я = "Анна", Прізвище = "Іванова", Телефон = "0501112233", Роль = "Client", Email = "anna.ivanova@example.com", Пароль = "user"};
            var client2 = new Client { Ім_я = "Сергій", Прізвище = "Петренко", Телефон = "0672223344", Роль = "Client", Email = "sergiy.petrenko@example.com", Пароль = "user" };
            var client3 = new Client { Ім_я = "Ольга", Прізвище = "Сидоренко", Телефон = "0933334455", Роль = "Client", Email = "olga.sidorenko@example.com", Пароль = "user" };
            
            context.Clients.AddRange(client1, client2, client3);

            var car1 = new Vehicle { Марка = "Toyota", Модель = "Camry", НомернийЗнак = "AA1234CE" };
            var car2 = new Vehicle { Марка = "Hyundai", Модель = "Sonata", НомернийЗнак = "AI5678BI" };
            var car3 = new Vehicle { Марка = "Skoda", Модель = "Octavia", НомернийЗнак = "BC9012AA" };

            context.Vehicles.AddRange(car1, car2, car3);
            
            var driver1 = new Driver { Ім_я = "Петро", Прізвище = "Водійко", Телефон = "0991111111", Роль = "Driver", Email = "petro.vodiyko@taxi.com", Пароль = "driver",  Доступний = true, Автомобіль = car1 };
            var driver2 = new Driver { Ім_я = "Іван", Прізвище = "Шумахер", Телефон = "0992222222", Роль = "Driver", Email = "ivan.shymaher@taxi.com", Пароль = "driver", Доступний = true, Автомобіль = car2 };
            var driver3 = new Driver { Ім_я = "Василь", Прізвище = "Гонщик", Телефон = "0993333333", Роль = "Driver", Email = "vasil.gonjik@taxi.com", Пароль = "driver", Доступний = true, Автомобіль = car3 }; 

            context.Drivers.AddRange(driver1, driver2, driver3);

            var dispatcher = new Dispatcher { Ім_я = "Admin", Прізвище = "Admin", Телефон = "0445555555", Роль = "Dispatcher", Email = "admin@taxi.com", Пароль = "admin" };
            context.Dispatchers.Add(dispatcher);
            
            context.SaveChanges(); 

            var locA = new Location { Адреса = "Хрещатик 1" };
            var locB = new Location { Адреса = "Вокзальна 5" };
            var locC = new Location { Адреса = "Прорізна 10" };
            var locD = new Location { Адреса = "Льва Толстого 15" };
            context.Locations.AddRange(locA, locB, locC, locD);

            var fare1 = new Fare { БазовийТариф = 50, ЦінаЗаКм = 8 };
            context.Fares.Add(fare1);

            var orders = new List<Order>();
            for (int i = 0; i < 6; i++)
            {
                var payment = new Payment { Сума = (i + 1) * 50, Статус = (i % 2 == 0) ? "ОПЛАЧЕНО" : "ОЧІКУЄ" };
                context.Payments.Add(payment);
                
                orders.Add(new Order
                {
                    КлієнтId = (i % 3) + 1,
                    ПризначенийВодійId = (i % 2 == 0) ? (i % 3) + 1 : (int?)null,
                    МісцеВідправлення = (i % 2 == 0) ? locA : locC,
                    МісцеПризначення = (i % 2 == 0) ? locB : locD,
                    Тариф = fare1,
                    Оплата = payment,
                    Статус = (i % 2 == 0) ? "ВИКОНАНО" : "СТВОРЕНО",
                    ЧасСтворення = DateTime.Now.AddDays(-i) 
                });
            }
            context.Orders.AddRange(orders);

            context.SaveChanges();
        }
    }
}