using Microsoft.AspNetCore.Mvc;
using TaxiDispatcherSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace TaxiDispatcherSystem.Controllers
{
    [Authorize(Roles = "Dispatcher")]
    public class DispatcherController : Controller
    {
        private readonly TaxiContext _context;

        public DispatcherController(TaxiContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders
                                    .Include(o => o.МісцеВідправлення)
                                    .Include(o => o.МісцеПризначення) 
                                    .Where(o => o.Статус == "СТВОРЕНО") 
                                    .ToListAsync();
            
            var availableDrivers = await _context.Drivers
                                                .Where(d => d.Доступний == true)
                                                .ToListAsync();

            ViewBag.AvailableDrivers = availableDrivers;
            
            return View(orders);
        }

        [HttpPost]
        public async Task<IActionResult> Assign(int orderId, int driverId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            var driver = await _context.Drivers.FindAsync(driverId);

            if (order == null || driver == null)
            {
                return NotFound();
            }

            order.ПризначенийВодійId = driverId;
            order.Статус = "ПРИЗНАЧЕНО_ВОДІЯ";
            driver.Доступний = false; 

            _context.Update(order);
            _context.Update(driver);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> CreateDriver()
        {
            var assignedVehicleIds = await _context.Drivers
                .Where(d => d.АвтомобільId != null)
                .Select(d => d.АвтомобільId)
                .ToListAsync();

            var availableVehicles = await _context.Vehicles
                .Where(v => !assignedVehicleIds.Contains(v.Id))
                .ToListAsync();
                
            ViewBag.AvailableVehicles = new SelectList(availableVehicles, "Id", "НомернийЗнак");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDriver(Driver driver)
        {

            if (ModelState.IsValid)
            {

                driver.Роль = "Driver";
                driver.Доступний = true;

                _context.Drivers.Add(driver);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(driver);
        }

        public async Task<IActionResult> Report()
        {
            var completedOrders = await _context.Orders
                                        .Include(o => o.Оплата)
                                        .Where(o => o.Статус == "ВИКОНАНО")
                                        .ToListAsync();

            ViewBag.TotalRides = completedOrders.Count;
            ViewBag.TotalRevenue = completedOrders.Sum(o => o.Оплата != null ? o.Оплата.Сума : 0);

            return View(completedOrders);
        }
        public async Task<IActionResult> ManageDrivers()
        {
            var drivers = await _context.Drivers.ToListAsync();

            var activeDriverIds = await _context.Orders
                .Where(o => o.Статус == "ПРИЗНАЧЕНО_ВОДІЯ" || o.Статус == "В_ПРОЦЕСІ")
                .Select(o => o.ПризначенийВодійId)
                .ToListAsync();

            ViewBag.ActiveDriverIds = activeDriverIds;
            return View(drivers);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleDriverStatus(int driverId)
        {
            var driver = await _context.Drivers.FindAsync(driverId);
            if (driver != null)
            {
                driver.Доступний = !driver.Доступний;
                _context.Update(driver);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ManageDrivers));
        }
    }
}