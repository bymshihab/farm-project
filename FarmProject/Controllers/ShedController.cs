using FarmProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace FarmProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShedController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string ConnectionString;

        public ShedController(IConfiguration configuration)
        {
            _configuration = configuration;
            ConnectionString = _configuration.GetConnectionString("DefaultConnection");
        }
        [HttpPost("CreateShed")]
        public IActionResult CreateSupplier(Shed shed)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();

                    SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM Sheds WHERE ShedName = @ShedName", con);
                    checkCmd.Parameters.AddWithValue("@ShedName", shed.ShedName);
                    int count = (int)checkCmd.ExecuteScalar();

                    if (count > 0)
                    {
                        return BadRequest(new { message = "Shed already exists" });
                    }
                    else
                    {
                        using (SqlCommand insertCmd = new SqlCommand("INSERT INTO Sheds (ShedName, ShedDescription, Status, ShedTypeId, AddedBy, AddedDate, AddedPc, UpdatedBy, UpdatedDate, UpdatedPc) VALUES (@ShedName, @ShedDescription, @Status, @ShedTypeId, @AddedBy, @AddedDate, @AddedPc, @UpdatedBy, @UpdatedDate, @UpdatedPc)", con))
                        {
                            insertCmd.Parameters.AddWithValue("@ShedName", shed.ShedName);
                            insertCmd.Parameters.AddWithValue("@ShedDescription", shed.ShedDescription ?? (object)DBNull.Value);
                            insertCmd.Parameters.AddWithValue("@Status", shed.Status);
                            insertCmd.Parameters.AddWithValue("@ShedTypeId", shed.ShedTypeId);
                            insertCmd.Parameters.AddWithValue("@AddedBy", shed.AddedBy);
                            insertCmd.Parameters.AddWithValue("@AddedDate", shed.AddedDate);
                            insertCmd.Parameters.AddWithValue("@AddedPc", shed.AddedPc);
                            insertCmd.Parameters.AddWithValue("@UpdatedBy", shed.UpdatedBy);
                            insertCmd.Parameters.AddWithValue("@UpdatedDate", shed.UpdatedDate);
                            insertCmd.Parameters.AddWithValue("@UpdatedPc", shed.UpDatedPc);

                            insertCmd.ExecuteNonQuery();
                        }

                        con.Close();

                        return Ok(new { message = "Shed created successfully" });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing the request", error = ex.Message });
            }
        }
        [HttpPut("UpdateShed/{ShedId}")]
        public IActionResult UpdateShedType(Shed shed, int ShedId)
        {
            try
            {
                if (shed == null)
                {
                    return BadRequest(new { message = "Invalid Shed data" });
                }

                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    using (SqlCommand updateCmd = new SqlCommand("UPDATE Sheds SET ShedName = @ShedName, ShedDescription = @ShedDescription, Status = @Status, UpdatedBy = @UpdatedBy, UpdatedDate = @UpdatedDate, UpdatedPc = @UpdatedPc WHERE ShedTypeId = @ShedTypeId", con))
                    {
                        updateCmd.Parameters.AddWithValue("@ShedId", ShedId);
                        updateCmd.Parameters.AddWithValue("@ShedName", shed.ShedName);
                        updateCmd.Parameters.AddWithValue("@ShedDescription", shed.ShedDescription ?? (object)DBNull.Value);
                        updateCmd.Parameters.AddWithValue("@Status", shed.Status);
                        updateCmd.Parameters.AddWithValue("@ShedTypeId", shed.ShedTypeId);
                        updateCmd.Parameters.AddWithValue("@UpdatedBy", shed.UpdatedBy);
                        updateCmd.Parameters.AddWithValue("@UpdatedDate", shed.UpdatedDate);
                        updateCmd.Parameters.AddWithValue("@UpdatedPc", shed.UpDatedPc);

                        updateCmd.ExecuteNonQuery();
                    }
                    con.Close();
                    return Ok(new { message = "Shed updated successfully" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing the request", error = ex.Message });
            }
        }
        [HttpDelete("DeleteShed/{ShedId}")]
        public IActionResult DeleteShed(int ShedId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    SqlCommand command = new SqlCommand("DELETE FROM Sheds WHERE ShedId = @ShedId", connection);
                    command.CommandType = CommandType.Text;

                    command.Parameters.AddWithValue("@ShedId", ShedId);

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
        [HttpGet]
        public IActionResult GetSheds()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    SqlCommand getShedsCmd = new SqlCommand("EXEC ActiveSheds", connection);

                    connection.Open();
                    SqlDataReader reader = getShedsCmd.ExecuteReader();
                    List<ActiveSheds> activeSheds = new List<ActiveSheds>();
                    while (reader.Read())
                    {
                        ActiveSheds activeShed = new ActiveSheds
                        {
                            ShedId = Convert.ToInt32(reader["ShedId"]),
                            ShedName = reader["ShedName"].ToString(),
                            ShedDescription = reader["ShedDescription"].ToString(),
                            shedTypeName = reader["ShedTypeName"].ToString(),
                            Status = reader["Status"] != DBNull.Value ? Convert.ToBoolean(reader["Status"]) : false,
                        };
                        activeSheds.Add(activeShed);
                    }

                    connection.Close();

                    if (activeSheds.Count > 0)
                    {
                        return Ok(activeSheds);
                    }
                    else
                    {
                        return NotFound(new { message = "No Sheds Here" });
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
