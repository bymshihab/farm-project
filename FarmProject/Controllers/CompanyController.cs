using FarmProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;

namespace FarmProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string ConnectionString;

        public CompanyController(IConfiguration configuration)
        {
            _configuration = configuration;
            ConnectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        [HttpPost]
        public async Task<IActionResult> CreateCompany([FromForm] Company company)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand getLastCompanyIdCmd = new SqlCommand("SELECT MAX(CompanyId) FROM Companys;", connection);
                    object result = await getLastCompanyIdCmd.ExecuteScalarAsync();
                    int lastCompanyId = result != DBNull.Value ? Convert.ToInt32(result) : 0;

                    company.CompanyId = lastCompanyId + 1;

                    string imageFileName = null;
                    if (company.ImageFile != null)
                    {
                        imageFileName = $"{Path.GetFileNameWithoutExtension(company.ImageFile.FileName)}{Path.GetExtension(company.ImageFile.FileName)}";
                        string imagePath = Path.Combine("CompanyLogo", imageFileName);
                        using (Stream stream = new FileStream(imagePath, FileMode.Create))
                        {
                            await company.ImageFile.CopyToAsync(stream);
                        }
                    }

                    // SQL query for insertion
                    string insertQuery = @"
                        INSERT INTO Companys (CompanyId, CompanyName, VATRegNo, TINNo, TradeLicenseNo, Address, PhoneNo, Email, Logo, IsMaster, IsActive, AddedBy, DateAdded, AddedPC, UpdatedBy, DateUpdated, UpdatedPC)
                        VALUES (@CompanyId, @CompanyName, @VATRegNo, @TINNo, @TradeLicenseNo, @Address, @PhoneNo, @Email, @Logo, @IsMaster, @IsActive, @AddedBy, @DateAdded, @AddedPC, @UpdatedBy, @DateUpdated, @UpdatedPC);";

                    SqlCommand cmd = new SqlCommand(insertQuery, connection);

                    cmd.Parameters.AddWithValue("@CompanyId", company.CompanyId);
                    cmd.Parameters.AddWithValue("@CompanyName", company.CompanyName);
                    cmd.Parameters.AddWithValue("@VATRegNo", company.VATRegNo != null ? company.VATRegNo : DBNull.Value);
                    cmd.Parameters.AddWithValue("@TINNo", company.TINNo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TradeLicenseNo", company.TradeLicenseNo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Address", company.Address ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@PhoneNo", company.PhoneNo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", company.Email ?? (object)DBNull.Value);

                    object logoValue = (object)imageFileName ?? DBNull.Value;
                    cmd.Parameters.AddWithValue("@Logo", logoValue);

                    cmd.Parameters.AddWithValue("@IsMaster", company.IsMaster);
                    cmd.Parameters.AddWithValue("@IsActive", company.IsActive);
                    cmd.Parameters.AddWithValue("@AddedBy", company.AddedBy ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@DateAdded", company.DateAdded ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@AddedPC", company.AddedPC ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@UpdatedBy", company.UpdatedBy ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@DateUpdated", company.DateUpdated ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@UpdatedPC", company.UpdatedPC ?? (object)DBNull.Value);

                    cmd.ExecuteNonQuery();

                    connection.Close();

                    return Ok(new { message = "company created successfully", company.CompanyId });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing the request", error = ex.Message });
            }
        }
        [HttpPut("{CompanyId}")]
        public async Task<IActionResult> UpdateCompany(int CompanyId, [FromForm] Company company)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand(" UPDATE Companys    SET  CompanyName = @CompanyName, VATRegNo = @VATRegNo, TINNo = @TINNo, TradeLicenseNo = @TradeLicenseNo, Address = @Address, PhoneNo = @PhoneNo, Email = @Email, Logo = @Logo, IsMaster = @IsMaster, IsActive = @IsActive, UpdatedBy = @UpdatedBy, DateUpdated = @DateUpdated, UpdatedPC = @UpdatedPC WHERE CompanyId = @CompanyId;", connection);
                    cmd.Parameters.AddWithValue("@CompanyId", CompanyId);
                    cmd.Parameters.AddWithValue("@CompanyName", company.CompanyName);
                    cmd.Parameters.AddWithValue("@VATRegNo", company.VATRegNo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TINNo", company.TINNo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TradeLicenseNo", company.TradeLicenseNo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Address", company.Address ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@PhoneNo", company.PhoneNo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", company.Email ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsMaster", company.IsMaster);
                    cmd.Parameters.AddWithValue("@IsActive", company.IsActive);
                    cmd.Parameters.AddWithValue("@UpdatedBy", company.UpdatedBy ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@DateUpdated", company.DateUpdated ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@UpdatedPC", company.UpdatedPC ?? (object)DBNull.Value);

                    string existingLogo = null;
                    if (company.ImageFile != null)
                    {
 
                      string  imageFileName = $"{Path.GetFileNameWithoutExtension(company.ImageFile.FileName)}{Path.GetExtension(company.ImageFile.FileName)}";

                        string imagePath = Path.Combine("CompanyLogo", imageFileName);

                        using (var fileStream = new FileStream(imagePath, FileMode.Create))
                        {
                            await company.ImageFile.CopyToAsync(fileStream);
                        }

                        existingLogo = company.Logo;
                        cmd.Parameters.AddWithValue("@Logo", imageFileName);
                    }
                    else
                    {
                        existingLogo = company.Logo;
                        cmd.Parameters.AddWithValue("@Logo", existingLogo);
                    }

                    connection.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    connection.Close();

                    if (rowsAffected > 0)
                    {
                        if (company.ImageFile != null && !string.IsNullOrEmpty(existingLogo))
                        {
                            string previousImagePath = Path.Combine("CompanyLogo", existingLogo);
                            if (System.IO.File.Exists(previousImagePath))
                            {
                                System.IO.File.Delete(previousImagePath);
                            }
                        }

                        return Ok(new { message = "company Updated successfully", company.CompanyId });
                    }
                    else
                    {
                        return NotFound(new { message = "company not found" });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing the request", error = ex.Message });
            }
        }
        [HttpDelete("{CompanyId}")]
        public IActionResult DeleteCompany(int CompanyId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    string imageFileName = GetImageFileName(connection, CompanyId);

                    SqlCommand deleteCommand = new SqlCommand("spDeleteCompany", connection);
                    deleteCommand.CommandType = CommandType.StoredProcedure;
                    deleteCommand.Parameters.AddWithValue("@CompanyId", CompanyId);
                    int rowsAffected = deleteCommand.ExecuteNonQuery();

                    connection.Close();

                    if (rowsAffected > 0)
                    {
                        if (!string.IsNullOrEmpty(imageFileName))
                        {
                            DeleteImageFile(imageFileName);
                        }

                        return Ok(new { message = "Company deleted successfully" });
                    }
                    else
                    {
                        return NotFound(new { message = "CompanyId not found" });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing the request", error = ex.Message });
            }
        }

        private string GetImageFileName(SqlConnection connection, int companyId)
        {
            string query = "SELECT Logo FROM Companys WHERE CompanyId = @CompanyId";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@CompanyId", companyId);
            object result = command.ExecuteScalar();
            return result != DBNull.Value ? result.ToString() : null;
        }
        private void DeleteImageFile(string imageFileName)
        {
            try
            {
                string imagePath = Path.Combine("CompanyLogo", imageFileName);
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
        [HttpGet]
        public IActionResult GetCompanies()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    SqlCommand getCompanyCmd = new SqlCommand("spGetCompanies", connection);

                    connection.Open();
                    SqlDataReader reader = getCompanyCmd.ExecuteReader();
                    List<GetCompanyDetails> companies = new List<GetCompanyDetails>();
                    while (reader.Read())
                    {
                        GetCompanyDetails company = new GetCompanyDetails
                        {
                            CompanyId = Convert.ToInt32(reader["CompanyId"]),
                            CompanyName = reader["CompanyName"].ToString(),
                            VATRegNo = reader["VATRegNo"].ToString(),
                            TINNo = reader["TINNo"].ToString(),
                            TradeLicenseNo = reader["TradeLicenseNo"].ToString(),
                            Address = reader["Address"].ToString(),
                            PhoneNo = reader["PhoneNo"].ToString(),
                            Email = reader["Email"].ToString(),
                            Logo = reader["Logo"].ToString(),
                            IsMaster = reader["IsMaster"] != DBNull.Value ? Convert.ToBoolean(reader["IsMaster"]) : false,
                            IsActive = reader["IsActive"] != DBNull.Value ? Convert.ToBoolean(reader["IsActive"]) : false,
                        };
                        companies.Add(company);
                    }

                    connection.Close();

                    if (companies.Count > 0)
                    {
                        return Ok(companies);
                    }
                    else
                    {
                        return NotFound(new { message = "No Companies found" });
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
