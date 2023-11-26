using FarmProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;

namespace FarmProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnimalController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string ConnectionString;

        public AnimalController(IConfiguration configuration)
        {
            _configuration = configuration;
            ConnectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        [HttpPost("CreateAnimal")]
        public async Task<IActionResult> CreateAnimal([FromForm] Animal animal)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    string imageFileName = null;
                    if (animal.ImageFile != null)
                    {
                        imageFileName = $"{Path.GetFileNameWithoutExtension(animal.ImageFile.FileName)}{Path.GetExtension(animal.ImageFile.FileName)}";
                        string imagePath = Path.Combine("AnimalImage", imageFileName);
                        using (Stream stream = new FileStream(imagePath, FileMode.Create))
                        {
                            await animal.ImageFile.CopyToAsync(stream);
                        }
                    }

                    string systemCode = string.Empty;
                    SqlCommand cmdSP = new SqlCommand("spMakeSystemCode", con);
                    {
                        cmdSP.CommandType = CommandType.StoredProcedure;
                        cmdSP.Parameters.AddWithValue("@TableName", "Animals");
                        cmdSP.Parameters.AddWithValue("@Date", DateTime.Now.ToString("yyyy-MM-dd"));
                        cmdSP.Parameters.AddWithValue("@AddNumber", 1);
                        systemCode = cmdSP.ExecuteScalar()?.ToString();
                    }

                    animal.AnimalTagNo = systemCode.Split('%')[1];

                    string qrCodeDataString = $"{animal.AnimalId},{animal.AnimalName},{animal.AnimalTagNo},{animal.ProductId},{animal.ShedId},{animal.IsDead},{animal.IsSold},{animal.MilkId},{animal.weight},{animal.DOB},{animal.GenderId},{animal.IsVaccinated},{animal.Status}";
                    if (!string.IsNullOrEmpty(qrCodeDataString))
                    {
                        QRCodeGenerator qrGenerator = new QRCodeGenerator();
                        QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrCodeDataString, QRCodeGenerator.ECCLevel.Q);
                        QRCode qrCode = new QRCode(qrCodeData);
                        Bitmap qrCodeImage = qrCode.GetGraphic(20);


                        using (SqlCommand insertCmd = new SqlCommand("INSERT INTO Animals (AnimalName, AnimalTagNo, ProductId, ShedId, IsDead, IsSold, IsVaccinated, QRCodeData, MilkId, Weight, DOB, GenderId, Status, CompanyId, AnimalImage, AddedBy, AddedDate, AddedPc, UpdatedBy, UpdatedDate, UpdatedPc) VALUES (@AnimalName, @AnimalTagNo, @ProductId, @ShedId, @IsDead, @IsSold, @IsVaccinated, @QRCodeData, @MilkId, @Weight, @DOB, @GenderId, @Status, @CompanyId, @AnimalImage, @AddedBy, @AddedDate, @AddedPc, @UpdatedBy, @UpdatedDate, @UpdatedPc)", con))
                        {
                            insertCmd.Parameters.AddWithValue("@AnimalName", animal.AnimalName);
                            insertCmd.Parameters.AddWithValue("@AnimalTagNo", animal.AnimalTagNo ?? (object)DBNull.Value);
                            insertCmd.Parameters.AddWithValue("@ProductId", animal.ProductId);
                            insertCmd.Parameters.AddWithValue("@ShedId", animal.ShedId);
                            insertCmd.Parameters.AddWithValue("@IsDead", animal.IsDead);
                            insertCmd.Parameters.AddWithValue("@IsSold", animal.IsSold);
                            insertCmd.Parameters.AddWithValue("@IsVaccinated", animal.IsVaccinated);
                            insertCmd.Parameters.AddWithValue("@QRCodeData", qrCodeDataString);
                            insertCmd.Parameters.AddWithValue("@MilkId", animal.MilkId != null ? (object)animal.MilkId : DBNull.Value);
                            insertCmd.Parameters.AddWithValue("@Weight", animal.weight);
                            insertCmd.Parameters.AddWithValue("@DOB", animal.DOB != null ? (object)animal.DOB : DBNull.Value);
                            insertCmd.Parameters.AddWithValue("@GenderId", animal.GenderId);
                            insertCmd.Parameters.AddWithValue("@Status", animal.Status);
                            insertCmd.Parameters.AddWithValue("@CompanyId", animal.CompanyId);

                            object logoValue = (object)imageFileName ?? DBNull.Value;
                            insertCmd.Parameters.AddWithValue("@AnimalImage", logoValue);
                            insertCmd.Parameters.AddWithValue("@AddedBy", animal.AddedBy ?? (object)DBNull.Value);
                            insertCmd.Parameters.AddWithValue("@AddedDate", animal.AddedDate != null ? (object)animal.AddedDate : DBNull.Value);
                            insertCmd.Parameters.AddWithValue("@AddedPc", animal.AddedPc ?? (object)DBNull.Value);
                            insertCmd.Parameters.AddWithValue("@UpdatedBy", animal.UpdatedBy ?? (object)DBNull.Value);
                            insertCmd.Parameters.AddWithValue("@UpdatedDate", animal.UpdatedDate != null ? (object)animal.UpdatedDate : DBNull.Value);
                            insertCmd.Parameters.AddWithValue("@UpDatedPc", animal.UpDatedPc ?? (object)DBNull.Value);

                            insertCmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        // Handle the case where QRCodeData is null or empty
                        return BadRequest(new { message = "QRCodeData is null or empty" });
                    }

                    return Ok(new { message = "Animal created successfully" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing the request", error = ex.Message });
            }
        }


        [HttpPut("UpdateAnimal/{id}")]
        public async Task<IActionResult> UpdateAnimal([FromForm] Animal animal)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();

                    SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM Animals WHERE AnimalId = @AnimalId", con);
                    checkCmd.Parameters.AddWithValue("@AnimalId", animal.AnimalId);

                    int animalCount = (int)checkCmd.ExecuteScalar();

                    if (animalCount == 0)
                    {
                        return NotFound(new { message = $"Animal with ID {animal.AnimalId} not found" });
                    }

                    string imageFileName = null;

                    // Check if a new image is provided
                    if (animal.ImageFile != null)
                    {
                        imageFileName = $"{Path.GetFileNameWithoutExtension(animal.ImageFile.FileName)}{Path.GetExtension(animal.ImageFile.FileName)}";
                        string imagePath = Path.Combine("AnimalImage", imageFileName);
                        using (Stream stream = new FileStream(imagePath, FileMode.Create))
                        {
                            await animal.ImageFile.CopyToAsync(stream);
                        }
                    }
                    else
                    {
                        // If no new image is provided, retain the existing image file name
                        SqlCommand getImageCmd = new SqlCommand("SELECT AnimalImage FROM Animals WHERE AnimalId = @AnimalId", con);
                        getImageCmd.Parameters.AddWithValue("@AnimalId", animal.AnimalId);
                        imageFileName = getImageCmd.ExecuteScalar() as string;
                    }

                    string qrCodeDataString = $"{animal.AnimalId},{animal.AnimalName},{animal.AnimalTagNo},{animal.ProductId},{animal.ShedId},{animal.IsDead},{animal.IsSold},{animal.MilkId},{animal.weight},{animal.DOB},{animal.GenderId},{animal.IsVaccinated},{animal.Status}";
                    if (!string.IsNullOrEmpty(qrCodeDataString))
                    {
                        QRCodeGenerator qrGenerator = new QRCodeGenerator();
                        QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrCodeDataString, QRCodeGenerator.ECCLevel.Q);
                        QRCode qrCode = new QRCode(qrCodeData);
                        Bitmap qrCodeImage = qrCode.GetGraphic(20);

                        using (SqlCommand updateCmd = new SqlCommand("UPDATE Animals SET AnimalName = @AnimalName, ProductId = @ProductId, ShedId = @ShedId, IsDead = @IsDead, IsSold = @IsSold, IsVaccinated = @IsVaccinated, QRCodeData = @QRCodeData, MilkId = @MilkId, Weight = @Weight, DOB = @DOB, GenderId = @GenderId, Status = @Status, CompanyId = @CompanyId, AnimalImage = @AnimalImage, UpdatedBy = @UpdatedBy, UpdatedDate = @UpdatedDate, UpdatedPc = @UpdatedPc WHERE AnimalId = @AnimalId", con))
                        {
                            updateCmd.Parameters.AddWithValue("@AnimalId", animal.AnimalId);
                            updateCmd.Parameters.AddWithValue("@AnimalName", animal.AnimalName);
                            updateCmd.Parameters.AddWithValue("@ProductId", animal.ProductId);
                            updateCmd.Parameters.AddWithValue("@ShedId", animal.ShedId);
                            updateCmd.Parameters.AddWithValue("@IsDead", animal.IsDead);
                            updateCmd.Parameters.AddWithValue("@IsSold", animal.IsSold);
                            updateCmd.Parameters.AddWithValue("@IsVaccinated", animal.IsVaccinated);
                            updateCmd.Parameters.AddWithValue("@QRCodeData", qrCodeDataString);
                            updateCmd.Parameters.AddWithValue("@MilkId", animal.MilkId != null ? (object)animal.MilkId : DBNull.Value);
                            updateCmd.Parameters.AddWithValue("@Weight", animal.weight);
                            updateCmd.Parameters.AddWithValue("@DOB", animal.DOB != null ? (object)animal.DOB : DBNull.Value);
                            updateCmd.Parameters.AddWithValue("@GenderId", animal.GenderId);
                            updateCmd.Parameters.AddWithValue("@Status", animal.Status);
                            updateCmd.Parameters.AddWithValue("@CompanyId", animal.CompanyId);

                            object logoValue = (object)imageFileName ?? DBNull.Value;
                            updateCmd.Parameters.AddWithValue("@AnimalImage", logoValue);
                            updateCmd.Parameters.AddWithValue("@UpdatedBy", animal.UpdatedBy ?? (object)DBNull.Value);
                            updateCmd.Parameters.AddWithValue("@UpdatedDate", DateTime.Now);
                            updateCmd.Parameters.AddWithValue("@UpdatedPc", animal.UpDatedPc ?? (object)DBNull.Value);

                            updateCmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        return BadRequest(new { message = "QRCodeData is null or empty" });
                    }

                    return Ok(new { message = $"Animal with ID {animal.AnimalId} updated successfully" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing the request", error = ex.Message });
            }
        }

        [HttpDelete("DeleteAnimal/{id}")]
        public IActionResult DeleteAnimal(int id)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();

                    SqlCommand checkCmd = new SqlCommand("SELECT AnimalImage FROM Animals WHERE AnimalId = @AnimalId", con);
                    checkCmd.Parameters.AddWithValue("@AnimalId", id);

                    string imageFileName = checkCmd.ExecuteScalar() as string;

                    if (imageFileName == null)
                    {
                        return NotFound(new { message = $"Animal with ID {id} not found" });
                    }
                    using (SqlCommand deleteCmd = new SqlCommand("DELETE FROM Animals WHERE AnimalId = @AnimalId", con))
                    {
                        deleteCmd.Parameters.AddWithValue("@AnimalId", id);
                        deleteCmd.ExecuteNonQuery();
                    }

                    // Delete the associated image file
                    if (!string.IsNullOrEmpty(imageFileName))
                    {
                        string imagePath = Path.Combine("AnimalImage", imageFileName);
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }

                    return Ok(new { message = $"Animal with ID {id} and associated image deleted successfully" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing the request", error = ex.Message });
            }
        }
        [HttpGet("GetAnimal")]
        public IActionResult GetAnimal(int AnimalId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();

                    SqlCommand getAnimalCmd = new SqlCommand("GetAnimalById", con);
                    getAnimalCmd.CommandType = CommandType.StoredProcedure;  // Set command type to stored procedure
                    getAnimalCmd.Parameters.AddWithValue("@AnimalId", AnimalId);

                    SqlDataReader reader = getAnimalCmd.ExecuteReader();

                    if (reader.Read())
                    {
                        AnimalQR animal = new AnimalQR
                        {
                            AnimalId = (int)reader["AnimalId"],
                            AnimalName = reader["AnimalName"].ToString(),
                            AnimalTagNo = reader["AnimalTagNo"].ToString(),
                            ProductName = reader["ProductName"].ToString(),
                            ShedName = reader["ShedName"].ToString(),
                            IsDead = (bool)reader["IsDead"],
                            IsSold = (bool)reader["IsSold"],
                            IsVaccinated = (bool)reader["IsVaccinated"],
                            QRCodeData = reader["QRCodeData"].ToString(),
                            MilkId = reader["MilkId"] != DBNull.Value ? (int?)reader["MilkId"] : null,
                            weight = reader["Weight"] != DBNull.Value ? (decimal?)reader["Weight"] : null,
                            DOB = reader["DOB"] != DBNull.Value ? (DateTime?)reader["DOB"] : null,
                            GenderType = reader["GenderType"].ToString(),
                            Status = reader["Status"].ToString()
                        };

                        if (reader["AnimalImage"] != DBNull.Value)
                        {
                            try
                            {
                                string base64Image = reader["AnimalImage"].ToString();
                                Console.WriteLine($"Base64 Image: {base64Image}"); // Debugging line

                                byte[] imageData = Convert.FromBase64String(base64Image);
                                animal.AnimalImageBase64 = Convert.ToBase64String(imageData);
                            }
                            catch (Exception ex)
                            {
                                return BadRequest(new { message = "Error decoding base64 image", error = ex.Message });
                            }
                        }

                        QRCodeGenerator qrGenerator = new QRCodeGenerator();
                        QRCodeData qrCodeData = qrGenerator.CreateQrCode(animal.QRCodeData, QRCodeGenerator.ECCLevel.Q);
                        QRCode qrCode = new QRCode(qrCodeData);
                        Bitmap qrCodeImage = qrCode.GetGraphic(20);

                        using (MemoryStream stream = new MemoryStream())
                        {
                            qrCodeImage.Save(stream, ImageFormat.Png);
                            byte[] qrCodeImageData = stream.ToArray();
                            string base64QRCodeImage = Convert.ToBase64String(qrCodeImageData);
                            animal.QRCodeImageBase64 = base64QRCodeImage;
                        }

                        return Ok(animal);
                    }
                    else
                    {
                        return NotFound(new { message = $"Animal with ID {AnimalId} not found" });
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
