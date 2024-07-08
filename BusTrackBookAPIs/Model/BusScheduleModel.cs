namespace BusTrackBookAPIs.Model
{
    public class BusScheduleModel
    {
        public int ScheduleId { get; set; }
        public int BusId { get; set; }
        public DateTime? StartDate { get; set; }
        public TimeSpan? StartTime { get; set; }
        public DateTime? StartDateTime { get; set; }
        public string BusNumber { get; set; }
        public string DriverName { get; set; }
        public string RouteName { get; set; }
    }
}