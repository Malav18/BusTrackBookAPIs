public class TicketBookingDetails
{
    public string userEmail { get; set; }
    public string userId { get; set; }
    public string PNR { get; set; }
    public string PaymentId { get; set; }
    public string Mode { get; set; }
    public decimal Total { get; set; }
    public string BusName { get; set; }
    public string DriverName { get; set; }
    public string StartCity { get; set; }
    public string EndCity { get; set; }
    public DateTime StartDate { get; set; }
    public string StartTime { get; set; } // Change to string if it's in "HH:mm:ss" format
    public List<SeatDetails> SelectedSeats { get; set; }
}

public class SeatDetails
{
    public string SeatNumber { get; set; }
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
}
