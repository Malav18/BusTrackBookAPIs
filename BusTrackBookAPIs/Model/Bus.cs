namespace BusTrackBookAPIs.Model
{
    public class Bus
    {
        public int BusId { get; set; }
        public string BusNumber { get; set; }
        public int? RouteId { get; set; }
        public int? DriverId { get; set; }
        public int? Capacity { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public int? Year { get; set; }
        public string CurrentLocation { get; set; }
        public DateTime? LastMaintenanceDate { get; set; }
        public DateTime? NextMaintenanceDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public bool IsDeleted { get; set; }
        public string DriverName { get; set; }
        public string RouteName { get; set; }
    }
}