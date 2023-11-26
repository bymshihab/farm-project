using FarmProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Net;

namespace FarmProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuppliersController : ControllerBase
    {

        private readonly IConfiguration _configuration;
        private readonly string ConnectionString;

        public SuppliersController(IConfiguration configuration)
        {
            _configuration = configuration;
            ConnectionString = _configuration.GetConnectionString("DefaultConnection");
        }
        [HttpPost("CreateSupplier")]
        public IActionResult CreateSupplier(Suppliers suppliers)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();

                    SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM Suppliers WHERE SupplierName = @SupplierName", con);
                    checkCmd.Parameters.AddWithValue("@SupplierName", suppliers.SupplierName);
                    int count = (int)checkCmd.ExecuteScalar();

                    if (count > 0)
                    {
                        return BadRequest(new { message = "Suppliers already exists" });
                    }
                    else
                    {
                        using (SqlCommand insertCmd = new SqlCommand("INSERT INTO Suppliers (SupplierName, PhoneNo, Email, Address, Status,CompanyId, AddedBy, AddedDate, AddedPc, UpdatedBy, UpdatedDate, UpdatedPc) VALUES (@SupplierName, @PhoneNo, @Email, @Address, @Status,@CompanyId, @AddedBy, @AddedDate, @AddedPc, @UpdatedBy, @UpdatedDate, @UpdatedPc)", con))
                        {
                            insertCmd.Parameters.AddWithValue("@SupplierName", suppliers.SupplierName);
                            insertCmd.Parameters.AddWithValue("@PhoneNo", suppliers.PhoneNo ?? (object)DBNull.Value);
                            insertCmd.Parameters.AddWithValue("@Email", suppliers.Email ?? (object)DBNull.Value);
                            insertCmd.Parameters.AddWithValue("@Address", suppliers.Address ?? (object)DBNull.Value);
                            insertCmd.Parameters.AddWithValue("@Status", suppliers.Status);
                            insertCmd.Parameters.AddWithValue("@CompanyId", suppliers.CompanyId);
                            insertCmd.Parameters.AddWithValue("@AddedBy", suppliers.AddedBy);
                            insertCmd.Parameters.AddWithValue("@AddedDate", suppliers.AddedDate);
                            insertCmd.Parameters.AddWithValue("@AddedPc", suppliers.AddedPc);
                            insertCmd.Parameters.AddWithValue("@UpdatedBy", suppliers.UpdatedBy);
                            insertCmd.Parameters.AddWithValue("@UpdatedDate", suppliers.UpdatedDate);
                            insertCmd.Parameters.AddWithValue("@UpdatedPc", suppliers.UpDatedPc);

                            insertCmd.ExecuteNonQuery();
                        }

                        con.Close();

                        return Ok(new { message = "Supplier created successfully" });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing the request", error = ex.Message });
            }
        }

        [HttpPut("UpdateSupplier/{SupplierId}")]
        public IActionResult UpdateSupplier(Suppliers suppliers, int SupplierId)
        {
            try
            {
                if (suppliers == null)
                {
                    return BadRequest(new { message = "Invalid supplier data" });
                }

                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    using (SqlCommand updateCmd = new SqlCommand("UPDATE Suppliers SET SupplierName = @SupplierName, PhoneNo = @PhoneNo, Email = @Email, Address = @Address, Status = @Status, UpdatedBy = @UpdatedBy, UpdatedDate = @UpdatedDate, UpdatedPc = @UpdatedPc WHERE SupplierId = @SupplierId", con))
                    {
                        updateCmd.Parameters.AddWithValue("@SupplierId", SupplierId);
                        updateCmd.Parameters.AddWithValue("@SupplierName", suppliers.SupplierName);
                        updateCmd.Parameters.AddWithValue("@PhoneNo", suppliers.PhoneNo ?? (object)DBNull.Value);
                        updateCmd.Parameters.AddWithValue("@Email", suppliers.Email ?? (object)DBNull.Value);
                        updateCmd.Parameters.AddWithValue("@Address", suppliers.Address);
                        updateCmd.Parameters.AddWithValue("@Status", suppliers.Status);
                        updateCmd.Parameters.AddWithValue("@UpdatedBy", suppliers.UpdatedBy);
                        updateCmd.Parameters.AddWithValue("@UpdatedDate", suppliers.UpdatedDate);
                        updateCmd.Parameters.AddWithValue("@UpdatedPc", suppliers.UpDatedPc);

                        updateCmd.ExecuteNonQuery();
                    }
                    con.Close();
                    return Ok(new { message = "Supplier updated successfully" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing the request", error = ex.Message });
            }
        }

        [HttpDelete("DeleteSupplier/{SupplierId}")]
        public IActionResult DeleteCategory(int SupplierId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    SqlCommand command = new SqlCommand("DELETE FROM Suppliers WHERE SupplierId = @SupplierId", connection);
                    command.CommandType = CommandType.Text;

                    command.Parameters.AddWithValue("@SupplierId", SupplierId);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    connection.Close();

                    if (rowsAffected > 0)
                    {
                        return Ok(new { message = "Supplier deleted successfully" });
                    }
                    else
                    {
                        return NotFound(new { message = "Supplier not found" });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing the request", error = ex.Message });
            }
        }
        [HttpGet]
        public IActionResult GetSupplier()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    SqlCommand getSupplierCmd = new SqlCommand("SELECT SupplierId, SupplierName, PhoneNo, Email, Address, Status FROM Suppliers", connection);

                    connection.Open();
                    SqlDataReader reader = getSupplierCmd.ExecuteReader();
                    List<Suppliers> suppliers = new List<Suppliers>();
                    while (reader.Read())
                    {
                        Suppliers supplier = new Suppliers
                        {
                            SupplierId = Convert.ToInt32(reader["SupplierId"]),
                            SupplierName = reader["SupplierName"].ToString(),
                            PhoneNo = reader["PhoneNo"].ToString(),
                            Email = reader["Email"].ToString(),
                            Address = reader["Address"].ToString(),
                            Status = reader["Status"] != DBNull.Value ? Convert.ToBoolean(reader["Status"]) : false,
                        };
                        suppliers.Add(supplier);
                    }

                    connection.Close();

                    if (suppliers.Count > 0)
                    {
                        return Ok(suppliers);
                    }
                    else
                    {
                        return NotFound(new { message = "No Suppliers Here" });
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
