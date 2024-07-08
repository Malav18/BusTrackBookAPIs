using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System;
using System.Data;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using BusTrackBookAPIs.Model;
using Microsoft.AspNetCore.Authorization;

namespace BusTrackBookAPIs.Controllers
{
    [Authorize(Policy = "UserOnly")]
    [Route("[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly string _connectionString;

        public ContactController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ContactForm contactForm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new NpgsqlCommand("SELECT AddContactForm(@Name, @Email, @Subject, @Message)", connection))
                    {
                        command.Parameters.AddWithValue("Name", contactForm.Name);
                        command.Parameters.AddWithValue("Email", contactForm.Email);
                        command.Parameters.AddWithValue("Subject", contactForm.Subject);
                        command.Parameters.AddWithValue("Message", contactForm.Message);

                        command.CommandType = CommandType.Text;

                        var result = await command.ExecuteScalarAsync();

                        var message = result.ToString();

                        await SendEmailToTeam(contactForm);
                        await SendEmailToUser(contactForm);

                        return Ok(new { message = message });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error processing message: {ex.Message}" });
            }
        }

        private async Task SendEmailToTeam(ContactForm contactForm)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("malav.amnex@gmail.com", "eomjjdvkotwcvgku"),
                EnableSsl = true,
            };

            var mailMessageToTeam = new MailMessage
            {
                From = new MailAddress(contactForm.Email),
                Subject = contactForm.Subject,
                IsBodyHtml = true,
            };

            // Enhanced HTML body for team email
            string htmlBody = $@"
    <html>
    <head>
        <style>
            body {{ font-family: Arial, sans-serif; margin: 0; padding: 20px; background-color: #f4f4f9; }}
            .container {{ max-width: 600px; margin: 0 auto; padding: 20px; background-color: #fff; border-radius: 8px; box-shadow: 0 0 10px rgba(0, 0, 0, 0.1); }}
            h2 {{ color: #333; }}
            p {{ color: #555; line-height: 1.6; }}
            .footer {{ margin-top: 20px; text-align: center; color: #888; font-size: 12px; }}
        </style>
    </head>
    <body>
        <div class='container'>
            <h2>New Contact Form Submission</h2>
            <p><strong>Name:</strong> {contactForm.Name}</p>
            <p><strong>Email:</strong> {contactForm.Email}</p>
            <p><strong>Subject:</strong> {contactForm.Subject}</p>
            <p><strong>Message:</strong></p>
            <p>{contactForm.Message}</p>
            <div class='footer'>
                <p>This email was generated automatically. Please do not reply to this email.</p>
            </div>
        </div>
    </body>
    </html>
    ";

            mailMessageToTeam.Body = htmlBody;

            mailMessageToTeam.To.Add("malav.amnex@gmail.com");

            await smtpClient.SendMailAsync(mailMessageToTeam);
        }

        private async Task SendEmailToUser(ContactForm contactForm)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("malav.amnex@gmail.com", "eomjjdvkotwcvgku"),
                EnableSsl = true,
            };

            var mailMessageToUser = new MailMessage
            {
                From = new MailAddress("malav.amnex@gmail.com"),
                Subject = "We have received your message",
                IsBodyHtml = true,
            };

            // Enhanced HTML body for user email
            string htmlBody = $@"
    <html>
    <head>
        <style>
            body {{ font-family: Arial, sans-serif; margin: 0; padding: 20px; background-color: #f4f4f9; }}
            .container {{ max-width: 600px; margin: 0 auto; padding: 20px; background-color: #fff; border-radius: 8px; box-shadow: 0 0 10px rgba(0, 0, 0, 0.1); }}
            p {{ color: #555; line-height: 1.6; }}
            .footer {{ margin-top: 20px; text-align: center; color: #888; font-size: 12px; }}
        </style>
    </head>
    <body>
        <div class='container'>
            <p>Dear {contactForm.Name},</p>
            <p>Thank you for reaching out to us. We have received your message and will get back to you soon.</p>
            <p>Here is a copy of your message:</p>
            <blockquote style='background-color: #f9f9f9; padding: 10px; border-left: 4px solid #ccc;'>{contactForm.Message}</blockquote>
            <br/>
            <p>Best regards,<br/>Amnex</p>
            <div class='footer'>
                <p>This email was generated automatically. Please do not reply to this email.</p>
            </div>
        </div>
    </body>
    </html>
    ";

            mailMessageToUser.Body = htmlBody;

            mailMessageToUser.To.Add(contactForm.Email);

            await smtpClient.SendMailAsync(mailMessageToUser);
        }


    }
}
