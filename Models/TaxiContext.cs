using Microsoft.EntityFrameworkCore;

namespace TaxiDispatcherSystem.Models
{
    public class TaxiContext : DbContext 
    {
        public TaxiContext(DbContextOptions<TaxiContext> options) : base(options) 
        {
        }

        public TaxiContext() 
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=TaxiDB;Trusted_Connection=True;MultipleActiveResultSets=true");
            }
        }
        
        public DbSet<User> Users { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Dispatcher> Dispatchers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Fare> Fares { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("Users");
            
            modelBuilder.Entity<Order>()
                .HasOne(o => o.МісцеВідправлення)
                .WithMany()
                .HasForeignKey(o => o.МісцеВідправленняId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.МісцеПризначення)
                .WithMany()
                .HasForeignKey(o => o.МісцеПризначенняId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}