using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;

namespace BusTrackBookAPIs.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TicketController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TicketController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("sendticketemail")]
        public async Task<IActionResult> SendTicketEmail([FromBody] TicketBookingDetails bookingDetails)
        {
            try
            {
                // Validate email field
                if (string.IsNullOrEmpty(bookingDetails.userEmail))
                {
                    return BadRequest(new { errors = new { email = new[] { "The email field is required." } } });
                }

                // Construct the HTML message based on the provided layout
                string htmlMessage = ConstructTicketEmail(bookingDetails);

                // Send email
                await SendEmail(bookingDetails.userEmail, "Bus Ticket Booking Confirmation", htmlMessage);

                return Ok(); // Return success response
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return StatusCode(500, $"Failed to send email: {ex.Message}");
            }
        }

        private async Task SendEmail(string email, string subject, string body)
        {
            try
            {
                using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtpClient.Credentials = new NetworkCredential("malav.amnex@gmail.com", "gicjnlpdlaozachd");
                    smtpClient.EnableSsl = true;

                    MailMessage mailMessage = new MailMessage("malav.amnex@gmail.com", email)
                    {
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    };

                    await smtpClient.SendMailAsync(mailMessage);
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception appropriately
                throw new ApplicationException($"Failed to send email to {email}: {ex.Message}", ex);
            }
        }

        private string ConstructTicketEmail(TicketBookingDetails bookingDetails)
        {
            // Construct your HTML email based on the provided layout
            string htmlMessage = $@"
    <html>
    <body style='font-family: Arial, sans-serif;'>
        <h2 style='color: #4CAF50;'>Bus Ticket Booking Confirmation</h2>
        <p>Hi {bookingDetails.SelectedSeats.FirstOrDefault()?.Name},</p>
        <p>Your bus ticket booking has been confirmed. See the details below:</p>
        <table style='border-collapse: collapse; width: 100%;'>
            <tr>
                <th style='text-align: left; padding: 8px; border: 1px solid #ddd;'>PNR No.</th>
                <td style='padding: 8px; border: 1px solid #ddd;'>{bookingDetails.PNR}</td>
            </tr>
            <tr>
                <th style='text-align: left; padding: 8px; border: 1px solid #ddd;'>Bus Name</th>
                <td style='padding: 8px; border: 1px solid #ddd;'>{bookingDetails.BusName}</td>
            </tr>
            <tr>
                <th style='text-align: left; padding: 8px; border: 1px solid #ddd;'>Driver Name</th>
                <td style='padding: 8px; border: 1px solid #ddd;'>{bookingDetails.DriverName}</td>
            </tr>
            <tr>
                <th style='text-align: left; padding: 8px; border: 1px solid #ddd;'>Start City</th>
                <td style='padding: 8px; border: 1px solid #ddd;'>{bookingDetails.StartCity}</td>
            </tr>
            <tr>
                <th style='text-align: left; padding: 8px; border: 1px solid #ddd;'>End City</th>
                <td style='padding: 8px; border: 1px solid #ddd;'>{bookingDetails.EndCity}</td>
            </tr>
            <tr>
                <th style='text-align: left; padding: 8px; border: 1px solid #ddd;'>Start Date</th>
                <td style='padding: 8px; border: 1px solid #ddd;'>{bookingDetails.StartDate.ToString("yyyy-MM-dd")}</td>
            </tr>
            <tr>
                <th style='text-align: left; padding: 8px; border: 1px solid #ddd;'>Start Time</th>
                <td style='padding: 8px; border: 1px solid #ddd;'>{bookingDetails.StartTime}</td>
            </tr>
            <tr>
                <th style='text-align: left; padding: 8px; border: 1px solid #ddd;'>Total Fare</th>
                <td style='padding: 8px; border: 1px solid #ddd;'>₹ {bookingDetails.Total}</td>
            </tr>
            <tr>
                <th style='text-align: left; padding: 8px; border: 1px solid #ddd;'>Payment ID</th>
                <td style='padding: 8px; border: 1px solid #ddd;'>{bookingDetails.PaymentId}</td>
            </tr>
            <tr>
                <th style='text-align: left; padding: 8px; border: 1px solid #ddd;'>Mode of Payment</th>
                <td style='padding: 8px; border: 1px solid #ddd;'>{bookingDetails.Mode}</td>
            </tr>
            <tr>
                <th style='text-align: left; padding: 8px; border: 1px solid #ddd;'>Seats</th>
                <td style='padding: 8px; border: 1px solid #ddd;'>
                    {string.Join("<br>", bookingDetails.SelectedSeats.Select(seat => $"Seat: {seat.SeatNumber}, Name: {seat.Name}, Phone: {seat.PhoneNumber}"))}
                </td>
            </tr>
        </table>
        <p>If you have any questions or need further assistance, please contact us at support@example.com.</p>
        <p>Thank you for choosing our service!</p>
        <p>Best regards,<br>Amnex</p>
    </body>
    </html>";

            return htmlMessage;
        }

    }
}
