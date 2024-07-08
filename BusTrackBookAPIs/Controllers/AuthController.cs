using BusTrackBookAPIs.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

[Route("[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("DefaultConnection");
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLogin model)
    {
        var user = await GetUserByEmailAsync(model.EmailId);

        if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        var token = GenerateJwtToken(user);

        //HttpContext.Session.SetString("UserId", user.UserID.ToString());
        //HttpContext.Session.SetString("Username", user.Username);
        //HttpContext.Session.SetString("UserRole", user.RoleID == 1 ? "User" : "Admin");
        //HttpContext.Session.SetString("UserEmail", user.Email);
        //HttpContext.Session.SetString("UserPhoneNumber", user.PhoneNumber);


        return Ok(new { token });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegister model)
    {
        if (await GetUserByEmailAsync(model.Username) != null)
        {
            return BadRequest("Username is already taken.");
        }

        var user = new User
        {
            Username = model.Username,
            Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
            Email = model.Email,
            PhoneNumber = model.PhoneNumber,
            ProfilePicture = model.ProfilePicture, // This will be null if not provided
            RoleID = 1, // Default to 'User' role
            CreateDate = DateTime.UtcNow,
            IsDeleted = false
        };

        await CreateUserAsync(user);

        return Ok(new { message = "User registered successfully." });
    }


    private async Task<User> GetUserByEmailAsync(string email)
    {
        User user = null;

        using (var connection = new NpgsqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            using (var command = new NpgsqlCommand("SELECT * FROM Users WHERE Email = @Email AND IsDeleted = false", connection))
            {
                command.Parameters.AddWithValue("@Email", email);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        user = new User
                        {
                            UserID = (int)reader["userid"],
                            Username = reader["username"].ToString(),
                            Password = reader["password"].ToString(),
                            Email = reader["email"].ToString(),
                            PhoneNumber = reader["phonenumber"].ToString(),
                            RoleID = (int)reader["roleid"],
                            ProfilePicture = reader["profilepicture"] as string,
                            CreateDate = (DateTime)reader["createdate"],
                            ModifyDate = reader["modifiedate"] as DateTime?,
                            IsDeleted = (bool)reader["isdeleted"]
                        };
                    }
                }
            }
        }

        return user;
    }

    private async Task CreateUserAsync(User user)
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            using (var command = new NpgsqlCommand(
                "INSERT INTO Users (Username, Password, Email, PhoneNumber, RoleID, ProfilePicture, CreateDate, IsDeleted) " +
                "VALUES (@Username, @Password, @Email, @PhoneNumber, @RoleID, @ProfilePicture, @CreateDate, @IsDeleted)", connection))
            {
                command.Parameters.AddWithValue("@Username", user.Username);
                command.Parameters.AddWithValue("@Password", user.Password);
                command.Parameters.AddWithValue("@Email", user.Email);
                command.Parameters.AddWithValue("@PhoneNumber", user.PhoneNumber);
                command.Parameters.AddWithValue("@RoleID", user.RoleID);
                command.Parameters.AddWithValue("@ProfilePicture", (object)user.ProfilePicture ?? DBNull.Value);
                command.Parameters.AddWithValue("@CreateDate", user.CreateDate);
                command.Parameters.AddWithValue("@IsDeleted", user.IsDeleted);

                await command.ExecuteNonQueryAsync();
            }
        }
    }

    //private string GenerateJwtToken(User user)
    //{

    //    var token = new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(
    //     claims: [
    //         new Claim(ClaimTypes.Name, user.Username),
    //         new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
    //         new Claim(ClaimTypes.Role, user.RoleID == 1 ? "User" : "Admin")
    //     ],
    //     expires: DateTime.Now.AddMinutes(20),
    //     signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration.GetSection("Jwt:Key").Get<string>()!)), SecurityAlgorithms.HmacSha256)
    // ));



    //    return token;
    //}
    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);
        var tokenHandler = new JwtSecurityTokenHandler();

        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
        new Claim(ClaimTypes.Role, user.RoleID == 1 ? "User" : "Admin"),
        new Claim(ClaimTypes.Email, user.Email)
    };

        if (!string.IsNullOrEmpty(user.PhoneNumber))
        {
            claims.Add(new Claim("PhoneNumber", user.PhoneNumber)); // Custom claim for phone number
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
          
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        Console.WriteLine(token);
        return tokenHandler.WriteToken(token);
    }



    [HttpPost("ForgotPassword")]
    public async Task<IActionResult> ForgotPassword(string email)
    {
        try
        {
            bool emailExists = CheckEmailExists(email);

            if (!emailExists)
            {
                return BadRequest(new { message = "Email address does not exist." });
            }

            string resetToken = Guid.NewGuid().ToString();
            DateTime resetTokenExpiry = DateTime.UtcNow.AddHours(1); // Example: Token expiry in 1 hour
            SaveResetToken(email, resetToken, resetTokenExpiry);

            string resetPasswordLink = $"http://localhost:4200/auth/resetpassword?email={WebUtility.UrlEncode(email)}&token={resetToken}";

            await SendEmail(email, resetPasswordLink);

            return Ok(new { message = "Password reset link has been sent to your email address." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while processing your request. Please try again later.", error = ex.Message });
        }
    }

    [HttpPost("ResetPassword")]
    public IActionResult ResetPassword(ResetPassword model)
    {
        try
        {
            if (VerifyResetToken(model.Email, model.Token, out DateTime resetTokenExpiry))
            {
                if (resetTokenExpiry < DateTime.UtcNow)
                {
                    return BadRequest(new { message = "Reset token has expired. Please request a new password reset." });
                }

                if (ResetUserPassword(model.Email, model.NewPassword))
                {
                    return Ok(new { message = "Password reset successfully." });
                }
                else
                {
                    return StatusCode(500, new { message = "Failed to reset password." });
                }
            }
            else
            {
                return BadRequest(new { message = "Invalid reset token." });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while resetting the password. Please try again later.", error = ex.Message });
        }
    }

    private bool CheckEmailExists(string email)
    {
        using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();

            using (NpgsqlCommand command = new NpgsqlCommand("SELECT COUNT(*) FROM users WHERE Email = @Email", connection))
            {
                command.Parameters.AddWithValue("@Email", email);
                int count = Convert.ToInt32(command.ExecuteScalar());
                return count > 0;
            }
        }
    }

    private async Task SendEmail(string email, string resetPasswordLink)
    {
        try
        {
            using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587))
            {
                smtpClient.Credentials = new NetworkCredential("malav.amnex@gmail.com", "gicjnlpdlaozachd");
                smtpClient.EnableSsl = true;

                MailMessage mailMessage = new MailMessage("malav.amnex@gmail.com", email)
                {
                    Subject = "AMNEX Reset Password",
                    Body = GetPasswordResetEmailTemplate(resetPasswordLink),
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

    private void SaveResetToken(string email, string resetToken, DateTime resetTokenExpiry)
    {
        using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();

            using (NpgsqlCommand command = new NpgsqlCommand("UPDATE users SET ResetToken = @ResetToken, ResetTokenExpiry = @ResetTokenExpiry WHERE Email = @Email", connection))
            {
                command.Parameters.AddWithValue("@ResetToken", resetToken);
                command.Parameters.AddWithValue("@ResetTokenExpiry", resetTokenExpiry);
                command.Parameters.AddWithValue("@Email", email);
                command.ExecuteNonQuery();
            }
        }
    }

    private bool VerifyResetToken(string email, string token, out DateTime resetTokenExpiry)
    {
        resetTokenExpiry = DateTime.MinValue;

        using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();

            using (NpgsqlCommand command = new NpgsqlCommand("SELECT ResetTokenExpiry FROM users WHERE Email = @Email AND ResetToken = @ResetToken", connection))
            {
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@ResetToken", token);
                object result = command.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    resetTokenExpiry = (DateTime)result;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }

    private bool ResetUserPassword(string email, string newPassword)
    {
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);

        using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();

            using (NpgsqlCommand command = new NpgsqlCommand("UPDATE users SET Password = @Password, ResetToken = NULL, ResetTokenExpiry = NULL WHERE Email = @Email", connection))
            {
                command.Parameters.AddWithValue("@Password", hashedPassword);
                command.Parameters.AddWithValue("@Email", email);
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
    }


    private static string GetPasswordResetEmailTemplate(string resetPasswordLink)
    {
        // Here you can define your HTML email template with placeholders for password
        string template = @"
    <html>
    <head>
        
    </head>
    <body>
        
        <p>You have requested to reset your password. Please click on the following button to reset your password:</p>
        <a href='{0}' style='display: inline-block; padding: 10px 20px; background-color: #007bff; color: #fff; text-decoration: none; border-radius: 5px;'>Reset Password</a>
        <p>Alternatively, you can copy and paste the following link into your browser:</p>
        <p><a href='{0}'>{0}</a></p>
        <p>If you did not request a password reset, please ignore this email.</p>
        <p>Please keep your new password secure and do not share it with anyone.</p>
        <hr>
        <p>Contact Information:</p>
        <p>Name: Malav Vora</p>
        <p>Role: Intern - Integrated</p>
        <p>Phone: (+91)-9106632795</p>
        <p>Website: <a href='http://www.amnex.com'>www.amnex.com</a></p>
        <p>Company: Amnex Infotechnologies Pvt. Ltd.</p>
        <p>Address: Wing-B, 1201/1202/1301, Mondeal Heights, S.G.Highway, Ahmedabad - 380015</p>
    </body>
    </html>";

        return string.Format(template, resetPasswordLink);
    }







    [HttpGet("userDetails/{userId}")]
    public async Task<ActionResult<UserDetails>> GetUserDetails(int userId)
    {
        try
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM fetch_user_details(@userId)", connection))
                {
                    command.Parameters.AddWithValue("@userId", userId);

                    using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            var userDetails = new UserDetails
                            {
                                UserID = reader.GetInt32(reader.GetOrdinal("userid")),
                                Username = reader.GetString(reader.GetOrdinal("username")),
                                Password = reader.GetString(reader.GetOrdinal("password")),
                                Email = reader.GetString(reader.GetOrdinal("email")),
                                PhoneNumber = reader.GetString(reader.GetOrdinal("phonenumber")),
                                RoleID = reader.GetInt32(reader.GetOrdinal("roleid")),
                                Role = reader.GetString(reader.GetOrdinal("role")),
                                ProfilePicture = reader.GetString(reader.GetOrdinal("profilepicture")),
                                CreateDate = reader.GetDateTime(reader.GetOrdinal("createdate")),
                                ModifyDate = reader.IsDBNull(reader.GetOrdinal("modifiedate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("modifiedate")),
                                IsDeleted = reader.GetBoolean(reader.GetOrdinal("isdeleted")),
                                ResetToken = reader.IsDBNull(reader.GetOrdinal("resettoken")) ? null : reader.GetString(reader.GetOrdinal("resettoken")),
                                ResetTokenExpiry = reader.IsDBNull(reader.GetOrdinal("resettokenexpiry")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("resettokenexpiry"))
                            };

                            return Ok(userDetails);
                        }
                        else
                        {
                            return NotFound("User not found.");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error fetching user details: {ex.Message}");
        }
    }

}



