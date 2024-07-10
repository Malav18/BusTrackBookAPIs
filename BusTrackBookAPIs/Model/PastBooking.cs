namespace BusTrackBookAPIs.Model
{
    public class PastBooking
    {
        public int BookingId { get; set; }
        public int UserId { get; set; }
        public int ScheduleId { get; set; }
        public DateTime BookingDate { get; set; }
        public string BookingStatus { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public string TransactionNumber { get; set; }
        public string PnrNumber { get; set; }
        public bool IsCancelled { get; set; }
        public DateTime? CancelledDate { get; set; }
        public string CancellationReason { get; set; }
        public decimal? RefundAmount { get; set; }
        public string RefundTransactionId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public bool IsDeleted { get; set; }
        public int StartSequenceNumber { get; set; }
        public int EndSequenceNumber { get; set; }
        public DateTime StartDate { get; set; }
        public string BusNumber { get; set; }
        public List<SeatDetailsModel> SeatDetails { get; set; }
        public string RouteName { get; set; }
        public string StartCity { get; set; }
        public string EndCity { get; set; }
        public TimeSpan StartTime { get; set; }
        public DateTime StartDateTime { get; set; }
    }
}