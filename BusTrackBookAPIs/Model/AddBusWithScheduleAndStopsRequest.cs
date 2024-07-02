namespace BusTrackBookAPIs.Model
{
    public class AddBusWithScheduleAndStopsRequest
    {
        public string BusNumber { get; set; }
        public int DriverId { get; set; }
        public int Capacity { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public string CurrentLocation { get; set; }
        public DateTime LastMaintenanceDate { get; set; }
        public DateTime NextMaintenanceDate { get; set; }
        public string RouteName { get; set; }
        public int StartCityId { get; set; }
        public int EndCityId { get; set; }
        public DateTime ScheduleStartDate { get; set; }
        public TimeSpan ScheduleStartTime { get; set; }
        public List<BusStop> Stops { get; set; }
    }

    public class BusStop
    {
        public string StopName { get; set; }
        public int CityId { get; set; }
        public int SequenceNumber { get; set; }
        public TimeSpan ArrivalTime { get; set; }
    }
}