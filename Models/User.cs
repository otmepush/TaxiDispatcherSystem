namespace TaxiDispatcherSystem.Models
{
    public class User
    {
        public int Id { get; set; } 
        public string? Ім_я { get; set; }
        public string? Прізвище { get; set; }
        public string? Телефон { get; set; }
        public string? Email { get; set; }
        public string? Роль { get; set; }
        public string? Пароль { get; set; }
    }

    public class Client : User
    {
        public string? ClientStatus { get; set; }
    }

    public class Driver : User
    {
        public bool Доступний { get; set; }
        public int? АвтомобільId { get; set; } 
        public virtual Vehicle? Автомобіль { get; set; }
    }
    
    public class Dispatcher : User
    {
    }
}