using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusTrackBookAPIs.Controllers;
using BusTrackBookAPIs.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace BusTrackBookAPIs.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BusController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public BusController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }
        [HttpGet("cities")]
        public ActionResult<IEnumerable<object>> GetCities()
        {
            List<object> cities = new List<object>();

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();

                    // Updated SQL query
                    string sqlQuery = @"
                SELECT DISTINCT c.id, c.cityname::VARCHAR, c.latitude, c.longitude
                FROM cities c
                JOIN bus_stops bs ON c.id = bs.cityid
                ORDER BY c.cityname ASC";

                    using (NpgsqlCommand command = new NpgsqlCommand(sqlQuery, connection))
                    {
                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var city = new
                                {
                                    cityid = reader.GetInt32(0),
                                    cityname = reader.GetString(1),
                                    latitude = reader.GetDouble(2),
                                    longitude = reader.GetDouble(3),
                                };
                                cities.Add(city);
                            }
                        }
                    }

                    connection.Close();
                }

                return Ok(cities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching cities: {ex.Message}");
            }
        }


        [HttpGet("buses")]
        public async Task<ActionResult<IEnumerable<object>>> GetBusesBetweenStops(int startStop, int endStop, string travelDate)
        {
            List<object> buses = new List<object>();

            try
            {
                // Parse travelDate to DateTime with format 'yyyy-MM-dd'
                if (!DateTime.TryParseExact(travelDate, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
                {
                    return BadRequest("Invalid date format. Please use 'yyyy-MM-dd'.");
                }

                var result = CalculateDistanceAndFare(startStop, endStop);
                using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM fetch_buses_between_stops1(@startStop, @endStop, @travelDate)", connection))
                    {
                        command.Parameters.AddWithValue("@startStop", startStop);
                        command.Parameters.AddWithValue("@endStop", endStop);
                        command.Parameters.AddWithValue("@travelDate", parsedDate);

                        using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var bus = new
                                {
                                    BusId = Convert.ToInt32(reader["busid"]),
                                    BusNumber = reader["busnumber"].ToString(),
                                    RouteId = Convert.ToInt32(reader["routeid"]),
                                    RouteName = reader["routename"].ToString(),
                                    StartStopId = Convert.ToInt32(reader["start_stop_id"]),
                                    StartStop = reader["start_stop"].ToString(),
                                    StartCityId = Convert.ToInt32(reader["start_city_id"]),
                                    EndStopId = Convert.ToInt32(reader["end_stop_id"]),
                                    EndStop = reader["end_stop"].ToString(),
                                    EndCityId = Convert.ToInt32(reader["end_city_id"]),
                                    DriverId = Convert.ToInt32(reader["driverid"]),
                                    DriverName = reader["drivername"].ToString(),
                                    TotalCapacity = Convert.ToInt32(reader["total_capacity"]),
                                    RemainingCapacity = Convert.ToInt64(reader["remaining_capacity"]),
                                    BookedSeats = Convert.ToInt64(reader["booked_seats"]),
                                    StartStopTime = reader["start_stop_time"].ToString(),
                                    EndStopTime = reader["end_stop_time"].ToString(),
                                    ScheduleId = Convert.ToInt32(reader["scheduleid"]),
                                    StartDate = Convert.ToDateTime(reader["start_date"]),
                                    Distance = result.distance,
                                    Fare = result.fare
                                };
                                buses.Add(bus);
                            }
                        }
                    }

                    await connection.CloseAsync();
                }

                return Ok(buses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("busDetails")]
        public async Task<ActionResult<object>> GetBusDetailsById(int busId)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM fetch_bus_details_by_id(@busId)", connection))
                    {
                        command.Parameters.AddWithValue("@busId", busId);

                        using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                var busDetails = new
                                {
                                    BusId = Convert.ToInt32(reader["bus_id"]),
                                    BusNumber = reader["bus_number"].ToString(),
                                    RouteId = Convert.ToInt32(reader["route_id"]),
                                    RouteName = reader["route_name"].ToString(),
                                    StartStopId = Convert.ToInt32(reader["start_stop_id"]),
                                    StartStop = reader["start_stop"].ToString(),
                                    EndStopId = Convert.ToInt32(reader["end_stop_id"]),
                                    EndStop = reader["end_stop"].ToString(),
                                    DriverId = Convert.ToInt32(reader["driver_id"]),
                                    DriverName = reader["driver_name"].ToString(),
                                    TotalCapacity = Convert.ToInt32(reader["total_capacity"]),
                                    RemainingCapacity = Convert.ToInt64(reader["remaining_capacity"]),
                                    BookedSeats = Convert.ToInt64(reader["booked_seats"]),
                                    StartStopTime = reader["start_stop_time"].ToString(),
                                    EndStopTime = reader["end_stop_time"].ToString()
                                };
                                return Ok(busDetails);
                            }
                            else
                            {
                                return NotFound("Bus not found.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        [HttpGet("routedetails/{busId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetRouteDetails(int busId)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM fetch_route_details_by_bus_id(@busId)", connection))
                    {
                        command.Parameters.AddWithValue("@busId", busId);

                        using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            List<object> routeDetails = new List<object>();
                            while (await reader.ReadAsync())
                            {
                                var detail = new
                                {
                                    BusId = reader.GetInt32(reader.GetOrdinal("busid")),
                                    BusNumber = reader["busnumber"].ToString(),
                                    DriverId = reader.GetInt32(reader.GetOrdinal("driverid")),
                                    DriverName = reader["drivername"].ToString(),
                                    RouteId = reader.GetInt32(reader.GetOrdinal("routeid")),
                                    RouteName = reader["routename"].ToString(),
                                    StopId = reader.GetInt32(reader.GetOrdinal("stopid")),
                                    StopName = reader["stopname"].ToString(),
                                    Latitude = reader.GetDouble(reader.GetOrdinal("latitude")),
                                    Longitude = reader.GetDouble(reader.GetOrdinal("longitude")),
                                    SequenceNumber = reader.GetInt32(reader.GetOrdinal("sequencenumber")),
                                    ArrivalTime = reader["arrival_time"].ToString(),
                                    CityId = reader.GetInt32(reader.GetOrdinal("cityid")),
                                    CityName = reader["cityname"].ToString()
                                };
                                routeDetails.Add(detail);
                            }
                            return Ok(routeDetails);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }



        private (double distance, double fare) CalculateDistanceAndFare(int fromCityId, int toCityId)
        {
            try
            {
                double distance = 0;
                double fare = 0;

                // Fetch latitude and longitude for fromCityId
                double fromLatitude = 0;
                double fromLongitude = 0;
                using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand("SELECT latitude, longitude FROM cities WHERE id = @fromCityId", connection))
                    {
                        command.Parameters.AddWithValue("fromCityId", fromCityId);
                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                fromLatitude = reader.GetDouble(0);
                                fromLongitude = reader.GetDouble(1);
                            }
                            else
                            {
                                //return BadRequest("Invalid fromCityId");
                                return (0, 0);
                            }
                        }
                    }

                    // Fetch latitude and longitude for toCityId
                    double toLatitude = 0;
                    double toLongitude = 0;
                    using (NpgsqlCommand command = new NpgsqlCommand("SELECT latitude, longitude FROM cities WHERE id = @toCityId", connection))
                    {
                        command.Parameters.AddWithValue("toCityId", toCityId);
                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                toLatitude = reader.GetDouble(0);
                                toLongitude = reader.GetDouble(1);
                            }
                            else
                            {
                                return (0, 0);
                            }
                        }
                    }

                    // Calculate distance using Haversine formula (assuming Earth's radius is 6371 km)
                    double dLat = ToRadians(toLatitude - fromLatitude);
                    double dLon = ToRadians(toLongitude - fromLongitude);
                    double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                               Math.Cos(ToRadians(fromLatitude)) * Math.Cos(ToRadians(toLatitude)) *
                               Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
                    double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
                    distance = 6371 * c;  // Radius of the Earth in km

                    // Calculate fare (example calculation)
                    fare = distance * 3; // Adjust according to your fare calculation logic

                    connection.Close();
                }

                var result = new
                {
                    distance = Math.Round(distance, 2), // Round to 2 decimal places
                    fare = Math.Round(fare, 2) // Round to 2 decimal places
                };

                return (Math.Round(distance), Math.Round(fare)); // Round to 2 decimal places
            }
            catch (Exception ex)
            {
                return (0, 0);
            }
        }

        private static double ToRadians(double angle)
        {
            return Math.PI * angle / 180.0;
        }





        [HttpGet("seats")]
        public async Task<ActionResult<IEnumerable<object>>> GetSeatsStatus(int busId, DateOnly startDate)
        {
            List<object> seatsStatus = new List<object>();

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM get_seats_status(@busId, @startDate)", connection))
                    {
                        command.Parameters.AddWithValue("@busId", busId);
                        command.Parameters.AddWithValue("@startDate", NpgsqlTypes.NpgsqlDbType.Date).Value = startDate;

                        using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var seatStatus = new
                                {
                                    SeatId = reader.GetInt32(0),
                                    SeatNumber = reader.GetInt32(1),
                                    SeatSection = reader.GetString(2),
                                    IsBooked = reader.GetBoolean(3)
                                };
                                seatsStatus.Add(seatStatus);
                            }
                        }
                    }

                    await connection.CloseAsync();
                }

                return Ok(seatsStatus);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error fetching seats status: {ex.Message}");
            }
        }



        [HttpPost("bookSeats")]
        public async Task<ActionResult> BookSeats([FromBody] BookSeatsRequest request)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var transaction = await connection.BeginTransactionAsync())
                    {
                        foreach (var seat in request.Seats)
                        {
                            // Insert booking record
                            using (NpgsqlCommand command = new NpgsqlCommand(@"
                        INSERT INTO public.bookings 
                        (userid, busid, seatid, totalamount, paymentmethod, transactionid, startcityid, endcityid, startdate, bookingstatus) 
                        VALUES 
                        (@userid, @busid, @seatid, @totalamount, @paymentmethod, @transactionid, @startcityid, @endcityid, @startdate, 'Confirmed')", connection))
                            {
                                command.Parameters.AddWithValue("@userid", request.UserId);
                                command.Parameters.AddWithValue("@busid", request.BusId);
                                command.Parameters.AddWithValue("@seatid", seat.SeatId);
                                command.Parameters.AddWithValue("@totalamount", request.TotalAmount);
                                command.Parameters.AddWithValue("@paymentmethod", request.PaymentMethod);
                                command.Parameters.AddWithValue("@transactionid", request.TransactionId);
                                command.Parameters.AddWithValue("@startcityid", request.StartCityId);
                                command.Parameters.AddWithValue("@endcityid", request.EndCityId);
                                command.Parameters.AddWithValue("@startdate", request.StartDate);

                                await command.ExecuteNonQueryAsync();
                            }
                        }

                        await transaction.CommitAsync();
                    }

                    await connection.CloseAsync();
                }

                return Ok("Seats booked successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error booking seats: {ex.Message}");
            }
        }


    }









}