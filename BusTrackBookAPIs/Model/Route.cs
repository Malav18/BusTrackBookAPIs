namespace BusTrackBookAPIs.Model
{

    public class RouteModel
    {
        public int RouteId { get; set; }
        public string RouteName { get; set; }
        public int StartCityId { get; set; }
        public string? StartCityName { get; set; } = null;
        public int EndCityId { get; set; }
        public string? EndCityName { get; set; } = null;
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public bool IsDeleted { get; set; }
    }



}
