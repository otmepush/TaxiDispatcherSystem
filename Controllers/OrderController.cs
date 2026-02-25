using Microsoft.AspNetCore.Mvc;
using TaxiDispatcherSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using TaxiDispatcherSystem.Patterns; // <--- ДОДАТИ ЦЕЙ USING

namespace TaxiDispatcherSystem.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly TaxiContext _context;
        private readonly OrderStatusTracker _orderTracker; // <--- ДОДАТИ ПОЛЕ

        // <--- ОНОВИТИ КОНСТРУКТОР
        public OrderController(TaxiContext context, OrderStatusTracker orderTracker)
        {
            _context = context;
            _orderTracker = orderTracker; 
        }

        // ... код методів Create, Track, DriverPanel залишається без змін ...

        // 5. ПОЧАТИ ПОЇЗДКУ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartRide(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) return NotFound();

            // БУЛО: order.Статус = "В_ПРОЦЕСІ";
            // СТАЛО: Використовуємо патерн Спостерігач
            _orderTracker.ChangeStatus(order, "В_ПРОЦЕСІ");

            _context.Orders.Update(order);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(DriverPanel));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(int orderId) 
        {
            if (orderId == 0) return NotFound();

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

            // БУЛО: order.Статус = "ВИКОНАНО";
            // СТАЛО: Використовуємо патерн Спостерігач
            _orderTracker.ChangeStatus(order, "ВИКОНАНО");
            
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
