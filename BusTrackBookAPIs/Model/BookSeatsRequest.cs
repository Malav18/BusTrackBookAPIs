namespace BusTrackBookAPIs.Model
{
    public class BookSeatsRequest
    {
        public int UserId { get; set; }
        public int BusId { get; set; }
        public List<Seat> Seats { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public string TransactionId { get; set; }
        public int StartCityId { get; set; }
        public int EndCityId { get; set; }
        public DateTime StartDate { get; set; }
    }
}