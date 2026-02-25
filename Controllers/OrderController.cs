using Microsoft.AspNetCore.Mvc;
using TaxiDispatcherSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace TaxiDispatcherSystem.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly TaxiContext _context;

        public OrderController(TaxiContext context)
        {
            _context = context;
        }

        // 1. СТВОРЕННЯ (GET)
        public IActionResult Create()
        {
            // Генеруємо ціну (int автоматично підходить і для double, і для decimal)
            int randomPrice = Random.Shared.Next(50, 301);
            ViewBag.EstimatedPrice = randomPrice;
            return View(new Order());
        }

        // 2. СТВОРЕННЯ (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [FromForm] string OriginAddress,
            [FromForm] string DestinationAddress,
            [FromForm] decimal EstimatedPrice) // <-- Змінив на decimal для точності
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null) return RedirectToAction("Login", "Account");
            int currentUserId = int.Parse(userIdClaim.Value);

            // 1. Створюємо локації
            var origin = new Location { Адреса = OriginAddress, Широта = 0, Довгота = 0 };
            var destination = new Location { Адреса = DestinationAddress, Широта = 0, Довгота = 0 };
            
            _context.Locations.Add(origin);
            _context.Locations.Add(destination);
            await _context.SaveChangesAsync(); // Зберігаємо, щоб отримати ID

            // 2. Створюємо Тариф та Оплату
            var fare = new Fare { БазовийТариф = 50, ЦінаЗаКм = 8 };
            
            // Важливо: переконайся, що у моделі Payment поле Сума має тип decimal або double.
            // Тут ми передаємо decimal.
            var payment = new Payment { Сума = (double)EstimatedPrice, Статус = "ОЧІКУЄ" };
            
            _context.Fares.Add(fare);
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync(); // Зберігаємо, щоб отримати ID

            // 3. Створюємо Замовлення
            var order = new Order
            {
                КлієнтId = currentUserId,
                МісцеВідправленняId = origin.Id,
                МісцеПризначенняId = destination.Id,
                ТарифId = fare.Id,
                ОплатаId = payment.Id,
                Статус = "СТВОРЕНО",
                ЧасСтворення = DateTime.Now
            };

            // Оновлюємо статус клієнта (тільки якщо це Клієнт, а не Водій)
            // Використовуємо User.Find, бо через _context.Clients водія не знайде (різні ролі)
            var user = await _context.Users.FindAsync(currentUserId);
            // Перевіряємо, чи є у користувача властивість ClientStatus (через наслідування)
            // Це трохи складний момент в EF Core TPH, тому спростимо:
            // Спробуємо знайти саме як клієнта
            var client = await _context.Clients.FindAsync(currentUserId);
            if (client != null)
            {
                client.ClientStatus = "AWAITING_DRIVER";
                _context.Update(client);
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return RedirectToAction("Track", new { id = order.Id });
        }

        // 3. ВІДСТЕЖЕННЯ
        public async Task<IActionResult> Track(int id)
        {
            var order = await _context.Orders
                .Include(o => o.МісцеВідправлення)
                .Include(o => o.МісцеПризначення)
                .Include(o => o.ПризначенийВодій)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (order == null) return NotFound();
            return View(order);
        }

        // 4. ПАНЕЛЬ ВОДІЯ
        public async Task<IActionResult> DriverPanel()
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null) return RedirectToAction("Login", "Account");
            int driverId = int.Parse(userIdClaim.Value);

            var currentOrder = await _context.Orders
                .Include(o => o.МісцеПризначення)
                .Include(o => o.МісцеВідправлення) // Адреси
                .Include(o => o.Тариф)
                .Include(o => o.Оплата)
                .FirstOrDefaultAsync(o => o.ПризначенийВодійId == driverId &&
                                        (o.Статус == "ПРИЗНАЧЕНО_ВОДІЯ" || o.Статус == "В_ПРОЦЕСІ"));
            
            ViewBag.DriverId = driverId;
            return View(currentOrder);
        }

        // 5. ПОЧАТИ ПОЇЗДКУ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartRide(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) return NotFound();

            order.Статус = "В_ПРОЦЕСІ";
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(DriverPanel));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(int orderId) 
        {
            if (orderId == 0) return NotFound(); // Перевірка ID

            var order = await _context.Orders
                .Include(o => o.Оплата)
                .Include(o => o.ПризначенийВодій)
                .Include(o => o.Клієнт)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null) return NotFound();
            
            if (order.Оплата != null)
            {
                order.Оплата.Статус = "ОПЛАЧЕНО";
                _context.Payments.Update(order.Оплата);
            }

            order.Статус = "ВИКОНАНО";
            
            // Звільняємо водія
            if (order.ПризначенийВодій != null)
            {
                order.ПризначенийВодій.Доступний = true;
                _context.Drivers.Update(order.ПризначенийВодій);
            }

            // Звільняємо клієнта
            if (order.Клієнт != null)
            {
                order.Клієнт.ClientStatus = "IDLE";
                _context.Update(order.Клієнт);
            }

            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            
            return RedirectToAction(nameof(DriverPanel));
        }
    } 
}