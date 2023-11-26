using FarmProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace FarmProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UomController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public UomController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        [HttpPost("CreateUom")]
        public IActionResult CreateUom( Uom uom)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    con.Open();

                    SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM Uoms WHERE UomName = @UomName", con);
                    checkCmd.Parameters.AddWithValue("@UomName", uom.UomName);
                    int count = (int)checkCmd.ExecuteScalar();

                    if (count > 0)
                    {
                        return BadRequest(new { message = "Unit already exists" });
                    }
                    else
                    {
                        using (SqlCommand insertCmd = new SqlCommand("INSERT INTO Uoms (UomName, UomDescription, Status,CompanyId, AddedBy, AddedDate, AddedPc, UpdatedBy, UpdatedDate, UpdatedPc) VALUES (@UomName, @UomDescription, @Status,@CompanyId, @AddedBy, @AddedDate, @AddedPc, @UpdatedBy, @UpdatedDate, @UpdatedPc)", con))
                        {
                            insertCmd.Parameters.AddWithValue("@UomName", uom.UomName);
                            insertCmd.Parameters.AddWithValue("@UomDescription", uom.UomDescription ?? (object)DBNull.Value);
                            insertCmd.Parameters.AddWithValue("@Status", uom.Status);
                            insertCmd.Parameters.AddWithValue("@CompanyId", uom.CompanyId);
                            insertCmd.Parameters.AddWithValue("@AddedBy", uom.AddedBy);
                            insertCmd.Parameters.AddWithValue("@AddedDate", uom.AddedDate);
                            insertCmd.Parameters.AddWithValue("@AddedPc", uom.AddedPc);
                            insertCmd.Parameters.AddWithValue("@UpdatedBy", uom.UpdatedBy);
                            insertCmd.Parameters.AddWithValue("@UpdatedDate", uom.UpdatedDate);
                            insertCmd.Parameters.AddWithValue("@UpdatedPc", uom.UpDatedPc);

                            insertCmd.ExecuteNonQuery();
                        }

                        con.Close();

                        return Ok(new { message = "Unit created successfully" });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing the request", error = ex.Message });
            }
        }
        [HttpPut("UpdateUom/{UomId}")]
        public IActionResult UpdateUom( Uom uom, int UomId)
        {
            try
            {
                if (uom == null)
                {
                    return BadRequest(new { message = "Invalid Unit data" });
                }

                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    con.Open();

                    SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM Uoms WHERE UomId = @UomId", con);
                    checkCmd.Parameters.AddWithValue("@UomId", UomId);
                    int count = (int)checkCmd.ExecuteScalar();

                    if (count == 0)
                    {
                        return NotFound(new { message = "Unit not found" });
                    }

                    using (SqlCommand updateCmd = new SqlCommand("UPDATE Uoms SET UomName = @UomName, UomDescription = @UomDescription, Status = @Status, UpdatedBy = @UpdatedBy, UpdatedDate = @UpdatedDate, UpdatedPc = @UpdatedPc WHERE UomId = @UomId", con))
                    {
                        updateCmd.Parameters.AddWithValue("@UomId", UomId);
                        updateCmd.Parameters.AddWithValue("@UomName", uom.UomName);
                        updateCmd.Parameters.AddWithValue("@UomDescription", uom.UomDescription ?? (object)DBNull.Value);
                        updateCmd.Parameters.AddWithValue("@Status", uom.Status);
                        updateCmd.Parameters.AddWithValue("@UpdatedBy", uom.UpdatedBy);
                        updateCmd.Parameters.AddWithValue("@UpdatedDate", uom.UpdatedDate);
                        updateCmd.Parameters.AddWithValue("@UpdatedPc", uom.UpDatedPc);

                        updateCmd.ExecuteNonQuery();
                    }

                    con.Close();

                    return Ok(new { message = "Unit updated successfully" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing the request", error = ex.Message });
            }
        }

        [HttpDelete("DeleteUom/{UomId}")]
        public IActionResult DeleteUom(int UomId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    SqlCommand command = new SqlCommand("DELETE FROM Uoms WHERE UomId = @UomId", connection);
                    command.CommandType = CommandType.Text;

                    command.Parameters.AddWithValue("@UomId", UomId);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    connection.Close();
                    if (rowsAffected > 0)
                    {
                        return Ok(new { message = "Unit deleted successfully" });
                    }
                    else
                    {
                        return NotFound(new { message = "Unit not found" });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing the request", error = ex.Message });
            }
        }


        [HttpGet]
        public IActionResult GetUoms()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    SqlCommand getCompanyCmd = new SqlCommand("SELECT UomId,UomName,UomDescription,Status FROM Uoms", connection);

                    connection.Open();
                    SqlDataReader reader = getCompanyCmd.ExecuteReader();
                    List<Uom> uoms = new List<Uom>();
                    while (reader.Read())
                    {
                        Uom uom = new Uom
                        {
                            UomId = Convert.ToInt32(reader["UomId"]),
                            UomName = reader["UomName"].ToString(),
                            UomDescription = reader["UomDescription"].ToString(),
                            Status = reader["Status"] != DBNull.Value ? Convert.ToBoolean(reader["Status"]) : false,
                        };
                        uoms.Add(uom);
                    }

                    connection.Close();

                    if (uoms.Count > 0)
                    {
                        return Ok(uoms);
                    }
                    else
                    {
                        return NotFound(new { message = "No Unit found" });
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
