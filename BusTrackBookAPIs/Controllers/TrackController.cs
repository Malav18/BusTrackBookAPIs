using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusTrackBookAPIs.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TrackController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public TrackController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        [HttpGet("ids/{pnr}")]
        public async Task<IActionResult> GetIds(string pnr)
        {
            try
            {
                await using var conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();

                // Query to get scheduleid
                string scheduleQuery = "SELECT scheduleid FROM public.bookings WHERE pnrnumber = @pnr";
                await using var scheduleCmd = new NpgsqlCommand(scheduleQuery, conn);
                scheduleCmd.Parameters.AddWithValue("pnr", pnr);
                var scheduleId = await scheduleCmd.ExecuteScalarAsync() as int?;

                if (scheduleId == null)
                {
                    return NotFound(); // If no scheduleid found for the given pnr
                }

                // Query to get busid based on scheduleid
                string busQuery = "SELECT busid FROM public.bus_schedules WHERE scheduleid = @scheduleId";
                await using var busCmd = new NpgsqlCommand(busQuery, conn);
                busCmd.Parameters.AddWithValue("scheduleId", scheduleId);
                var busId = await busCmd.ExecuteScalarAsync() as int?;

                if (busId == null)
                {
                    return NotFound(); // If no busid found for the given scheduleId
                }

                // Return both scheduleid and busid as JSON response
                return Ok(new { ScheduleId = scheduleId, BusId = busId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{scheduleId}")]
        public IActionResult GetBusLocation(int scheduleId, [FromQuery] DateTime timestamp)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();

                    var query = @"
                        SELECT latitude, longitude
                        FROM public.get_bus_locations(@scheduleId, @timestamp)
                    ";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@scheduleId", scheduleId);
                        command.Parameters.AddWithValue("@timestamp", timestamp);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var result = new Dictionary<string, object>();
                                result["latitude"] = reader.GetDecimal(0);
                                result["longitude"] = reader.GetDecimal(1);
                                return Ok(result);
                            }
                        }
                    }
                }

                return NotFound(); // Return NotFound if no location found
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
