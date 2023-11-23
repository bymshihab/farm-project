using FarmProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace FarmProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OutsiderController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string ConnectionString;

        public OutsiderController(IConfiguration configuration)
        {
            _configuration = configuration;
            ConnectionString = _configuration.GetConnectionString("DefaultConnection");
        }
        [HttpPost("CreateOutsider")]
        public IActionResult CreateOutsider([FromBody] Outsider outsider)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();

                    SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM Outsiders WHERE OutsiderName = @OutsiderName AND phoneNumber=@phoneNumber", con);
                    checkCmd.Parameters.AddWithValue("@OutsiderName", outsider.OutsiderName);
                    checkCmd.Parameters.AddWithValue("@phoneNumber", outsider.PhoneNumber);
                    int count = (int)checkCmd.ExecuteScalar();

                    if (count > 0)
                    {
                        return BadRequest(new { message = "Outsider already exists" });
                    }
                    else
                    {
                        using (SqlCommand insertCmd = new SqlCommand("INSERT INTO Outsiders (OutsiderName, OutsiderCatagory, OutsiderAddress, phoneNumber, AddedBy, AddedDate, AddedPc, UpdatedBy, UpdatedDate, UpDatedPc) VALUES (@OutsiderName, @OutsiderCatagory, @OutsiderAddress, @phoneNumber, @AddedBy, @AddedDate, @AddedPc, @UpdatedBy, @UpdatedDate, @UpDatedPc)", con))
                        {
                            insertCmd.Parameters.AddWithValue("@OutsiderName", outsider.OutsiderName);
                            insertCmd.Parameters.AddWithValue("@OutsiderCatagory", outsider.OutsiderCatagory ?? (object)DBNull.Value);
                            insertCmd.Parameters.AddWithValue("@OutsiderAddress", outsider.OutsiderAddress ?? (object)DBNull.Value);
                            insertCmd.Parameters.AddWithValue("@phoneNumber", outsider.PhoneNumber ?? (object)DBNull.Value);
                            insertCmd.Parameters.AddWithValue("@AddedBy", outsider.AddedBy);
                            insertCmd.Parameters.AddWithValue("@AddedDate", outsider.AddedDate ?? (object)DBNull.Value);
                            insertCmd.Parameters.AddWithValue("@AddedPc", outsider.AddedPc);
                            insertCmd.Parameters.AddWithValue("@UpdatedBy", outsider.UpdatedBy ?? (object)DBNull.Value);
                            insertCmd.Parameters.AddWithValue("@UpdatedDate", outsider.UpdatedDate ?? (object)DBNull.Value);
                            insertCmd.Parameters.AddWithValue("@UpDatedPc", outsider.UpDatedPc ?? (object)DBNull.Value);

                            insertCmd.ExecuteNonQuery();
                        }

                        con.Close();

                        return Ok(new { message = "Outsider created successfully" });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing the request", error = ex.Message });
            }
        }
        [HttpPut("UpdateOutsider/{OutsiderId}")]
        public IActionResult UpdateShedType(Outsider outsider, int OutsiderId)
        {
            try
            {
                if (outsider == null)
                {
                    return BadRequest(new { message = "Invalid Outsider data" });
                }

                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    using (SqlCommand updateCmd = new SqlCommand("UPDATE Outsiders SET OutsiderName = @OutsiderName, OutsiderCatagory = @OutsiderCatagory, OutsiderAddress = @OutsiderAddress, phoneNumber = @phoneNumber, UpdatedBy = @UpdatedBy, UpdatedDate = @UpdatedDate, UpDatedPc = @UpDatedPc WHERE OutsiderId = @OutsiderId", con))
                    {
                        updateCmd.Parameters.AddWithValue("@OutsiderId", OutsiderId);
                        updateCmd.Parameters.AddWithValue("@OutsiderName", outsider.OutsiderName);
                        updateCmd.Parameters.AddWithValue("@OutsiderCatagory", outsider.OutsiderCatagory ?? (object)DBNull.Value);
                        updateCmd.Parameters.AddWithValue("@OutsiderAddress", outsider.OutsiderAddress ?? (object)DBNull.Value);
                        updateCmd.Parameters.AddWithValue("@phoneNumber", outsider.PhoneNumber ?? (object)DBNull.Value);
                        updateCmd.Parameters.AddWithValue("@UpdatedBy", outsider.UpdatedBy);
                        updateCmd.Parameters.AddWithValue("@UpdatedDate", outsider.UpdatedDate);
                        updateCmd.Parameters.AddWithValue("@UpDatedPc", outsider.UpDatedPc ?? (object)DBNull.Value);

                        updateCmd.ExecuteNonQuery();
                    }
                    con.Close();
                    return Ok(new { message = "Outsider updated successfully" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing the request", error = ex.Message });
            }
        }
        [HttpDelete("DeleteOutsider/{OutsiderId}")]
        public IActionResult DeleteOutsider(int OutsiderId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    SqlCommand command = new SqlCommand("DELETE FROM Outsiders WHERE OutsiderId = @OutsiderId", connection);
                    command.CommandType = CommandType.Text;

                    command.Parameters.AddWithValue("@OutsiderId", OutsiderId);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    connection.Close();

                    if (rowsAffected > 0)
                    {
                        return Ok(new { message = "Outsider deleted successfully" });
                    }
                    else
                    {
                        return NotFound(new { message = "Outsider not found" });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing the request", error = ex.Message });
            }
        }
        [HttpGet]
        public IActionResult GetOutsiders()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    SqlCommand getOutsidersCmd = new SqlCommand("SELECT OutsiderId, OutsiderName, OutsiderCatagory, OutsiderAddress, phoneNumber FROM Outsiders", connection);

                    connection.Open();
                    SqlDataReader reader = getOutsidersCmd.ExecuteReader();
                    List<Outsider> outsiders = new List<Outsider>();
                    while (reader.Read())
                    {
                        Outsider outsider = new Outsider
                        {
                            OutsiderId = Convert.ToInt32(reader["OutsiderId"]),
                            OutsiderName = reader["OutsiderName"].ToString(),
                            OutsiderCatagory = reader["OutsiderCatagory"].ToString(),
                            OutsiderAddress = reader["OutsiderAddress"].ToString(),
                            PhoneNumber = reader["phoneNumber"].ToString(),
                        };
                        outsiders.Add(outsider);
                    }
                    connection.Close();
                    if (outsiders.Count > 0)
                    {
                        return Ok(outsiders);
                    }
                    else
                    {
                        return NotFound(new { message = "No Outsiders Here" });
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
