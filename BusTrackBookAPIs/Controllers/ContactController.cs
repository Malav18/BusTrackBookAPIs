﻿using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System;
using System.Data;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using BusTrackBookAPIs.Model;

namespace BusTrackBookAPIs.Controllers
{
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

            // Construct HTML body for team email
            string htmlBody = $@"
        <html>
        <body>
            <h2>New Contact Form Submission</h2>
            <p><strong>Name:</strong> {contactForm.Name}</p>
            <p><strong>Email:</strong> {contactForm.Email}</p>
            <p><strong>Subject:</strong> {contactForm.Subject}</p>
            <p><strong>Message:</strong></p>
            <p>{contactForm.Message}</p>
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

            // Construct HTML body for user email
            string htmlBody = $@"
        <html>
        <body>
            <p>Dear {contactForm.Name},</p>
            <p>Thank you for reaching out to us. We have received your message and will get back to you soon.</p>
            <p>Here is a copy of your message:</p>
            <blockquote>{contactForm.Message}</blockquote>
            <br/>
            <p>Best regards,<br/>Amnex</p>
        </body>
        </html>
    ";

            mailMessageToUser.Body = htmlBody;

            mailMessageToUser.To.Add(contactForm.Email);

            await smtpClient.SendMailAsync(mailMessageToUser);
        }

    }
}
