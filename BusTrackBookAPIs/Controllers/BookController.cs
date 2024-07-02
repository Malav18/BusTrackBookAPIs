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
       
        public List<SeatDetails> selectedSeats { get; set; }
        public string pnr { get; set; }
    }

    public class SeatDetails
    {
        public string name { get; set; }
        public string phoneNumber { get; set; }
        public int seatNumber { get; set; }
        
    }
}
