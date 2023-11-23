using FarmProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace FarmProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string ConnectionString;

        public CustomerController(IConfiguration configuration)
        {
            _configuration = configuration;
            ConnectionString = _configuration.GetConnectionString("DefaultConnection");
        }
        [HttpPost("CreateCustomer")]
        public IActionResult CreateCustomer([FromBody] Customer customer)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();

                    SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM Customers WHERE CustomerName = @CustomerName AND CustomerPhoneNumber = @CustomerPhoneNumber", con);
                    checkCmd.Parameters.AddWithValue("@CustomerName", customer.CustomerName);
                    checkCmd.Parameters.AddWithValue("@CustomerPhoneNumber", customer.CustomerPhoneNumber ?? (object)DBNull.Value);
                    int count = (int)checkCmd.ExecuteScalar();


                    if (count > 0)
                    {
                        return BadRequest(new { message = "Customer already exists" });
                    }
                    else
                    {
                        using (SqlCommand insertCmd = new SqlCommand("INSERT INTO Customers (CustomerName, CustomerPhoneNumber, CustomerEmail, CustomerAddress, CustomerStatus, AddedBy, AddedDate, AddedPc, UpdatedBy, UpdatedDate, UpdatedPc) VALUES (@CustomerName, @CustomerPhoneNumber, @CustomerEmail, @CustomerAddress, @CustomerStatus, @AddedBy, @AddedDate, @AddedPc, @UpdatedBy, @UpdatedDate, @UpdatedPc)", con))
                        {
                            insertCmd.Parameters.AddWithValue("@CustomerName", customer.CustomerName);
                            insertCmd.Parameters.AddWithValue("@CustomerPhoneNumber", customer.CustomerPhoneNumber ?? (object)DBNull.Value);
                            insertCmd.Parameters.AddWithValue("@CustomerEmail", customer.CustomerEmail ?? (object)DBNull.Value);
                            insertCmd.Parameters.AddWithValue("@CustomerAddress", customer.CustomerAddress ?? (object)DBNull.Value);
                            insertCmd.Parameters.AddWithValue("@CustomerStatus", customer.CustomerStatus);
                            insertCmd.Parameters.AddWithValue("@AddedBy", customer.AddedBy);
                            insertCmd.Parameters.AddWithValue("@AddedDate", customer.AddedDate);
                            insertCmd.Parameters.AddWithValue("@AddedPc", customer.AddedPc);
                            insertCmd.Parameters.AddWithValue("@UpdatedBy", customer.UpdatedBy);
                            insertCmd.Parameters.AddWithValue("@UpdatedDate", customer.UpdatedDate);
                            insertCmd.Parameters.AddWithValue("@UpdatedPc", customer.UpDatedPc);

                            insertCmd.ExecuteNonQuery();
                        }
                        con.Close();
                        return Ok(new { message = "Customer created successfully" });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing the request", error = ex.Message });
            }
        }
        [HttpPut("UpdateCustomer{CustomerId}")]
        public IActionResult UpdateCustomer(Customer customer, int CustomerId)
        {
            try
            {
                if (customer == null)
                {
                    return BadRequest(new { message = "Invalid Customer data" });
                }

                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    using (SqlCommand updateCmd = new SqlCommand("UPDATE Customers SET CustomerName = @CustomerName, CustomerPhoneNumber = @CustomerPhoneNumber, CustomerEmail = @CustomerEmail, CustomerAddress = @CustomerAddress, CustomerStatus = @CustomerStatus, UpdatedBy = @UpdatedBy, UpdatedDate = @UpdatedDate, UpDatedPc = @UpDatedPc WHERE CustomerId = @CustomerId", con))
                    {
                        updateCmd.Parameters.AddWithValue("@CustomerId", CustomerId);
                        updateCmd.Parameters.AddWithValue("@CustomerName", customer.CustomerName);
                        updateCmd.Parameters.AddWithValue("@CustomerPhoneNumber", customer.CustomerPhoneNumber ?? (object)DBNull.Value);
                        updateCmd.Parameters.AddWithValue("@CustomerEmail", customer.CustomerEmail ?? (object)DBNull.Value);
                        updateCmd.Parameters.AddWithValue("@CustomerAddress", customer.CustomerAddress ?? (object)DBNull.Value);
                        updateCmd.Parameters.AddWithValue("@CustomerStatus", customer.CustomerStatus);
                        updateCmd.Parameters.AddWithValue("@UpdatedBy", customer.UpdatedBy);
                        updateCmd.Parameters.AddWithValue("@UpdatedDate", customer.UpdatedDate);
                        updateCmd.Parameters.AddWithValue("@UpDatedPc", customer.UpDatedPc);

                        updateCmd.ExecuteNonQuery();
                    }
                    con.Close();
                    return Ok(new { message = "Customer updated successfully" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing the request", error = ex.Message });
            }
        }
        [HttpDelete("DeleteCustomer/{CustomerId}")]
        public IActionResult DeleteCustomer(int CustomerId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    SqlCommand command = new SqlCommand("DELETE FROM Customers WHERE CustomerId = @CustomerId", connection);
                    command.CommandType = CommandType.Text;

                    command.Parameters.AddWithValue("@CustomerId", CustomerId);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    connection.Close();

                    if (rowsAffected > 0)
                    {
                        return Ok(new { message = "Customer Entry deleted successfully" });
                    }
                    else
                    {
                        return NotFound(new { message = "Customer not found" });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing the request", error = ex.Message });
            }
        }
        [HttpGet]
        public IActionResult GetCustomers()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    SqlCommand getCustomersCmd = new SqlCommand("SELECT CustomerId,CustomerName,CustomerPhoneNumber,CustomerEmail,CustomerAddress,CustomerStatus FROM Customers", connection);

                    connection.Open();
                    SqlDataReader reader = getCustomersCmd.ExecuteReader();
                    List<Customer> customers = new List<Customer>();
                    while (reader.Read())
                    {
                        Customer customer = new Customer
                        {
                            CustomerId = Convert.ToInt32(reader["CustomerId"]),
                            CustomerName = reader["CustomerName"].ToString(),
                            CustomerPhoneNumber = reader["CustomerPhoneNumber"].ToString(),
                            CustomerEmail = reader["CustomerEmail"].ToString(),
                            CustomerAddress = reader["CustomerAddress"].ToString(),
                            CustomerStatus = Convert.ToBoolean(reader["CustomerStatus"]),
                        };
                        customers.Add(customer);
                    }

                    connection.Close();

                    if (customers.Count > 0)
                    {
                        return Ok(customers);
                    }
                    else
                    {
                        return NotFound(new { message = "No customers found" });
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
