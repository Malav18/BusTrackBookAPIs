using BusTrackBookAPIs.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace BusTrackBookAPIs.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly string _connectionString;

        public BookController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpPost("bookTickets")]
        public async Task<IActionResult> BookTickets([FromBody] PaymentDetails paymentDetails)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                // Call PostgreSQL function to insert booking and seats
                var command = new NpgsqlCommand("SELECT public.insert_booking_and_seats(" +
                    "@p_payment_id, @p_user_id, @p_username, @p_user_email, @p_mode, @p_total, " +
                    "@p_bus_name, @p_schedule_id, @p_bus_number, @p_driver_name, @p_start_city, " +
                    "@p_start_sequence_number, @p_end_sequence_number, @p_end_city, @p_start_date, " +
                    "@p_selected_seats::jsonb, @p_pnr)", connection);

                command.Parameters.AddWithValue("@p_payment_id", paymentDetails.paymentId);
                command.Parameters.AddWithValue("@p_user_id", paymentDetails.userId);
                command.Parameters.AddWithValue("@p_username", paymentDetails.username);
                command.Parameters.AddWithValue("@p_user_email", paymentDetails.userEmail);
                command.Parameters.AddWithValue("@p_mode", paymentDetails.mode);
                command.Parameters.AddWithValue("@p_total", paymentDetails.total);
                command.Parameters.AddWithValue("@p_bus_name", paymentDetails.busName);
                command.Parameters.AddWithValue("@p_schedule_id", paymentDetails.scheduleId);
                command.Parameters.AddWithValue("@p_bus_number", paymentDetails.busNumber);
                command.Parameters.AddWithValue("@p_driver_name", paymentDetails.driverName);
                command.Parameters.AddWithValue("@p_start_city", paymentDetails.startCity);
                command.Parameters.AddWithValue("@p_start_sequence_number", paymentDetails.startSequenceNumber);
                command.Parameters.AddWithValue("@p_end_sequence_number", paymentDetails.endSequenceNumber);
                command.Parameters.AddWithValue("@p_end_city", paymentDetails.endCity);
                command.Parameters.AddWithValue("@p_start_date", paymentDetails.startDate);
                command.Parameters.AddWithValue("@p_selected_seats", JsonSerializer.Serialize(paymentDetails.selectedSeats));
                command.Parameters.AddWithValue("@p_pnr", paymentDetails.pnr);

                await command.ExecuteNonQueryAsync();

                return Ok(new { message = "Booking successful" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to book tickets", message = ex.Message });
            }
        }


        [HttpGet("getUpcomingBookings/{userId}")]
        public async Task<IActionResult> GetUpcomingBookings(int userId)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                var command = new NpgsqlCommand("SELECT * FROM public.get_upcoming_bookings(@p_userid)", connection);
                command.Parameters.AddWithValue("@p_userid", userId);

                using var reader = await command.ExecuteReaderAsync();

                var upcomingBookings = new List<UpcomingBooking>();

                while (await reader.ReadAsync())
                {
                    var seatDetailsJson = reader.GetString(reader.GetOrdinal("seat_details"));
                    var seatDetails = JsonSerializer.Deserialize<List<SeatDetailsModel>>(seatDetailsJson);

                    var booking = new UpcomingBooking
                    {
                        BookingId = reader.GetInt32(reader.GetOrdinal("bookingid")),
                        UserId = reader.GetInt32(reader.GetOrdinal("userid")),
                        ScheduleId = reader.GetInt32(reader.GetOrdinal("scheduleid")),
                        BookingDate = reader.GetDateTime(reader.GetOrdinal("bookingdate")),
                        BookingStatus = reader.GetString(reader.GetOrdinal("bookingstatus")),
                        TotalAmount = reader.GetDecimal(reader.GetOrdinal("totalamount")),
                        PaymentMethod = reader.GetString(reader.GetOrdinal("paymentmethod")),
                        TransactionNumber = reader.GetString(reader.GetOrdinal("transactionnumber")),
                        PnrNumber = reader.GetString(reader.GetOrdinal("pnrnumber")),
                        IsCancelled = reader.GetBoolean(reader.GetOrdinal("iscancelled")),
                        CancelledDate = reader.IsDBNull(reader.GetOrdinal("cancelleddate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("cancelleddate")),
                        CancellationReason = reader.IsDBNull(reader.GetOrdinal("cancellationreason")) ? null : reader.GetString(reader.GetOrdinal("cancellationreason")),
                        RefundAmount = reader.IsDBNull(reader.GetOrdinal("refundamount")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("refundamount")),
                        RefundTransactionId = reader.IsDBNull(reader.GetOrdinal("refundtransactionid")) ? null : reader.GetString(reader.GetOrdinal("refundtransactionid")),
                        CreatedAt = reader.GetDateTime(reader.GetOrdinal("createdat")),
                        ModifiedAt = reader.IsDBNull(reader.GetOrdinal("modifiedat")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("modifiedat")),
                        IsDeleted = reader.GetBoolean(reader.GetOrdinal("isdeleted")),
                        StartSequenceNumber = reader.GetInt32(reader.GetOrdinal("startsequencenumber")),
                        EndSequenceNumber = reader.GetInt32(reader.GetOrdinal("endsequencenumber")),
                        StartDate = reader.GetDateTime(reader.GetOrdinal("startdate")),
                        BusNumber = reader.GetString(reader.GetOrdinal("busnumber")),
                        SeatDetails = seatDetails,
                        RouteName = reader.GetString(reader.GetOrdinal("route_name")),
                        StartCity = reader.GetString(reader.GetOrdinal("start_city")),
                        EndCity = reader.GetString(reader.GetOrdinal("end_city")),
                        StartTime = reader.GetTimeSpan(reader.GetOrdinal("start_time")),
                        StartDateTime = reader.GetDateTime(reader.GetOrdinal("start_date_time"))
                    };

                    upcomingBookings.Add(booking);
                }

                return Ok(upcomingBookings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to retrieve upcoming bookings", message = ex.Message });
            }
        }





        [HttpGet("getPastBookings/{userId}")]
        public  ActionResult<PastBooking> GetPastBookings(int userId)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                 connection.Open();

                var command = new NpgsqlCommand("SELECT * FROM public.get_past_bookings(@p_userid)", connection);
                command.Parameters.AddWithValue("@p_userid", userId);

                using var reader =  command.ExecuteReader();

                var pastBookings = new List<PastBooking>();

                while ( reader.Read())
                {
                    var seatDetailsJson = reader.GetString(reader.GetOrdinal("seat_details"));
                    var seatDetails = JsonSerializer.Deserialize<List<SeatDetailsModel>>(seatDetailsJson);

                    var booking = new PastBooking
                    {
                        BookingId = reader.GetInt32(reader.GetOrdinal("bookingid")),
                        UserId = reader.GetInt32(reader.GetOrdinal("userid")),
                        ScheduleId = reader.GetInt32(reader.GetOrdinal("scheduleid")),
                        BookingDate = reader.GetDateTime(reader.GetOrdinal("bookingdate")),
                        BookingStatus = reader.GetString(reader.GetOrdinal("bookingstatus")),
                        TotalAmount = reader.GetDecimal(reader.GetOrdinal("totalamount")),
                        PaymentMethod = reader.GetString(reader.GetOrdinal("paymentmethod")),
                        TransactionNumber = reader.GetString(reader.GetOrdinal("transactionnumber")),
                        PnrNumber = reader.GetString(reader.GetOrdinal("pnrnumber")),
                        IsCancelled = reader.GetBoolean(reader.GetOrdinal("iscancelled")),
                        CancelledDate = reader.IsDBNull(reader.GetOrdinal("cancelleddate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("cancelleddate")),
                        CancellationReason = reader.IsDBNull(reader.GetOrdinal("cancellationreason")) ? null : reader.GetString(reader.GetOrdinal("cancellationreason")),
                        RefundAmount = reader.IsDBNull(reader.GetOrdinal("refundamount")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("refundamount")),
                        RefundTransactionId = reader.IsDBNull(reader.GetOrdinal("refundtransactionid")) ? null : reader.GetString(reader.GetOrdinal("refundtransactionid")),
                        CreatedAt = reader.GetDateTime(reader.GetOrdinal("createdat")),
                        ModifiedAt = reader.IsDBNull(reader.GetOrdinal("modifiedat")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("modifiedat")),
                        IsDeleted = reader.GetBoolean(reader.GetOrdinal("isdeleted")),
                        StartSequenceNumber = reader.GetInt32(reader.GetOrdinal("startsequencenumber")),
                        EndSequenceNumber = reader.GetInt32(reader.GetOrdinal("endsequencenumber")),
                        StartDate = reader.GetDateTime(reader.GetOrdinal("startdate")),
                        BusNumber = reader.GetString(reader.GetOrdinal("busnumber")),
                        SeatDetails = seatDetails,
                        RouteName = reader.GetString(reader.GetOrdinal("route_name")),
                        StartCity = reader.GetString(reader.GetOrdinal("start_city")),
                        EndCity = reader.GetString(reader.GetOrdinal("end_city")),
                        StartTime = reader.GetTimeSpan(reader.GetOrdinal("start_time")),
                        StartDateTime = reader.GetDateTime(reader.GetOrdinal("start_date_time"))
                    };

                    pastBookings.Add(booking);
                }

                return Ok(pastBookings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to retrieve past bookings", message = ex.Message });
            }
        }
    }

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

        public TimeOnly? startTime { get; set; }
       
        public List<SeatDetails> selectedSeats { get; set; }
        public string pnr { get; set; }
    }

    public class SeatDetails
    {
        public string seatNo { get; set; }
        public string name { get; set; }
        public string phoneNumber { get; set; }
        public int seatNumber { get; set; }
        
    }
}
