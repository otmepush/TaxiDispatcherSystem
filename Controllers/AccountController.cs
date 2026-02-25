using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaxiDispatcherSystem.Models;

namespace TaxiDispatcherSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly TaxiContext _context;

        public AccountController(TaxiContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string phone, string password)
        {
            // 1. Шукаємо користувача за телефоном і паролем
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Телефон == phone && u.Пароль == password);

            if (user != null)
            {
                // 2. Створюємо "паспорт" користувача (Claims)
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Ім_я ?? "User"),
                    new Claim(ClaimTypes.MobilePhone, user.Телефон),
                    new Claim(ClaimTypes.Role, user.Роль ?? "Client"), // Важливо для ролей!
                    new Claim("UserId", user.Id.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // 3. Видаємо куку (входимо в систему)
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Невірний телефон або пароль";
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
        
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}