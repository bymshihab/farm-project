using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;

namespace FarmProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActiveGETMilksController : ControllerBase
    {

        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        public ActiveGETMilksController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        [HttpGet]
        public IActionResult GetActiveCompany()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("GETMilks", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();

                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        var result = new List<Dictionary<string, object>>();
                        foreach (DataRow row in dataTable.Rows)
                        {
                            var rowData = new Dictionary<string, object>();
                            foreach (DataColumn column in dataTable.Columns)
                            {
                                rowData[column.ColumnName] = row[column];
                            }
                            result.Add(rowData);
                        }

                        return Ok(result);
                    }
                }
            }
        }
    }
}
