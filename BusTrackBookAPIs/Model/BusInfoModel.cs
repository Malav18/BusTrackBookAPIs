public class BusDetails
{
    public int BusId { get; set; }
    public string BusNumber { get; set; }
    public int RouteId { get; set; }
    public string RouteName { get; set; }
    public int StartStopId { get; set; }
    public string StartStop { get; set; }
    public int StartCityId { get; set; }
    public int EndStopId { get; set; }
    public string EndStop { get; set; }
    public int EndCityId { get; set; }
    public int DriverId { get; set; }
    public string DriverName { get; set; }
    public long BookedSeats { get; set; }
    public string StartStopTime { get; set; }
    public string EndStopTime { get; set; }
    public int ScheduleId { get; set; }
    public DateTime StartDate { get; set; }
    public int StartSequenceNumber { get; set; }
    public int EndSequenceNumber { get; set; }
    public double Distance { get; set; }
    public double Fare { get; set; }
    public int TotalCapacity { get; set; } // Add total capacity property
    public int RemainingCapacity { get; set; } // Add remaining capacity property
}
