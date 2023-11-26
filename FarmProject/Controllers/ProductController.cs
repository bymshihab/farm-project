using FarmProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace FarmProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string ConnectionString;

        public ProductController(IConfiguration configuration)
        {
            _configuration = configuration;
            ConnectionString = _configuration.GetConnectionString("DefaultConnection");
        }
        [HttpPost("Createproduct")]
        public IActionResult CreateSupplier(Product product)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();

                    SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM Products WHERE ProductName = @ProductName", con);
                    checkCmd.Parameters.AddWithValue("@ProductName", product.ProductName);
                    int count = (int)checkCmd.ExecuteScalar();

                    if (count > 0)
                    {
                        return BadRequest(new { message = "Product already exists" });
                    }
                    else
                    {
                        using (SqlCommand insertCmd = new SqlCommand("INSERT INTO Products (ProductName, ProductDescription, Status, Price, CategoryId, UomId,CompanyId, AddedBy, AddedDate, AddedPc, UpdatedBy, UpdatedDate, UpdatedPc) VALUES (@ProductName, @ProductDescription, @Status, @Price, @CategoryId, @UomId,@CompanyId, @AddedBy, @AddedDate, @AddedPc, @UpdatedBy, @UpdatedDate, @UpdatedPc)", con))
                        {
                            insertCmd.Parameters.AddWithValue("@ProductName", product.ProductName);
                            insertCmd.Parameters.AddWithValue("@ProductDescription", product.ProductDescription ?? (object)DBNull.Value);
                            insertCmd.Parameters.AddWithValue("@Status", product.Status);
                            insertCmd.Parameters.AddWithValue("@Price", product.Price ?? (object)DBNull.Value);
                            insertCmd.Parameters.AddWithValue("@CategoryId", product.CategoryId ?? (object)DBNull.Value);
                            insertCmd.Parameters.AddWithValue("@UomId", product.UomId ?? (object)DBNull.Value);
                            insertCmd.Parameters.AddWithValue("@CompanyId", product.CompanyId);
                            insertCmd.Parameters.AddWithValue("@AddedBy", product.AddedBy);
                            insertCmd.Parameters.AddWithValue("@AddedDate", product.AddedDate);
                            insertCmd.Parameters.AddWithValue("@AddedPc", product.AddedPc);
                            insertCmd.Parameters.AddWithValue("@UpdatedBy", product.UpdatedBy);
                            insertCmd.Parameters.AddWithValue("@UpdatedDate", product.UpdatedDate);
                            insertCmd.Parameters.AddWithValue("@UpdatedPc", product.UpDatedPc);

                            insertCmd.ExecuteNonQuery();
                        }

                        con.Close();

                        return Ok(new { message = "product created successfully" });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing the request", error = ex.Message });
            }
        }
        [HttpPut("UpdateProduct")]
        public IActionResult UpdateProduct(Product product)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM Products WHERE ProductId = @ProductId", con);
                    checkCmd.Parameters.AddWithValue("@ProductId", product.ProductId);
                    int count = (int)checkCmd.ExecuteScalar();

                    if (count == 0)
                    {
                        return NotFound(new { message = $"Product with ProductName {product.ProductName} not found" });
                    }
                    using (SqlCommand updateCmd = new SqlCommand("UPDATE Products SET ProductName = @ProductName, ProductDescription = @ProductDescription, Status = @Status, Price = @Price, CategoryId = @CategoryId, UomId = @UomId, UpdatedBy = @UpdatedBy, UpdatedDate = @UpdatedDate, UpdatedPc = @UpdatedPc WHERE ProductId = @ProductId", con))
                    {
                        updateCmd.Parameters.AddWithValue("@ProductId", product.ProductId);
                        updateCmd.Parameters.AddWithValue("@ProductName", product.ProductName);
                        updateCmd.Parameters.AddWithValue("@ProductDescription", product.ProductDescription ?? (object)DBNull.Value);
                        updateCmd.Parameters.AddWithValue("@Status", product.Status);
                        updateCmd.Parameters.AddWithValue("@Price", product.Price ?? (object)DBNull.Value);
                        updateCmd.Parameters.AddWithValue("@CategoryId", product.CategoryId ?? (object)DBNull.Value);
                        updateCmd.Parameters.AddWithValue("@UomId", product.UomId ?? (object)DBNull.Value);
                        updateCmd.Parameters.AddWithValue("@UpdatedBy", product.UpdatedBy);
                        updateCmd.Parameters.AddWithValue("@UpdatedDate", DateTime.Now);
                        updateCmd.Parameters.AddWithValue("@UpdatedPc", product.UpDatedPc);

                        updateCmd.ExecuteNonQuery();
                    }

                    return Ok(new { message = $"Product with ProductId {product.ProductName} updated successfully" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing the request", error = ex.Message });
            }
        }
        [HttpDelete("DeleteProduct/{productId}")]
        public IActionResult DeleteProduct(int productId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM Products WHERE ProductId = @ProductId", con);
                    checkCmd.Parameters.AddWithValue("@ProductId", productId);
                    int count = (int)checkCmd.ExecuteScalar();
                    if (count == 0)
                    {
                        return NotFound(new { message = $"Product not found" });
                    }
                    using (SqlCommand deleteCmd = new SqlCommand("DELETE FROM Products WHERE ProductId = @ProductId", con))
                    {
                        deleteCmd.Parameters.AddWithValue("@ProductId", productId);
                        deleteCmd.ExecuteNonQuery();
                    }
                    return Ok(new { message = $"Product Data deleted successfully" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing the request", error = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetProduct()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    SqlCommand getCompanyCmd = new SqlCommand("GetProductDetails", connection);

                    connection.Open();
                    SqlDataReader reader = getCompanyCmd.ExecuteReader();
                    List<ProductGet> products= new List<ProductGet>();
                    while (reader.Read())
                    {
                        ProductGet product = new ProductGet
                        {
                            ProductId = Convert.ToInt32(reader["ProductId"]),
                            CategoryName = reader["CategoryName"].ToString(),
                             ProductName= reader["ProductName"].ToString(),
                            ProductDescription = reader["ProductDescription"].ToString(),
                            Price = Convert.ToInt32(reader["Price"]),
                            UomName = reader["UomName"].ToString(),

                            Status = reader["Status"] != DBNull.Value ? Convert.ToBoolean(reader["Status"]) : false
                        };
                        products.Add(product);
                    }

                    connection.Close();

                    if (products.Count > 0)
                    {
                        return Ok(products);
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
