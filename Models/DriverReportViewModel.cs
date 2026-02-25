namespace TaxiDispatcherSystem.Models
{
    public class DriverReportViewModel
    {
        public int DriverId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string CarInfo { get; set; } = string.Empty;
        public int CompletedRides { get; set; } 
        public double TotalEarnings { get; set; }
    }
}