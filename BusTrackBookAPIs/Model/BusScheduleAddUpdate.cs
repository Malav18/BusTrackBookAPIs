namespace BusTrackBookAPIs.Model
{
    public class BusScheduleAddUpdate
    {
        public int? ScheduleId { get; set; }
        public int BusId { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime StartDate => StartDateTime.Date;
        public TimeSpan StartTime => StartDateTime.TimeOfDay;
    }
}