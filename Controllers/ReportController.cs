using Microsoft.AspNetCore.Mvc;
using TaxiDispatcherSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace TaxiDispatcherSystem.Controllers
{
    [Authorize(Roles = "Dispatcher")]
    public class ReportController : Controller
    {
        private readonly TaxiContext _context;

        public ReportController(TaxiContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> DriverPerformance()
        {
            var report = await _context.Drivers
                .Include(d => d.Автомобіль)
                .Select(driver => new DriverReportViewModel
                {
                    DriverId = driver.Id,
                    FullName = driver.Ім_я + " " + driver.Прізвище,
                    
                    CarInfo = driver.Автомобіль != null 
                            ? $"{driver.Автомобіль.Марка} {driver.Автомобіль.Модель} ({driver.Автомобіль.НомернийЗнак})" 
                            : "Без авто",

                    CompletedRides = _context.Orders
                        .Count(o => o.ПризначенийВодійId == driver.Id && o.Статус == "ВИКОНАНО"),

                    TotalEarnings = _context.Orders
                        .Where(o => o.ПризначенийВодійId == driver.Id && o.Статус == "ВИКОНАНО" && o.Оплата != null)
                        .Sum(o => o.Оплата.Сума)
                })
                .OrderByDescending(r => r.TotalEarnings)
                .ToListAsync();

            return View(report);
        }
        public async Task<IActionResult> Query1_AvailableDrivers()
        {
            var drivers = await _context.Drivers
                                        .Where(d => d.Доступний == true)
                                        .Include(d => d.Автомобіль) 
                                        .ToListAsync();
            ViewBag.QueryTitle = "Всі доступні водії (Статус = Доступний)";
            return View("QueryResultDrivers", drivers);
        }

        public async Task<IActionResult> Query3_TodayRevenue()
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);
            var revenue = await _context.Orders
                                        .Where(o => o.ЧасСтворення >= today &&
                                                    o.ЧасСтворення < tomorrow &&
                                                    o.Оплата != null &&
                                                    o.Оплата.Статус == "ОПЛАЧЕНО")
                                        .SumAsync(o => o.Оплата.Сума);

            ViewBag.QueryTitle = "Запит 3: Загальний дохід за сьогодні";
            ViewBag.Result = $"Загальний дохід: {revenue.ToString("N2")} UAH";
            return View("QueryResultScalar");
        }

        public IActionResult Query6_AddDriver()
        {
            return RedirectToAction("CreateDriver", "Dispatcher");
        }
    }
}