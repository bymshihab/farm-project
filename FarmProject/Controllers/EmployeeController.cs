using FarmProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;

namespace FarmProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string ConnectionString;

        public EmployeeController(IConfiguration configuration)
        {
            _configuration = configuration;
            ConnectionString = _configuration.GetConnectionString("DefaultConnection");
        }


        [HttpPost("CreateEmployee")]
        public async Task<IActionResult> CreateEmployee([FromForm] Employee employee)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    string imageFileName = null;
                    if (employee.ImageFile != null)
                    {
                        imageFileName = $"{Path.GetFileNameWithoutExtension(employee.ImageFile.FileName)}{Path.GetExtension(employee.ImageFile.FileName)}";
                        string imagePath = Path.Combine("EmployeeImage", imageFileName);
                        using (Stream stream = new FileStream(imagePath, FileMode.Create))
                        {
                            await employee.ImageFile.CopyToAsync(stream);
                        }
                    }

                    string systemCode = string.Empty;
                    SqlCommand cmdSP = new SqlCommand("spMakeSystemCode", con);
                    {
                        cmdSP.CommandType = CommandType.StoredProcedure;
                        cmdSP.Parameters.AddWithValue("@TableName", "Employees");
                        cmdSP.Parameters.AddWithValue("@Date", DateTime.Now.ToString("yyyy-MM-dd"));
                        cmdSP.Parameters.AddWithValue("@AddNumber", 1);
                        systemCode = cmdSP.ExecuteScalar()?.ToString();
                    }

                    employee.EmployeeCode = systemCode.Split('%')[1];

                    using (SqlCommand insertCmd = new SqlCommand("INSERT INTO Employees (EmployeeCode, FirstName, LastName, MiddleName, Salary, DOB, HireDate, PhoneNumber, Email, NID, Description, JobTitle, Status, EmployeeImage,CompanyId, AddedBy, AddedDate, AddedPc, UpdatedBy, UpdatedDate, UpdatedPc) VALUES (@EmployeeCode, @FirstName, @LastName, @MiddleName, @Salary, @DOB, @HireDate, @PhoneNumber, @Email, @NID, @Description, @JobTitle, @Status, @EmployeeImage,@CompanyId, @AddedBy, @AddedDate, @AddedPc, @UpdatedBy, @UpdatedDate, @UpdatedPc)", con))
                    {
                        insertCmd.Parameters.AddWithValue("@EmployeeCode", employee.EmployeeCode);
                        insertCmd.Parameters.AddWithValue("@FirstName", employee.FirstName ?? (object)DBNull.Value);
                        insertCmd.Parameters.AddWithValue("@MiddleName", employee.MiddleName ?? (object)DBNull.Value);
                        insertCmd.Parameters.AddWithValue("@LastName", employee.LastName ?? (object)DBNull.Value);
                        insertCmd.Parameters.AddWithValue("@Salary", employee.Salary);
                        insertCmd.Parameters.AddWithValue("@DOB", employee.DOB ?? (object)DBNull.Value);
                        insertCmd.Parameters.AddWithValue("@HireDate", employee.HireDate != null ? (object)employee.HireDate : DBNull.Value);
                        insertCmd.Parameters.AddWithValue("@PhoneNumber", employee.PhoneNumber ?? (object)DBNull.Value);
                        insertCmd.Parameters.AddWithValue("@Email", employee.Email ?? (object)DBNull.Value);
                        insertCmd.Parameters.AddWithValue("@NID", employee.NID ?? (object)DBNull.Value);
                        insertCmd.Parameters.AddWithValue("@Description", employee.Description ?? (object)DBNull.Value);
                        insertCmd.Parameters.AddWithValue("@JobTitle", employee.JobTitle ?? (object)DBNull.Value);
                        insertCmd.Parameters.AddWithValue("@Status", employee.Status);
                        object logoValue = (object)imageFileName ?? DBNull.Value;
                        insertCmd.Parameters.AddWithValue("@EmployeeImage", logoValue);
                        insertCmd.Parameters.AddWithValue("@CompanyId", employee.CompanyId);
                        insertCmd.Parameters.AddWithValue("@AddedBy", employee.AddedBy ?? (object)DBNull.Value);
                        insertCmd.Parameters.AddWithValue("@AddedDate", employee.AddedDate != null ? (object)employee.AddedDate : DBNull.Value);
                        insertCmd.Parameters.AddWithValue("@AddedPc", employee.AddedPc ?? (object)DBNull.Value);
                        insertCmd.Parameters.AddWithValue("@UpdatedBy", employee.UpdatedBy ?? (object)DBNull.Value);
                        insertCmd.Parameters.AddWithValue("@UpdatedDate", employee.UpdatedDate != null ? (object)employee.UpdatedDate : DBNull.Value);
                        insertCmd.Parameters.AddWithValue("@UpdatedPc", employee.UpDatedPc ?? (object)DBNull.Value);

                        insertCmd.ExecuteNonQuery();
                    }

                    return Ok(new { message = "Employee created successfully" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing the request", error = ex.Message });
            }
        }

        [HttpPut("UpdateEmployee")]
        public async Task<IActionResult> UpdateEmployee([FromForm] Employee employee)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();

                    SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM Employees WHERE EId = @EId", con);
                    checkCmd.Parameters.AddWithValue("@EId", employee.EId);
                    int employeeCount = (int)checkCmd.ExecuteScalar();

                    if (employeeCount == 0)
                    {
                        return NotFound(new { message = $"Employee with EId {employee.EId} not found" });
                    }

                    string imageFileName = null;
                    if (employee.ImageFile != null)
                    {
                        imageFileName = $"{Path.GetFileNameWithoutExtension(employee.ImageFile.FileName)}{Path.GetExtension(employee.ImageFile.FileName)}";
                        string imagePath = Path.Combine("EmployeeImage", imageFileName);
                        using (Stream stream = new FileStream(imagePath, FileMode.Create))
                        {
                            await employee.ImageFile.CopyToAsync(stream);
                        }
                    }

                    using (SqlCommand updateCmd = new SqlCommand("UPDATE Employees SET FirstName = @FirstName, LastName = @LastName, MiddleName = @MiddleName, Salary = @Salary, DOB = @DOB, HireDate = @HireDate, PhoneNumber = @PhoneNumber, Email = @Email, NID = @NID, Description = @Description, JobTitle = @JobTitle, Status = @Status, EmployeeImage = @EmployeeImage, UpdatedBy = @UpdatedBy, UpdatedDate = @UpdatedDate, UpdatedPc = @UpdatedPc WHERE EId = @EId", con))
                    {
                        updateCmd.Parameters.AddWithValue("@EId", employee.EId);
                        updateCmd.Parameters.AddWithValue("@FirstName", employee.FirstName ?? (object)DBNull.Value);
                        updateCmd.Parameters.AddWithValue("@MiddleName", employee.MiddleName ?? (object)DBNull.Value);
                        updateCmd.Parameters.AddWithValue("@LastName", employee.LastName ?? (object)DBNull.Value);
                        updateCmd.Parameters.AddWithValue("@Salary", employee.Salary);
                        updateCmd.Parameters.AddWithValue("@DOB", employee.DOB ?? (object)DBNull.Value);
                        updateCmd.Parameters.AddWithValue("@HireDate", employee.HireDate != null ? (object)employee.HireDate : DBNull.Value);
                        updateCmd.Parameters.AddWithValue("@PhoneNumber", employee.PhoneNumber ?? (object)DBNull.Value);
                        updateCmd.Parameters.AddWithValue("@Email", employee.Email ?? (object)DBNull.Value);
                        updateCmd.Parameters.AddWithValue("@NID", employee.NID ?? (object)DBNull.Value);
                        updateCmd.Parameters.AddWithValue("@Description", employee.Description ?? (object)DBNull.Value);
                        updateCmd.Parameters.AddWithValue("@JobTitle", employee.JobTitle ?? (object)DBNull.Value);
                        updateCmd.Parameters.AddWithValue("@Status", employee.Status);
                        updateCmd.Parameters.AddWithValue("@UpdatedBy", employee.UpdatedBy ?? (object)DBNull.Value);
                        updateCmd.Parameters.AddWithValue("@UpdatedDate", employee.UpdatedDate != null ? (object)employee.UpdatedDate : DBNull.Value);
                        updateCmd.Parameters.AddWithValue("@UpdatedPc", employee.UpDatedPc ?? (object)DBNull.Value);

                        string existingImageFileName = null;

                        if (employee.ImageFile != null)
                        {
                            existingImageFileName = employee.EmployeeImage;
                            string newImageFileName = $"{Path.GetFileNameWithoutExtension(employee.ImageFile.FileName)}{Path.GetExtension(employee.ImageFile.FileName)}";
                            string newImagePath = Path.Combine("EmployeeImage", newImageFileName);

                            using (Stream stream = new FileStream(newImagePath, FileMode.Create))
                            {
                                await employee.ImageFile.CopyToAsync(stream);
                            }
                            updateCmd.Parameters.AddWithValue("@EmployeeImage", newImageFileName);
                        }
                        else
                        {
                            updateCmd.Parameters.AddWithValue("@EmployeeImage", employee.EmployeeImage);
                        }
                        updateCmd.ExecuteNonQuery();
                        if (employee.ImageFile != null && !string.IsNullOrEmpty(existingImageFileName))
                        {
                            string previousImagePath = Path.Combine("EmployeeImage", existingImageFileName);
                            if (System.IO.File.Exists(previousImagePath))
                            {
                                System.IO.File.Delete(previousImagePath);
                            }
                        }

                        return Ok(new { message = $"Employee with EId {employee.EId} updated successfully" });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing the request", error = ex.Message });
            }
        }
        [HttpDelete("{EId}")]
        public IActionResult DeleteEmployee(int EId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    string imageFileName = GetImageFileName(connection, EId);
                    SqlCommand deleteCommand = new SqlCommand("spDeleteEmployee", connection);
                    deleteCommand.CommandType = CommandType.StoredProcedure;
                    deleteCommand.Parameters.AddWithValue("@EId", EId);
                    int rowsAffected = deleteCommand.ExecuteNonQuery();
                    connection.Close();

                    if (rowsAffected > 0)
                    {
                        if (!string.IsNullOrEmpty(imageFileName))
                        {
                            DeleteImageFile(imageFileName);
                        }

                        return Ok(new { message = "Employee Data deleted successfully" });
                    }
                    else
                    {
                        return NotFound(new { message = "Employee not found" });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing the request", error = ex.Message });
            }
        }
        private string GetImageFileName(SqlConnection connection, int EId)
        {
            string query = "SELECT EmployeeImage FROM Employees WHERE EId = @EId";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@EId", EId);
            object result = command.ExecuteScalar();
            return result != DBNull.Value ? result.ToString() : null;
        }
        private void DeleteImageFile(string imageFileName)
        {
            try
            {
                string imagePath = Path.Combine("EmployeeImage", imageFileName);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting image file: {ex.Message}");
            }
        }
    }
}
