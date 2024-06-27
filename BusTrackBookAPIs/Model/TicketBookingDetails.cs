public class PaymentDetails
{
    public string paymentId { get; set; }
    public int userId { get; set; }
    public string username { get; set; }
    public string userEmail { get; set; }
    public string mode { get; set; }
    public decimal total { get; set; }
    public string busName { get; set; }
    public int scheduleId { get; set; }
    public string busNumber { get; set; }
    public string driverName { get; set; }
    public string startCity { get; set; }
    public int startSequenceNumber { get; set; }
    public int endSequenceNumber { get; set; }
    public string endCity { get; set; }
    public DateTime startDate { get; set; }
    public string startTime { get; set; }
    public List<SeatDetails> selectedSeats { get; set; }
    public string pnr { get; set; }
}

public class SeatDetails
{
    public string name { get; set; }
    public string phoneNumber { get; set; }
    public int seatNumber { get; set; }
}