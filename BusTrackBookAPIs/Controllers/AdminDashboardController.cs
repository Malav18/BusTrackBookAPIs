using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace BusTrackBookAPIs.Controllers
{
    public class AdminDashboardController : Controller
    {
        private readonly string _connectionString;

        public AdminDashboardController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection"); // Replace with your connection string key
        }

        [HttpGet("counts")]
        public IActionResult GetCounts()
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                // Count users and admins
                var userAdminCountQuery = @"
            SELECT
                COUNT(*) FILTER (WHERE roleid = 1) AS total_user,
                COUNT(*) FILTER (WHERE roleid = 2) AS total_admin
            FROM
                public.users;";

                var scheduledBusCountQuery = @"
            SELECT COUNT(*) AS total_scheduled_bus
            FROM public.bus_schedules
            WHERE start_date >= CURRENT_DATE;"; // Filter for future dates

                var totalBusCountQuery = @"
            SELECT COUNT(*) AS total_bus
            FROM public.buses;";

                int totalUser, totalAdmin, totalScheduledBus, totalBus;

                // Execute userAdminCountQuery
                using (var cmd = new NpgsqlCommand(userAdminCountQuery, connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            totalUser = Convert.ToInt32(reader["total_user"]);
                            totalAdmin = Convert.ToInt32(reader["total_admin"]);
                        }
                        else
                        {
                            return BadRequest("Failed to fetch user and admin counts.");
                        }
                    }
                }

                // Execute scheduledBusCountQuery
                using (var cmd = new NpgsqlCommand(scheduledBusCountQuery, connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            totalScheduledBus = Convert.ToInt32(reader["total_scheduled_bus"]);
                        }
                        else
                        {
                            return BadRequest("Failed to fetch scheduled bus count.");
                        }
                    }
                }

                // Execute totalBusCountQuery
                using (var cmd = new NpgsqlCommand(totalBusCountQuery, connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            totalBus = Convert.ToInt32(reader["total_bus"]);
                        }
                        else
                        {
                            return BadRequest("Failed to fetch total bus count.");
                        }
                    }
                }

                // Create and return response object
                var response = new
                {
                    totalUser,
                    totalAdmin,
                    totalScheduledBus,
                    totalBus
                };

                return Ok(response);
            }
        }
        //[HttpGet("route-bookings")]
        //public IActionResult GetRouteBookings([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        //{
        //    using (var connection = new NpgsqlConnection(_connectionString))
        //    {
        //        connection.Open();

        //        var query = @"
        //SELECT r.routename, COUNT(bs.scheduleid) as bookings
        //FROM public.routes r
        //LEFT JOIN public.buses b ON r.routeid = b.routeid
        //LEFT JOIN public.bus_schedules bs ON b.busid = bs.busid
        //WHERE (@StartDate IS NULL OR bs.start_date >= @StartDate)
        //AND (@EndDate IS NULL OR bs.start_date <= @EndDate)
        //GROUP BY r.routename";

        //        using (var cmd = new NpgsqlCommand(query, connection))
        //        {
        //            cmd.Parameters.AddWithValue("StartDate", (object)startDate ?? DBNull.Value);
        //            cmd.Parameters.AddWithValue("EndDate", (object)endDate ?? DBNull.Value);

        //            using (var reader = cmd.ExecuteReader())
        //            {
        //                var results = new List<object>();
        //                while (reader.Read())
        //                {
        //                    results.Add(new
        //                    {
        //                        name = reader["routename"].ToString(),
        //                        value = Convert.ToInt32(reader["bookings"])
        //                    });
        //                }

        //                return Ok(results);
        //            }
        //        }
        //    }
        //}
        [HttpGet("bookings-over-time")]
        public IActionResult GetBookingsOverTime([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                // Set default date range to the last 10 days if no dates are provided
                startDate ??= DateTime.UtcNow.AddDays(-10);
                endDate ??= DateTime.UtcNow;

                var query = @"
        SELECT DATE(bookingdate) as bookingdate, COUNT(*) as totalbookings
        FROM public.bookings
        WHERE bookingdate >= @StartDate AND bookingdate <= @EndDate
        GROUP BY DATE(bookingdate)
        ORDER BY DATE(bookingdate)";

                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("StartDate", startDate);
                    cmd.Parameters.AddWithValue("EndDate", endDate);

                    using (var reader = cmd.ExecuteReader())
                    {
                        var results = new List<object>();
                        while (reader.Read())
                        {
                            results.Add(new
                            {
                                bookingDate = Convert.ToDateTime(reader["bookingdate"]),
                                totalBookings = Convert.ToInt32(reader["totalbookings"])
                            });
                        }

                        return Ok(results);
                    }
                }
            }
        }
        [HttpGet("users-added-over-time")]
        public IActionResult GetUsersAddedOverTime([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                startDate ??= DateTime.UtcNow.AddDays(-10);
                endDate ??= DateTime.UtcNow;

                var query = @"
            SELECT DATE_TRUNC('day', createdate) AS date, COUNT(userid) AS count
            FROM public.users
            WHERE roleid = 1
              AND createdate >= COALESCE(@StartDate, createdate)
              AND createdate <= COALESCE(@EndDate, NOW())
            GROUP BY DATE_TRUNC('day', createdate)
            ORDER BY DATE_TRUNC('day', createdate)";

                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("StartDate", (object)startDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("EndDate", (object)endDate ?? DBNull.Value);

                    using (var reader = cmd.ExecuteReader())
                    {
                        var results = new List<object>();
                        while (reader.Read())
                        {
                            results.Add(new
                            {
                                date = reader.GetDateTime(0).ToString("yyyy-MM-dd"), // Adjust date format as needed
                                count = reader.GetInt32(1)
                            });
                        }

                        return Ok(results);
                    }
                }
            }
        }
        [HttpGet("route-statistics")]
        public async Task<ActionResult<IEnumerable<object>>> GetRouteStatistics(DateTime? startDate)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                // Call the PostgreSQL function get_route_statistics
                var sql = @"
                SELECT * FROM public.get_route_statistics1(@startDate)
            ";

                using var cmd = new NpgsqlCommand(sql, connection);
                cmd.Parameters.AddWithValue("startDate", (object)startDate ?? DBNull.Value);

                var routeStatistics = new List<object>();

                await using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var routeId = reader.GetInt32(0);
                    var routeName = reader.GetString(1);
                    var busStartDate = reader.GetDateTime(2);
                    var tripsCompleted = reader.GetInt64(3);
                    var totalBookedSeats = reader.GetInt64(4);
                    var totalPayments = reader.GetDecimal(5);

                    // Create an anonymous object to hold the result
                    var result = new
                    {
                        RouteId = routeId,
                        RouteName = routeName,
                        BusStartDate = busStartDate,
                        TripsCompleted = tripsCompleted,
                        TotalBookedSeats = totalBookedSeats,
                        TotalPayments = totalPayments
                    };

                    routeStatistics.Add(result);
                }

                return Ok(routeStatistics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


    }
}

