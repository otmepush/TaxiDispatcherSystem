namespace TaxiDispatcherSystem.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string? Статус { get; set; }
        public DateTime ЧасСтворення { get; set; }

        public int КлієнтId { get; set; }
        public virtual Client? Клієнт { get; set; }

        public int? ПризначенийВодійId { get; set; }
        public virtual Driver? ПризначенийВодій { get; set; }

        public int МісцеВідправленняId { get; set; }
        public virtual Location? МісцеВідправлення { get; set; }

        public int МісцеПризначенняId { get; set; }
        public virtual Location? МісцеПризначення { get; set; }

        public int ТарифId { get; set; }
        public virtual Fare? Тариф { get; set; }

        public int ОплатаId { get; set; }
        public virtual Payment? Оплата { get; set; }
    }
}