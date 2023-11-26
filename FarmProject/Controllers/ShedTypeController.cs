using FarmProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design;
using System.Data;
using System.Data.SqlClient;

namespace FarmProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShedTypeController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string ConnectionString;

        public ShedTypeController(IConfiguration configuration)
        {
            _configuration = configuration;
            ConnectionString = _configuration.GetConnectionString("DefaultConnection");
        }
        [HttpPost("CreateShedType")]
        public IActionResult CreateSupplier(ShedType shedType)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();

                    SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM ShedTypes WHERE shedTypeName = @shedTypeName", con);
                    checkCmd.Parameters.AddWithValue("@shedTypeName", shedType.shedTypeName);
                    int count = (int)checkCmd.ExecuteScalar();

                    if (count > 0)
                    {
                        return BadRequest(new { message = "ShedType already exists" });
                    }
                    else
                    {
                        using (SqlCommand insertCmd = new SqlCommand("INSERT INTO ShedTypes (shedTypeName, ShedTypeDescription, Status,CompanyId, AddedBy, AddedDate, AddedPc, UpdatedBy, UpdatedDate, UpdatedPc) VALUES (@shedTypeName, @ShedTypeDescription, @Status,@CompanyId, @AddedBy, @AddedDate, @AddedPc, @UpdatedBy, @UpdatedDate, @UpdatedPc)", con))
                        {
                            insertCmd.Parameters.AddWithValue("@shedTypeName", shedType.shedTypeName);
                            insertCmd.Parameters.AddWithValue("@ShedTypeDescription", shedType.ShedTypeDescription ?? (object)DBNull.Value);
                            insertCmd.Parameters.AddWithValue("@Status", shedType.Status);
                            insertCmd.Parameters.AddWithValue("@CompanyId", shedType.CompanyId);
                            insertCmd.Parameters.AddWithValue("@AddedBy", shedType.AddedBy);
                            insertCmd.Parameters.AddWithValue("@AddedDate", shedType.AddedDate);
                            insertCmd.Parameters.AddWithValue("@AddedPc", shedType.AddedPc);
                            insertCmd.Parameters.AddWithValue("@UpdatedBy", shedType.UpdatedBy);
                            insertCmd.Parameters.AddWithValue("@UpdatedDate", shedType.UpdatedDate);
                            insertCmd.Parameters.AddWithValue("@UpdatedPc", shedType.UpDatedPc);

                            insertCmd.ExecuteNonQuery();
                        }

                        con.Close();

                        return Ok(new { message = "ShedType created successfully" });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing the request", error = ex.Message });
            }
        }
           [HttpPut("UpdateShedType/{ShedTypeId}")]
            public IActionResult UpdateShedType(ShedType shedType, int ShedTypeId)
            {
                try
                {
                    if (shedType == null)
                    {
                        return BadRequest(new { message = "Invalid ShedType data" });
                    }

                    using (SqlConnection con = new SqlConnection(ConnectionString))
                    {
                        con.Open();
                        using (SqlCommand updateCmd = new SqlCommand("UPDATE ShedTypes SET shedTypeName = @shedTypeName, ShedTypeDescription = @ShedTypeDescription, Status = @Status, UpdatedBy = @UpdatedBy, UpdatedDate = @UpdatedDate, UpdatedPc = @UpdatedPc WHERE ShedTypeId = @ShedTypeId", con))
                        {
                            updateCmd.Parameters.AddWithValue("@ShedTypeId", ShedTypeId);
                            updateCmd.Parameters.AddWithValue("@shedTypeName", shedType.shedTypeName);
                            updateCmd.Parameters.AddWithValue("@ShedTypeDescription", shedType.ShedTypeDescription ?? (object)DBNull.Value);
                            updateCmd.Parameters.AddWithValue("@Status", shedType.Status);
                            updateCmd.Parameters.AddWithValue("@UpdatedBy", shedType.UpdatedBy);
                            updateCmd.Parameters.AddWithValue("@UpdatedDate", shedType.UpdatedDate);
                            updateCmd.Parameters.AddWithValue("@UpdatedPc", shedType.UpDatedPc);

                            updateCmd.ExecuteNonQuery();
                        }
                        con.Close();
                        return Ok(new { message = "ShedType updated successfully" });
                    }
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { message = "An error occurred while processing the request", error = ex.Message });
                }
            }

        [HttpDelete("DeleteShedType/{ShedTypeId}")]
        public IActionResult DeleteShedType(int ShedTypeId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    SqlCommand command = new SqlCommand("DELETE FROM ShedTypes WHERE ShedTypeId = @ShedTypeId", connection);
                    command.CommandType = CommandType.Text;

                    command.Parameters.AddWithValue("@ShedTypeId", ShedTypeId);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    connection.Close();

                    if (rowsAffected > 0)
                    {
                        return Ok(new { message = "ShedType deleted successfully" });
                    }
                    else
                    {
                        return NotFound(new { message = "ShedType not found" });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing the request", error = ex.Message });
            }
        }
        [HttpGet("GetShedType")]
        public IActionResult GetShedType()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    SqlCommand getshedTypesCmd = new SqlCommand("SELECT ShedTypeId, shedTypeName, ShedTypeDescription, Status FROM ShedTypes", connection);

                    connection.Open();
                    SqlDataReader reader = getshedTypesCmd.ExecuteReader();
                    List<ShedType> shedTypes = new List<ShedType>();
                    while (reader.Read())
                    {
                        ShedType shedType = new ShedType
                        {
                            shedTypeId = Convert.ToInt32(reader["ShedTypeId"]),
                            shedTypeName = reader["shedTypeName"].ToString(),
                            ShedTypeDescription = reader["ShedTypeDescription"].ToString(),
                            Status = reader["Status"] != DBNull.Value ? Convert.ToBoolean(reader["Status"]) : false,
                        };
                        shedTypes.Add(shedType);
                    }

                    connection.Close();

                    if (shedTypes.Count > 0)
                    {
                        return Ok(shedTypes);
                    }
                    else
                    {
                        return NotFound(new { message = "No ShedType Here" });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing the request", error = ex.Message });
            }
        }
        [HttpGet("getShedTypeName")]
        public IActionResult getShedTypeName(int CompanyId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    SqlCommand getShedTypeNameCmd = new SqlCommand("SELECT ShedTypeId, shedTypeName FROM ShedTypes Where Status=1 And CompanyId=@CompanyId", connection);
                    getShedTypeNameCmd.Parameters.AddWithValue("@CompanyId", CompanyId);
                    connection.Open();
                    SqlDataReader reader = getShedTypeNameCmd.ExecuteReader();
                    List<ShedType> shedTypes = new List<ShedType>();
                    while (reader.Read())
                    {
                        ShedType shedType = new ShedType
                        {
                            shedTypeId = Convert.ToInt32(reader["ShedTypeId"]),
                            shedTypeName = reader["shedTypeName"].ToString(),
                        };
                        shedTypes.Add(shedType);
                    }

                    connection.Close();

                    if (shedTypes.Count > 0)
                    {
                        return Ok(shedTypes);
                    }
                    else
                    {
                        return NotFound(new { message = "No ShedTypes Here" });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing the request", error = ex.Message });
            }
        }
    }
}
