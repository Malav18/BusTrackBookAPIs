using BusTrackBookAPIs.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Npgsql;
using System.Data;

namespace BusTrackBookAPIs.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [Route("[controller]")]
    [ApiController]
    public class AdminController : Controller
    {

        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public AdminController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Driver>>> GetAllDrivers()
        {
            var drivers = new List<Driver>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new NpgsqlCommand("SELECT * FROM drivers", connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            drivers.Add(new Driver
                            {
                                DriverId = reader.GetInt32(reader.GetOrdinal("driverid")),
                                DriverName = reader.GetString(reader.GetOrdinal("drivername")),
                                PhoneNumber = reader.IsDBNull(reader.GetOrdinal("phonenumber")) ? null : reader.GetString(reader.GetOrdinal("phonenumber")),
                                DrivingLicenseNumber = reader.GetString(reader.GetOrdinal("drivinglicensenumber")),
                                DateOfBirth = reader.IsDBNull(reader.GetOrdinal("dateofbirth")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("dateofbirth")),
                                AddressStreet = reader.IsDBNull(reader.GetOrdinal("addressstreet")) ? null : reader.GetString(reader.GetOrdinal("addressstreet")),
                                AddressCity = reader.IsDBNull(reader.GetOrdinal("addresscity")) ? null : reader.GetString(reader.GetOrdinal("addresscity")),
                                AddressState = reader.IsDBNull(reader.GetOrdinal("addressstate")) ? null : reader.GetString(reader.GetOrdinal("addressstate")),
                                AddressZipCode = reader.IsDBNull(reader.GetOrdinal("addresszipcode")) ? null : reader.GetString(reader.GetOrdinal("addresszipcode")),
                                EmergencyContact = reader.IsDBNull(reader.GetOrdinal("emergencycontact")) ? null : reader.GetString(reader.GetOrdinal("emergencycontact")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("createdat")),
                                ModifiedAt = reader.IsDBNull(reader.GetOrdinal("modifiedat")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("modifiedat")),
                                IsDeleted = reader.GetBoolean(reader.GetOrdinal("isdeleted"))
                            });
                        }
                    }
                }
            }

            return Ok(drivers);
        }

        [HttpGet("getbusdrivers")]
        public async Task<ActionResult<IEnumerable<Driver>>> GetBusDrivers()
        {
            var drivers = new List<Driver>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new NpgsqlCommand("SELECT * FROM drivers", connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            drivers.Add(new Driver
                            {
                                DriverId = reader.GetInt32(reader.GetOrdinal("driverid")),
                                DriverName = reader.GetString(reader.GetOrdinal("drivername")),
                                PhoneNumber = reader.IsDBNull(reader.GetOrdinal("phonenumber")) ? null : reader.GetString(reader.GetOrdinal("phonenumber")),
                                DrivingLicenseNumber = reader.GetString(reader.GetOrdinal("drivinglicensenumber")),
                                DateOfBirth = reader.IsDBNull(reader.GetOrdinal("dateofbirth")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("dateofbirth")),
                                AddressStreet = reader.IsDBNull(reader.GetOrdinal("addressstreet")) ? null : reader.GetString(reader.GetOrdinal("addressstreet")),
                                AddressCity = reader.IsDBNull(reader.GetOrdinal("addresscity")) ? null : reader.GetString(reader.GetOrdinal("addresscity")),
                                AddressState = reader.IsDBNull(reader.GetOrdinal("addressstate")) ? null : reader.GetString(reader.GetOrdinal("addressstate")),
                                AddressZipCode = reader.IsDBNull(reader.GetOrdinal("addresszipcode")) ? null : reader.GetString(reader.GetOrdinal("addresszipcode")),
                                EmergencyContact = reader.IsDBNull(reader.GetOrdinal("emergencycontact")) ? null : reader.GetString(reader.GetOrdinal("emergencycontact")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("createdat")),
                                ModifiedAt = reader.IsDBNull(reader.GetOrdinal("modifiedat")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("modifiedat")),
                                IsDeleted = reader.GetBoolean(reader.GetOrdinal("isdeleted"))
                            });
                        }
                    }
                }
            }

            return Ok(drivers);

           
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Driver>> GetDriverById(int id)
        {
            Driver driver = null;

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new NpgsqlCommand("SELECT * FROM drivers WHERE driverid = @DriverId", connection))
                {
                    command.Parameters.AddWithValue("DriverId", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            driver = new Driver
                            {
                                DriverId = reader.GetInt32(reader.GetOrdinal("driverid")),
                                DriverName = reader.GetString(reader.GetOrdinal("drivername")),
                                PhoneNumber = reader.IsDBNull(reader.GetOrdinal("phonenumber")) ? null : reader.GetString(reader.GetOrdinal("phonenumber")),
                                DrivingLicenseNumber = reader.GetString(reader.GetOrdinal("drivinglicensenumber")),
                                DateOfBirth = reader.IsDBNull(reader.GetOrdinal("dateofbirth")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("dateofbirth")),
                                AddressStreet = reader.IsDBNull(reader.GetOrdinal("addressstreet")) ? null : reader.GetString(reader.GetOrdinal("addressstreet")),
                                AddressCity = reader.IsDBNull(reader.GetOrdinal("addresscity")) ? null : reader.GetString(reader.GetOrdinal("addresscity")),
                                AddressState = reader.IsDBNull(reader.GetOrdinal("addressstate")) ? null : reader.GetString(reader.GetOrdinal("addressstate")),
                                AddressZipCode = reader.IsDBNull(reader.GetOrdinal("addresszipcode")) ? null : reader.GetString(reader.GetOrdinal("addresszipcode")),
                                EmergencyContact = reader.IsDBNull(reader.GetOrdinal("emergencycontact")) ? null : reader.GetString(reader.GetOrdinal("emergencycontact")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("createdat")),
                                ModifiedAt = reader.IsDBNull(reader.GetOrdinal("modifiedat")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("modifiedat")),
                                IsDeleted = reader.GetBoolean(reader.GetOrdinal("isdeleted"))
                            };
                        }
                    }
                }
            }

            if (driver == null)
            {
                return NotFound();
            }

            return Ok(driver);
        }

        [HttpPost]
        public async Task<ActionResult> AddDriver([FromBody] Driver driver)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new NpgsqlCommand("INSERT INTO drivers (drivername, phonenumber, drivinglicensenumber, dateofbirth, addressstreet, addresscity, addressstate, addresszipcode, emergencycontact, createdat, isdeleted) VALUES (@DriverName, @PhoneNumber, @DrivingLicenseNumber, @DateOfBirth, @AddressStreet, @AddressCity, @AddressState, @AddressZipCode, @EmergencyContact, @CreatedAt, @IsDeleted) RETURNING driverid", connection))
                {
                    command.Parameters.AddWithValue("@DriverName", driver.DriverName);
                    command.Parameters.AddWithValue("@PhoneNumber", driver.PhoneNumber ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@DrivingLicenseNumber", driver.DrivingLicenseNumber);
                    command.Parameters.AddWithValue("@DateOfBirth", driver.DateOfBirth ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@AddressStreet", driver.AddressStreet ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@AddressCity", driver.AddressCity ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@AddressState", driver.AddressState ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@AddressZipCode", driver.AddressZipCode ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@EmergencyContact", driver.EmergencyContact ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);
                    command.Parameters.AddWithValue("@IsDeleted", false);
                    driver.DriverId = (int)await command.ExecuteScalarAsync();
                }
            }

            return CreatedAtAction(nameof(GetDriverById), new { id = driver.DriverId }, driver);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateDriver(int id, [FromBody] Driver driver)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new NpgsqlCommand("UPDATE drivers SET drivername = @DriverName, phonenumber = @PhoneNumber, drivinglicensenumber = @DrivingLicenseNumber, dateofbirth = @DateOfBirth, addressstreet = @AddressStreet, addresscity = @AddressCity, addressstate = @AddressState, addresszipcode = @AddressZipCode, emergencycontact = @EmergencyContact, modifiedat = @ModifiedAt WHERE driverid = @DriverId", connection))
                {
                    command.Parameters.AddWithValue("DriverId", id);
                    command.Parameters.AddWithValue("DriverName", driver.DriverName);
                    command.Parameters.AddWithValue("PhoneNumber", driver.PhoneNumber ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("DrivingLicenseNumber", driver.DrivingLicenseNumber);
                    command.Parameters.AddWithValue("@DateOfBirth", driver.DateOfBirth.HasValue ? driver.DateOfBirth.Value : (object)DBNull.Value);
                    command.Parameters.AddWithValue("AddressStreet", driver.AddressStreet ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("AddressCity", driver.AddressCity ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("AddressState", driver.AddressState ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("AddressZipCode", driver.AddressZipCode ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("EmergencyContact", driver.EmergencyContact ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("ModifiedAt", DateTime.UtcNow);

                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    if (rowsAffected == 0)
                    {
                        return NotFound();
                    }
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteDriver(int id)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new NpgsqlCommand("UPDATE drivers SET isdeleted = TRUE WHERE driverid = @DriverId", connection))
                {
                    command.Parameters.AddWithValue("DriverId", id);

                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    if (rowsAffected == 0)
                    {
                        return NotFound();
                    }
                }
            }

            return NoContent();
        }



        [HttpPost("add-bus-with-schedule-and-stops")]
        public async Task<IActionResult> AddBusWithScheduleAndStops([FromBody] AddBusWithScheduleAndStopsRequest request)
        {
            using (var connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var command = new NpgsqlCommand("SELECT add_bus_with_schedule_and_stops(@busnumber, @driverid, @capacity, @manufacturer, @model, @year, @currentlocation, @lastmaintenancedate, @nextmaintenancedate, @routename, @startcityid, @endcityid, @schedule_start_date, @schedule_start_time, @stops)", connection);

                        command.Parameters.AddWithValue("busnumber", request.BusNumber);
                        command.Parameters.AddWithValue("driverid", request.DriverId);
                        command.Parameters.AddWithValue("capacity", request.Capacity);
                        command.Parameters.AddWithValue("manufacturer", request.Manufacturer);
                        command.Parameters.AddWithValue("model", request.Model);
                        command.Parameters.AddWithValue("year", request.Year);
                        command.Parameters.AddWithValue("currentlocation", request.CurrentLocation);
                        command.Parameters.AddWithValue("lastmaintenancedate", request.LastMaintenanceDate);
                        command.Parameters.AddWithValue("nextmaintenancedate", request.NextMaintenanceDate);
                        command.Parameters.AddWithValue("routename", request.RouteName);
                        command.Parameters.AddWithValue("startcityid", request.StartCityId);
                        command.Parameters.AddWithValue("endcityid", request.EndCityId);
                        command.Parameters.AddWithValue("schedule_start_date", request.ScheduleStartDate);
                        command.Parameters.AddWithValue("schedule_start_time", request.ScheduleStartTime);
                        command.Parameters.AddWithValue("stops", JsonConvert.SerializeObject(request.Stops));

                        command.CommandType = CommandType.Text;
                        await command.ExecuteNonQueryAsync();

                        await transaction.CommitAsync();
                        return Ok();
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        return StatusCode(500, ex.Message);
                    }
                }
            }
        }

        [HttpGet("getcities")]
        public ActionResult<IEnumerable<City>> GetCities()
        {
            List<City> cities = new List<City>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = new NpgsqlCommand("SELECT id, cityname, latitude, longitude FROM public.cities", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var city = new City
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                CityName = reader["cityname"].ToString(),
                                Latitude = Convert.ToDouble(reader["latitude"]),
                                Longitude = Convert.ToDouble(reader["longitude"])
                            };
                            cities.Add(city);
                        }
                    }
                }
            }

            return Ok(cities);
        }

        [HttpGet("Routes/{routeId}/BusStops")]
        public async Task<ActionResult<IEnumerable<BusStopsModel>>> GetBusStops(int routeId)
        {
            List<BusStopsModel> busStops = new List<BusStopsModel>();

            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string sql = @"
                    SELECT bs.*, c.cityname 
                    FROM bus_stops bs 
                    INNER JOIN cities c ON bs.cityid = c.id 
                    WHERE bs.routeid = @routeid";

                using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@routeid", routeId);

                    using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            BusStopsModel busStop = new BusStopsModel
                            {
                                StopId = reader.GetInt32(reader.GetOrdinal("stopid")),
                                StopName = reader.GetString(reader.GetOrdinal("stopname")),
                                RouteId = reader.GetInt32(reader.GetOrdinal("routeid")),
                                CityId = reader.GetInt32(reader.GetOrdinal("cityid")),
                                SequenceNumber = reader.GetInt32(reader.GetOrdinal("sequencenumber")),
                                ArrivalTime = reader.IsDBNull(reader.GetOrdinal("arrival_time")) ? (TimeSpan?)null : reader.GetTimeSpan(reader.GetOrdinal("arrival_time")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("createdat")),
                                ModifiedAt = reader.IsDBNull(reader.GetOrdinal("modifiedat")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("modifiedat")),
                                IsDeleted = reader.GetBoolean(reader.GetOrdinal("isdeleted")),
                                CityName = reader.GetString(reader.GetOrdinal("cityname"))
                            };

                            busStops.Add(busStop);
                        }
                    }
                }
            }

            return busStops;

        }

        [HttpGet("getroutes")]
        public async Task<ActionResult<IEnumerable<RouteModel>>> GetRoutes()
        {
            List<RouteModel> routes = new List<RouteModel>();

            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM public.get_routes()", connection))
                {
                    using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            RouteModel route = new RouteModel
                            {
                                RouteId = reader.GetInt32(reader.GetOrdinal("routeid")),
                                RouteName = reader.GetString(reader.GetOrdinal("routename")),
                                StartCityId = reader.GetInt32(reader.GetOrdinal("startcityid")),
                                EndCityId = reader.GetInt32(reader.GetOrdinal("endcityid")),
                                StartCityName = reader.GetString(reader.GetOrdinal("startcityname")),
                                EndCityName = reader.GetString(reader.GetOrdinal("endcityname")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("createdat")),
                                ModifiedAt = reader.IsDBNull(reader.GetOrdinal("modifiedat")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("modifiedat")),
                                IsDeleted = reader.GetBoolean(reader.GetOrdinal("isdeleted"))
                            };

                            routes.Add(route);
                        }
                    }
                }
            }

            return routes;
        }


        [HttpGet("getroutesbyid/{id}")]
        public async Task<ActionResult<IEnumerable<RouteModel>>> GetRoutesById(int id)
        {
            List<RouteModel> routes = new List<RouteModel>();

            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Use parameterized query to prevent SQL injection
                using (NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM public.get_routes() WHERE routeid = @routeid", connection))
                {
                    // Add the parameter for the route ID
                    command.Parameters.AddWithValue("@routeid", id);

                    using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            RouteModel route = new RouteModel
                            {
                                RouteId = reader.GetInt32(reader.GetOrdinal("routeid")),
                                RouteName = reader.GetString(reader.GetOrdinal("routename")),
                                StartCityId = reader.GetInt32(reader.GetOrdinal("startcityid")),
                                EndCityId = reader.GetInt32(reader.GetOrdinal("endcityid")),
                                StartCityName = reader.GetString(reader.GetOrdinal("startcityname")),
                                EndCityName = reader.GetString(reader.GetOrdinal("endcityname")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("createdat")),
                                ModifiedAt = reader.IsDBNull(reader.GetOrdinal("modifiedat")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("modifiedat")),
                                IsDeleted = reader.GetBoolean(reader.GetOrdinal("isdeleted"))
                            };

                            routes.Add(route);
                        }
                    }
                }
            }

            return routes;
        }

        [HttpPost("addroutes")]
        public async Task<ActionResult<RouteModel>> PostRoute(RouteModel route)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return validation errors if ModelState is not valid
            }

            // Example: Inserting into database
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string sql = "INSERT INTO routes (routename, startcityid, endcityid) " +
                             "VALUES (@routename, @startcityid, @endcityid)";

                using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@routename", route.RouteName);
                    command.Parameters.AddWithValue("@startcityid", route.StartCityId);
                    command.Parameters.AddWithValue("@endcityid", route.EndCityId);

                    await command.ExecuteNonQueryAsync();
                }
            }

            return Ok();
        }


        [HttpDelete("deleteroute/{id}")]
        public async Task<IActionResult> DeleteRoute(int id)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string sql = "SELECT public.delete_route(@routeid)";

                using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@routeid", id);

                    await command.ExecuteNonQueryAsync();
                }
            }

            return NoContent();
        }

        [HttpPut("editroute")]
        public async Task<IActionResult> PutRoute(RouteModel route)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string sql = "SELECT public.edit_route(@routeid, @routename, @startcityid, @endcityid, @isdeleted)";

                using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@routeid", route.RouteId);
                    command.Parameters.AddWithValue("@routename", route.RouteName);
                    command.Parameters.AddWithValue("@startcityid", route.StartCityId);
                    command.Parameters.AddWithValue("@endcityid", route.EndCityId);
                    command.Parameters.AddWithValue("@isdeleted", route.IsDeleted);

                    await command.ExecuteNonQueryAsync();
                }
            }

            return NoContent();
        }



        [HttpPut("UpdateBusStop")]
        public async Task<IActionResult> UpdateBusStop(List<BusStopsModel> busStops)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    foreach (var busStop in busStops)
                    {
                        bool isExists = await CheckBusStopExists(conn, busStop.StopId, busStop.RouteId);

                        if (isExists)
                        {
                            // Perform update
                            await UpdateBusStopInDatabase(conn, busStop);
                        }
                        else
                        {
                            // Perform insert
                            await InsertBusStopIntoDatabase(conn, busStop);
                        }
                    }

                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private async Task<bool> CheckBusStopExists(NpgsqlConnection conn, int stopId, int routeId)
        {
            string query = "SELECT COUNT(*) FROM public.bus_stops WHERE stopid = @p_stopid and routeid = @p_routeid";

            using (var cmd = new NpgsqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@p_stopid", stopId);
                cmd.Parameters.AddWithValue("@p_routeid", routeId);
                var result = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(result) > 0;
            }
        }

        private async Task InsertBusStopIntoDatabase(NpgsqlConnection conn, BusStopsModel busStop)
        {
            string query = @"
                INSERT INTO public.bus_stops ( stopname, routeid, cityid, sequencenumber, arrival_time, modifiedat)
                VALUES ( @p_stopname, @p_routeid, @p_cityid, @p_sequencenumber, @p_arrival_time, @p_modifiedat);
            ";

            using (var cmd = new NpgsqlCommand(query, conn))
            {
                //cmd.Parameters.AddWithValue("@p_stopid", busStop.StopId);
                cmd.Parameters.AddWithValue("@p_stopname", busStop.StopName);
                cmd.Parameters.AddWithValue("@p_routeid", busStop.RouteId);
                cmd.Parameters.AddWithValue("@p_cityid", busStop.CityId);
                cmd.Parameters.AddWithValue("@p_sequencenumber", busStop.SequenceNumber);
                cmd.Parameters.AddWithValue("@p_arrival_time", busStop.ArrivalTime ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@p_modifiedat", busStop.ModifiedAt ?? (object)DBNull.Value);

                await cmd.ExecuteNonQueryAsync();
            }
        }

        private async Task UpdateBusStopInDatabase(NpgsqlConnection conn, BusStopsModel busStop)
        {
            string query = @"
                UPDATE public.bus_stops
                SET 
                    stopname = @p_stopname,
                    routeid = @p_routeid,
                    cityid = @p_cityid,
                    sequencenumber = @p_sequencenumber,
                    arrival_time = @p_arrival_time,
                    modifiedat = @p_modifiedat
                WHERE 
                    stopid = @p_stopid;
            ";

            using (var cmd = new NpgsqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@p_stopid", busStop.StopId);
                cmd.Parameters.AddWithValue("@p_stopname", busStop.StopName);
                cmd.Parameters.AddWithValue("@p_routeid", busStop.RouteId);
                cmd.Parameters.AddWithValue("@p_cityid", busStop.CityId);
                cmd.Parameters.AddWithValue("@p_sequencenumber", busStop.SequenceNumber);
                cmd.Parameters.AddWithValue("@p_arrival_time", busStop.ArrivalTime ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@p_modifiedat", busStop.ModifiedAt ?? (object)DBNull.Value);

                await cmd.ExecuteNonQueryAsync();
            }
        }

        [HttpDelete("deleteBusStops")]
        public async Task<IActionResult> DeleteBusStops([FromBody] Dictionary<string, List<int>> request)
        {
            // Check if request is null or does not contain necessary keys
            if (request == null || !request.ContainsKey("stopIds") || request["stopIds"].Count == 0)
            {
                return BadRequest(new { message = "Invalid request. Provide a valid list of StopIds." });
            }

            List<int> stopIds = request["stopIds"];

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "DELETE FROM bus_stops WHERE stopid = ANY(@stopids)";
                    cmd.Parameters.AddWithValue("stopids", stopIds);
                    int result = await cmd.ExecuteNonQueryAsync();

                    if (result > 0)
                    {
                        return Ok(new { message = "Bus stop(s) deleted successfully." });
                    }
                    else
                    {
                        return NotFound(new { message = "Bus stop(s) not found." });
                    }
                }
            }
        }


        [HttpGet("getallbuses")]
        public async Task<ActionResult<IEnumerable<Bus>>> GetAllBuses()
        {
            var buses = new List<Bus>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = @"
            SELECT 
                b.busid, b.busnumber, b.routeid, b.driverid, b.capacity, 
                b.manufacturer, b.model, b.year, b.currentlocation, 
                b.lastmaintenancedate, b.nextmaintenancedate, b.createdat, 
                b.modifiedat, b.isdeleted, 
                d.drivername, r.routename
            FROM buses b
            LEFT JOIN drivers d ON b.driverid = d.driverid
            LEFT JOIN routes r ON b.routeid = r.routeid
            WHERE b.isdeleted = FALSE";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            buses.Add(new Bus
                            {
                                BusId = reader.GetInt32(reader.GetOrdinal("busid")),
                                BusNumber = reader.GetString(reader.GetOrdinal("busnumber")),
                                RouteId = reader.IsDBNull(reader.GetOrdinal("routeid")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("routeid")),
                                DriverId = reader.IsDBNull(reader.GetOrdinal("driverid")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("driverid")),
                                Capacity = reader.IsDBNull(reader.GetOrdinal("capacity")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("capacity")),
                                Manufacturer = reader.IsDBNull(reader.GetOrdinal("manufacturer")) ? null : reader.GetString(reader.GetOrdinal("manufacturer")),
                                Model = reader.IsDBNull(reader.GetOrdinal("model")) ? null : reader.GetString(reader.GetOrdinal("model")),
                                Year = reader.IsDBNull(reader.GetOrdinal("year")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("year")),
                                CurrentLocation = reader.IsDBNull(reader.GetOrdinal("currentlocation")) ? null : reader.GetString(reader.GetOrdinal("currentlocation")),
                                LastMaintenanceDate = reader.IsDBNull(reader.GetOrdinal("lastmaintenancedate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("lastmaintenancedate")),
                                NextMaintenanceDate = reader.IsDBNull(reader.GetOrdinal("nextmaintenancedate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("nextmaintenancedate")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("createdat")),
                                ModifiedAt = reader.IsDBNull(reader.GetOrdinal("modifiedat")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("modifiedat")),
                                IsDeleted = reader.GetBoolean(reader.GetOrdinal("isdeleted")),
                                DriverName = reader.IsDBNull(reader.GetOrdinal("drivername")) ? null : reader.GetString(reader.GetOrdinal("drivername")),
                                RouteName = reader.IsDBNull(reader.GetOrdinal("routename")) ? null : reader.GetString(reader.GetOrdinal("routename"))
                            });
                        }
                    }
                }
            }

            return Ok(buses);
        }





        [HttpGet("getbusbyid/{id}")]
        public async Task<ActionResult<Bus>> GetBusById(int id)
        {
            Bus bus = null;

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = @"
            SELECT 
                b.busid, b.busnumber, b.routeid, b.driverid, b.capacity, 
                b.manufacturer, b.model, b.year, b.currentlocation, 
                b.lastmaintenancedate, b.nextmaintenancedate, b.createdat, 
                b.modifiedat, b.isdeleted, 
                d.drivername, r.routename
            FROM buses b
            LEFT JOIN drivers d ON b.driverid = d.driverid
            LEFT JOIN routes r ON b.routeid = r.routeid
            WHERE b.busid = @BusId AND b.isdeleted = FALSE";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("BusId", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            bus = new Bus
                            {
                                BusId = reader.GetInt32(reader.GetOrdinal("busid")),
                                BusNumber = reader.GetString(reader.GetOrdinal("busnumber")),
                                RouteId = reader.IsDBNull(reader.GetOrdinal("routeid")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("routeid")),
                                DriverId = reader.IsDBNull(reader.GetOrdinal("driverid")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("driverid")),
                                Capacity = reader.IsDBNull(reader.GetOrdinal("capacity")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("capacity")),
                                Manufacturer = reader.IsDBNull(reader.GetOrdinal("manufacturer")) ? null : reader.GetString(reader.GetOrdinal("manufacturer")),
                                Model = reader.IsDBNull(reader.GetOrdinal("model")) ? null : reader.GetString(reader.GetOrdinal("model")),
                                Year = reader.IsDBNull(reader.GetOrdinal("year")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("year")),
                                CurrentLocation = reader.IsDBNull(reader.GetOrdinal("currentlocation")) ? null : reader.GetString(reader.GetOrdinal("currentlocation")),
                                LastMaintenanceDate = reader.IsDBNull(reader.GetOrdinal("lastmaintenancedate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("lastmaintenancedate")),
                                NextMaintenanceDate = reader.IsDBNull(reader.GetOrdinal("nextmaintenancedate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("nextmaintenancedate")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("createdat")),
                                ModifiedAt = reader.IsDBNull(reader.GetOrdinal("modifiedat")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("modifiedat")),
                                IsDeleted = reader.GetBoolean(reader.GetOrdinal("isdeleted")),
                                DriverName = reader.IsDBNull(reader.GetOrdinal("drivername")) ? null : reader.GetString(reader.GetOrdinal("drivername")),
                                RouteName = reader.IsDBNull(reader.GetOrdinal("routename")) ? null : reader.GetString(reader.GetOrdinal("routename"))
                            };
                        }
                    }
                }
            }

            if (bus == null)
            {
                return NotFound();
            }

            return Ok(bus);
        }





        [HttpPost("addbus")]
        public async Task<ActionResult> AddBus([FromBody] Bus bus)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new NpgsqlCommand("INSERT INTO buses (busnumber, routeid, driverid, capacity, manufacturer, model, year, currentlocation, lastmaintenancedate, nextmaintenancedate, createdat, isdeleted) VALUES (@BusNumber, @RouteId, @DriverId, @Capacity, @Manufacturer, @Model, @Year, @CurrentLocation, @LastMaintenanceDate, @NextMaintenanceDate, @CreatedAt, @IsDeleted) RETURNING busid", connection))
                {
                    command.Parameters.AddWithValue("@BusNumber", bus.BusNumber);
                    command.Parameters.AddWithValue("@RouteId", bus.RouteId ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@DriverId", bus.DriverId ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Capacity", bus.Capacity ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Manufacturer", bus.Manufacturer ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Model", bus.Model ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Year", bus.Year ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@CurrentLocation", bus.CurrentLocation ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@LastMaintenanceDate", bus.LastMaintenanceDate ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@NextMaintenanceDate", bus.NextMaintenanceDate ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);
                    command.Parameters.AddWithValue("@IsDeleted", false);

                    bus.BusId = (int)await command.ExecuteScalarAsync();
                }
            }

            return CreatedAtAction(nameof(GetBusById), new { id = bus.BusId }, bus);
        }




        [HttpPut("updatebus/{id}")]
        public async Task<ActionResult> UpdateBus(int id, [FromBody] Bus bus)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new NpgsqlCommand("UPDATE buses SET busnumber = @BusNumber, routeid = @RouteId, driverid = @DriverId, capacity = @Capacity, manufacturer = @Manufacturer, model = @Model, year = @Year, currentlocation = @CurrentLocation, lastmaintenancedate = @LastMaintenanceDate, nextmaintenancedate = @NextMaintenanceDate, modifiedat = @ModifiedAt WHERE busid = @BusId", connection))
                {
                    command.Parameters.AddWithValue("BusId", id);
                    command.Parameters.AddWithValue("BusNumber", bus.BusNumber);
                    command.Parameters.AddWithValue("RouteId", bus.RouteId ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("DriverId", bus.DriverId ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("Capacity", bus.Capacity ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("Manufacturer", bus.Manufacturer ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("Model", bus.Model ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("Year", bus.Year ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("CurrentLocation", bus.CurrentLocation ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("LastMaintenanceDate", bus.LastMaintenanceDate ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("NextMaintenanceDate", bus.NextMaintenanceDate ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("ModifiedAt", DateTime.UtcNow);

                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    if (rowsAffected == 0)
                    {
                        return NotFound();
                    }
                }
            }

            return NoContent();
        }



        [HttpDelete("deletebus/{id}")]
        public async Task<ActionResult> DeleteBus(int id)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new NpgsqlCommand("UPDATE buses SET isdeleted = TRUE WHERE busid = @BusId", connection))
                {
                    command.Parameters.AddWithValue("BusId", id);

                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    if (rowsAffected == 0)
                    {
                        return NotFound();
                    }
                }
            }

            return NoContent();
        }


















        [HttpGet("getallschedules")]
        public async Task<ActionResult<IEnumerable<BusScheduleModel>>> GetAllSchedules()
        {
            var schedules = new List<BusScheduleModel>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = @"
                    SELECT bs.*, b.busnumber, d.drivername, r.routename
                    FROM bus_schedules bs
                    JOIN buses b ON bs.busid = b.busid
                    LEFT JOIN drivers d ON b.driverid = d.driverid
                    LEFT JOIN routes r ON b.routeid = r.routeid";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            schedules.Add(new BusScheduleModel
                            {
                                ScheduleId = reader.GetInt32(reader.GetOrdinal("scheduleid")),
                                BusId = reader.GetInt32(reader.GetOrdinal("busid")),
                                StartDate = reader.IsDBNull(reader.GetOrdinal("start_date")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("start_date")),
                                StartTime = reader.IsDBNull(reader.GetOrdinal("start_time")) ? (TimeSpan?)null : reader.GetTimeSpan(reader.GetOrdinal("start_time")),
                                StartDateTime = reader.IsDBNull(reader.GetOrdinal("start_date_time")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("start_date_time")),
                                BusNumber = reader.GetString(reader.GetOrdinal("busnumber")),
                                DriverName = reader.IsDBNull(reader.GetOrdinal("drivername")) ? null : reader.GetString(reader.GetOrdinal("drivername")),
                                RouteName = reader.IsDBNull(reader.GetOrdinal("routename")) ? null : reader.GetString(reader.GetOrdinal("routename"))
                            });
                        }
                    }
                }
            }

            return Ok(schedules);
        }

        [HttpGet("getschedulebyid/{id}")]
        public async Task<ActionResult<BusScheduleModel>> GetScheduleById(int id)
        {
            BusScheduleModel schedule = null;

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = @"
                    SELECT bs.*, b.busnumber, d.drivername, r.routename
                    FROM bus_schedules bs
                    JOIN buses b ON bs.busid = b.busid
                    LEFT JOIN drivers d ON b.driverid = d.driverid
                    LEFT JOIN routes r ON b.routeid = r.routeid
                    WHERE bs.scheduleid = @ScheduleId";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("ScheduleId", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            schedule = new BusScheduleModel
                            {
                                ScheduleId = reader.GetInt32(reader.GetOrdinal("scheduleid")),
                                BusId = reader.GetInt32(reader.GetOrdinal("busid")),
                                StartDate = reader.IsDBNull(reader.GetOrdinal("start_date")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("start_date")),
                                StartTime = reader.IsDBNull(reader.GetOrdinal("start_time")) ? (TimeSpan?)null : reader.GetTimeSpan(reader.GetOrdinal("start_time")),
                                StartDateTime = reader.IsDBNull(reader.GetOrdinal("start_date_time")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("start_date_time")),
                                BusNumber = reader.GetString(reader.GetOrdinal("busnumber")),
                                DriverName = reader.IsDBNull(reader.GetOrdinal("drivername")) ? null : reader.GetString(reader.GetOrdinal("drivername")),
                                RouteName = reader.IsDBNull(reader.GetOrdinal("routename")) ? null : reader.GetString(reader.GetOrdinal("routename"))
                            };
                        }
                    }
                }
            }

            if (schedule == null)
            {
                return NotFound();
            }

            return Ok(schedule);
        }

        [HttpPost("addschedule")]
        public async Task<ActionResult> AddSchedule([FromBody] BusScheduleAddUpdate schedule)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = @"
            INSERT INTO bus_schedules (busid, start_date, start_time, start_date_time)
            VALUES (@BusId, @StartDate, @StartTime, @StartDateTime)
            RETURNING scheduleid";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("BusId", schedule.BusId);
                    command.Parameters.AddWithValue("StartDate", schedule.StartDate); // Use the derived date
                    command.Parameters.AddWithValue("StartTime", schedule.StartTime); // Use the derived time
                    command.Parameters.AddWithValue("StartDateTime", schedule.StartDateTime);

                    schedule.ScheduleId = (int)await command.ExecuteScalarAsync();
                }
            }

            return CreatedAtAction(nameof(GetScheduleById), new { id = schedule.ScheduleId }, schedule);
        }

        [HttpPut("updateschedule/{id}")]
        public async Task<ActionResult> UpdateSchedule(int id, [FromBody] BusScheduleAddUpdate schedule)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = @"
    UPDATE bus_schedules
    SET busid = @BusId, start_date = @StartDate, start_time = @StartTime, start_date_time = @StartDateTime
    WHERE scheduleid = @ScheduleId";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("ScheduleId", id);
                    command.Parameters.AddWithValue("BusId", schedule.BusId);
                    command.Parameters.AddWithValue("StartDate", schedule.StartDate); // Use the derived date
                    command.Parameters.AddWithValue("StartTime", schedule.StartTime); // Use the derived time
                    command.Parameters.AddWithValue("StartDateTime", schedule.StartDateTime);

                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    if (rowsAffected == 0)
                    {
                        return NotFound();
                    }
                }
            }

            return NoContent();
        }
        [HttpDelete("deleteschedule/{id}")]
        public async Task<ActionResult> DeleteSchedule(int id)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "DELETE FROM bus_schedules WHERE scheduleid = @ScheduleId";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("ScheduleId", id);

                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    if (rowsAffected == 0)
                    {
                        return NotFound();
                    }
                }
            }

            return NoContent();
        }
    }
}






