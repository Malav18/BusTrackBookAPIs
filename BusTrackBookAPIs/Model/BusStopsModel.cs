namespace BusTrackBookAPIs.Model
{
    public class BusStopsModel
    {
        public int StopId { get; set; }
        public string StopName { get; set; }
        public int RouteId { get; set; }
        public int CityId { get; set; }
        public int SequenceNumber { get; set; }
        public TimeSpan? ArrivalTime { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public bool IsDeleted { get; set; }
        public string CityName { get; set; }
    }
}
