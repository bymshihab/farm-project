using FarmProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace FarmProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string ConnectionString;

        public CategoryController(IConfiguration configuration)
        {
            _configuration = configuration;
            ConnectionString = _configuration.GetConnectionString("DefaultConnection");
        }
        [HttpPost("CreateCategory")]
        public IActionResult CreateCategory(Category category)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();

                    SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM Categories WHERE CategoryName = @CategoryName", con);
                    checkCmd.Parameters.AddWithValue("@CategoryName", category.CategoryName);
                    int count = (int)checkCmd.ExecuteScalar();

                    if (count > 0)
                    {
                        return BadRequest(new { message = "Categories already exists" });
                    }
                    else
                    {
                        using (SqlCommand insertCmd = new SqlCommand("INSERT INTO Categories (CategoryName, CategoryDescription, CategoryStatus,CompanyId, AddedBy, DateAdded, AddedPc, UpdateBy, DateUpdatedBy, UpdatePc) VALUES (@CategoryName, @CategoryDescription, @CategoryStatus,@CompanyId, @AddedBy, @DateAdded, @AddedPc, @UpdateBy, @DateUpdatedBy, @UpdatePc)", con))
                        {
                            insertCmd.Parameters.AddWithValue("@CategoryName", category.CategoryName);
                            insertCmd.Parameters.AddWithValue("@CategoryDescription", category.CategoryDescription ?? (object)DBNull.Value);
                            insertCmd.Parameters.AddWithValue("@CategoryStatus", category.CategoryStatus);
                            insertCmd.Parameters.AddWithValue("@CompanyId", category.CompanyId);
                            insertCmd.Parameters.AddWithValue("@AddedBy", category.AddedBy);
                            insertCmd.Parameters.AddWithValue("@DateAdded", category.DateAdded);
                            insertCmd.Parameters.AddWithValue("@AddedPc", category.AddedPc);
                            insertCmd.Parameters.AddWithValue("@UpdateBy", category.UpdateBy);
                            insertCmd.Parameters.AddWithValue("@DateUpdatedBy", category.DateUpdatedBy);
                            insertCmd.Parameters.AddWithValue("@UpdatePc", category.UpdatePc);

                            insertCmd.ExecuteNonQuery();
                        }

                        con.Close();

                        return Ok(new { message = "Category created successfully" });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing the request", error = ex.Message });
            }
        }

        [HttpPut("UpdateCategory/{CategoryId}")]
        public IActionResult UpdateCategory(Category category, int CategoryId)
        {
            try
            {
                if (category == null)
                {
                    return BadRequest(new { message = "Invalid Category data" });
                }

                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();

                    SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM Categories WHERE CategoryId = @CategoryId", con);
                    checkCmd.Parameters.AddWithValue("@CategoryId", CategoryId);
                    int count = (int)checkCmd.ExecuteScalar();

                    if (count == 0)
                    {
                        return NotFound(new { message = "Category not found" });
                    }

                    using (SqlCommand updateCmd = new SqlCommand("UPDATE Categories SET CategoryName = @CategoryName, CategoryDescription = @CategoryDescription, CategoryStatus = @CategoryStatus, UpdateBy = @UpdateBy, DateUpdatedBy = @DateUpdatedBy, UpdatePc = @UpdatePc WHERE CategoryId = @CategoryId", con))
                    {
                        updateCmd.Parameters.AddWithValue("@CategoryId", CategoryId);
                        updateCmd.Parameters.AddWithValue("@CategoryName", category.CategoryName);
                        updateCmd.Parameters.AddWithValue("@CategoryDescription", category.CategoryDescription);
                        updateCmd.Parameters.AddWithValue("@CategoryStatus", category.CategoryStatus);
                        updateCmd.Parameters.AddWithValue("@UpdateBy", category.UpdateBy);
                        updateCmd.Parameters.AddWithValue("@DateUpdatedBy", category.DateUpdatedBy);
                        updateCmd.Parameters.AddWithValue("@UpdatePc", category.UpdatePc);

                        updateCmd.ExecuteNonQuery();
                    }

                    con.Close();

                    return Ok(new { message = "Category updated successfully" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing the request", error = ex.Message });
            }
        }
        [HttpDelete("DeleteCategory/{CategoryId}")]
        public IActionResult DeleteCategory(int CategoryId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    SqlCommand command = new SqlCommand("DELETE FROM Categories WHERE CategoryId = @CategoryId", connection);
                    command.CommandType = CommandType.Text;

                    command.Parameters.AddWithValue("@CategoryId", CategoryId);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    connection.Close();

                    if (rowsAffected > 0)
                    {
                        return Ok(new { message = "Category deleted successfully" });
                    }
                    else
                    {
                        return NotFound(new { message = "Category not found" });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing the request", error = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetCategories()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    SqlCommand getCompanyCmd = new SqlCommand("SELECT CategoryId, CategoryName, CategoryDescription, CategoryStatus FROM Categories", connection);

                    connection.Open();
                    SqlDataReader reader = getCompanyCmd.ExecuteReader();
                    List<Category> categories = new List<Category>();
                    while (reader.Read())
                    {
                        Category category = new Category
                        {
                            CategoryId = Convert.ToInt32(reader["CategoryId"]),
                            CategoryName = reader["CategoryName"].ToString(),
                            CategoryDescription = reader["CategoryDescription"].ToString(),
                            CategoryStatus = reader["CategoryStatus"] != DBNull.Value ? Convert.ToBoolean(reader["CategoryStatus"]) : false,
                        };
                        categories.Add(category);
                    }

                    connection.Close();

                    if (categories.Count > 0)
                    {
                        return Ok(categories);
                    }
                    else
                    {
                        return NotFound(new { message = "No Categories Here" });
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
