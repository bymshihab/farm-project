using FarmProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace FarmProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserInfoController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly SqlConnection con;

        public UserInfoController(IConfiguration configuration)
        {
            _configuration = configuration;
            con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        }

        [HttpPost("CreateUser")]
        public IActionResult CreateUser([FromForm] UserInfo user)
        {
            try
            {
                if (user == null)
                {
                    return BadRequest(new { message = "Invalid user data" });
                }

                string connectionString = _configuration.GetConnectionString("DefaultConnection");

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM UserInfo WHERE UserCode = @UserCode", con);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@UserCode", user.UserCode);

                    con.Open();
                    int count = (int)cmd.ExecuteScalar();
                    con.Close();

                    if (count > 0)
                    {
                        return BadRequest(new { message = "User already exists" });
                    }
                    else
                    {
                        string encryptedPassword = EncryptPassword(user.Password);

                        using (SqlCommand insertCmd = new SqlCommand("INSERT INTO UserInfo (UserCode, UserName, Password, EmployeeId, Email, CompanyId, IsAdmin, IsAudit, IsActive, AddedBy, DateAdded, AddedPC, UpdatedBy, DateUpdated, UpdatedPC) VALUES (@UserCode, @UserName, @Password, @EmployeeId, @Email, @CompanyId, @IsAdmin, @IsAudit, @IsActive, @AddedBy, @DateAdded, @AddedPC, @UpdatedBy, @DateUpdated, @UpdatedPC)", con))
                        {
                            insertCmd.CommandType = CommandType.Text;

                            insertCmd.Parameters.AddWithValue("@UserCode", user.UserCode);
                            insertCmd.Parameters.AddWithValue("@UserName", (user.UserName?.Length <= 200) ? user.UserName : user.UserName?.Substring(0, 200));
                            insertCmd.Parameters.AddWithValue("@Password", encryptedPassword);
                            insertCmd.Parameters.AddWithValue("@EmployeeId", user.EmployeeId.HasValue ? user.EmployeeId.Value : (object)DBNull.Value);
                            insertCmd.Parameters.AddWithValue("@Email", (user.Email?.Length <= 100) ? user.Email : user.Email?.Substring(0, 100) ?? (object)DBNull.Value);
                            insertCmd.Parameters.AddWithValue("@CompanyId", user.CompanyId);

                            insertCmd.Parameters.AddWithValue("@IsAdmin", user.IsAdmin);
                            insertCmd.Parameters.AddWithValue("@IsAudit", user.IsAudit);
                            insertCmd.Parameters.AddWithValue("@IsActive", user.IsActive);

                            insertCmd.Parameters.AddWithValue("@AddedBy", user.AddedBy ?? (object)DBNull.Value);
                            insertCmd.Parameters.AddWithValue("@DateAdded", user.DateAdded ?? (object)DBNull.Value);
                            insertCmd.Parameters.AddWithValue("@AddedPC", user.AddedPC ?? (object)DBNull.Value);
                            insertCmd.Parameters.AddWithValue("@UpdatedBy", user.UpdatedBy ?? (object)DBNull.Value);
                            insertCmd.Parameters.AddWithValue("@DateUpdated", user.DateUpdated ?? (object)DBNull.Value);
                            insertCmd.Parameters.AddWithValue("@UpdatedPC", user.UpdatedPC ?? (object)DBNull.Value);

                            con.Open();
                            insertCmd.ExecuteNonQuery();
                            con.Close();

                            string encryptedUserCode = EncryptPassword(user.UserCode);
                            return Ok(new { message = "User created successfully", encryptedUserCode });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing the request", error = ex.Message });
            }
        }


        [HttpPut("UpdateUser/{UserCode}")]
        public IActionResult UpdateUser([FromForm] UserInfo user, string UserCode)
        {
            try
            {
                if (user == null)
                {
                    return BadRequest(new { message = "Invalid user data" });
                }

                string connectionString = _configuration.GetConnectionString("DefaultConnection");

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    // Check if the user with the specified UserCode exists
                    SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM UserInfo WHERE UserCode = @UserCode", con);
                    checkCmd.Parameters.AddWithValue("@UserCode", UserCode);
                    int count = (int)checkCmd.ExecuteScalar();

                    if (count == 0)
                    {
                        return NotFound(new { message = "User not found" });
                    }

                    string encryptedPassword = EncryptPassword(user.Password);

                    // Update command
                    // Update command
                    using (SqlCommand updateCmd = new SqlCommand("UPDATE UserInfo SET UserName = @UserName, Password = @Password, EmployeeId = @EmployeeId, Email = @Email, CompanyId = @CompanyId, IsAdmin = @IsAdmin, IsAudit = @IsAudit, IsActive = @IsActive, UpdatedBy = @UpdatedBy, DateUpdated = @DateUpdated, UpdatedPC = @UpdatedPC WHERE UserCode = @UserCode", con))
                    {
                        updateCmd.Parameters.AddWithValue("@UserCode", UserCode);
                        updateCmd.Parameters.AddWithValue("@UserName", (user.UserName?.Length <= 200) ? user.UserName : user.UserName?.Substring(0, 200));
                        updateCmd.Parameters.AddWithValue("@Password", encryptedPassword);
                        updateCmd.Parameters.AddWithValue("@EmployeeId", user.EmployeeId.HasValue ? user.EmployeeId.Value : (object)DBNull.Value);
                        updateCmd.Parameters.AddWithValue("@Email", (user.Email?.Length <= 100) ? user.Email : user.Email?.Substring(0, 100) ?? (object)DBNull.Value);
                        updateCmd.Parameters.AddWithValue("@CompanyId", user.CompanyId);
                        updateCmd.Parameters.AddWithValue("@IsAdmin", user.IsAdmin);
                        updateCmd.Parameters.AddWithValue("@IsAudit", user.IsAudit);
                        updateCmd.Parameters.AddWithValue("@IsActive", user.IsActive);
                        updateCmd.Parameters.AddWithValue("@UpdatedBy", user.UpdatedBy ?? (object)DBNull.Value);
                        updateCmd.Parameters.AddWithValue("@DateUpdated", user.DateUpdated ?? (object)DBNull.Value);
                        updateCmd.Parameters.AddWithValue("@UpdatedPC", user.UpdatedPC ?? (object)DBNull.Value);

                        updateCmd.ExecuteNonQuery();
                    }


                    con.Close();

                    return Ok(new { message = "User updated successfully" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing the request", error = ex.Message });
            }
        }
        [HttpDelete("DeleteUser/{userCode}")]
        public IActionResult DeleteUser(string userCode)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                SqlCommand command = new SqlCommand("spDeleteUser", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UserCode", userCode);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    return Ok(new { message = "User deleted successfully" });
                }
                else
                {
                    return NotFound(new { message = "User not found" });
                }
            }
        }

        [HttpPost]
        [Route("login")]
        public IActionResult LoginUser([FromForm] UserLogIn user)

        {

            string encryptedPassword = EncryptPassword(user.Password);
            SqlCommand cmd = new SqlCommand("SELECT * FROM UserInfo WHERE UserCode = @UserCode AND password = @password AND CompanyId = @CompanyId", con);

            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@UserCode", user.UserCode);
            cmd.Parameters.AddWithValue("@password", encryptedPassword);
            cmd.Parameters.AddWithValue("@CompanyId", user.CompanyId);
            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                string encryptedUserCode = reader["UserCode"].ToString();

                con.Close();
                return Ok(new { message = "Login successful", encryptedUserCode });
            }
            else
            {
                con.Close();
                return BadRequest(new { message = "Invalid User Name or password" });
            }
        }


        public static string EncryptPassword(string clearText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }
        ////DecryptPassword
        private string DecryptPassword(string cipherText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            try
            {
                byte[] cipherBytes = Convert.FromBase64String(cipherText);
                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(cipherBytes, 0, cipherBytes.Length);
                            cs.Close();
                        }
                        cipherText = Encoding.Unicode.GetString(ms.ToArray());
                    }
                }
                return cipherText;
            }
            catch (FormatException ex)
            {
                return "Decryption failed: " + ex.Message;
            }
            catch (Exception ex)
            {
                return "An error occurred during decryption: " + ex.Message;
            }
        }
    }
}
