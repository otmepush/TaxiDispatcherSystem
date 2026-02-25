using Microsoft.EntityFrameworkCore;
using TaxiDispatcherSystem.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using TaxiDispatcherSystem.Patterns;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("DbConnection");

if (string.IsNullOrEmpty(connectionString))
{
    connectionString = "Server=(localdb)\\mssqllocaldb;Database=TaxiDB;Trusted_Connection=True;MultipleActiveResultSets=true";
}

builder.Services.AddDbContext<TaxiContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // Куди перенаправляти, якщо не залогінений
        options.AccessDeniedPath = "/Account/AccessDenied"; // Куди, якщо немає прав
    });
builder.Services.AddScoped<OrderStatusTracker>(provider => 
{
    var tracker = new OrderStatusTracker();
    // Підписуємо спостерігачів
    tracker.AddObserver(new ClientSmsNotifierObserver());
    tracker.AddObserver(new SystemLogObserver());
    return tracker;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); 

app.UseRouting();

app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
    
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<TaxiContext>();
        DbInitializer.Initialize(context); // Виклик ініціалізації
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred creating the DB.");
    }
}

app.Run();
