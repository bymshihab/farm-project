using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace FarmProject.Models
{
    public class Employee
    {
        public int? EId { get; set; }
        public string? EmployeeCode { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? MiddleName { get; set; }
        public double? Salary { get; set; }
        public DateTime? DOB { get; set; }
        public DateTime? HireDate { get; set; }= DateTime.Now;
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public long? NID { get; set; }
        public string? Description { get; set; }
        public string? JobTitle { get; set; }
        public bool Status { get; set; } = true;
        public string? EmployeeImage { get; set; }
        public IFormFile? ImageFile { get; set; }
        public int? CompanyId { get; set; }
        public string? AddedBy { get; set; } = "appUser";
        public DateTime? AddedDate { get; set; } = DateTime.Now;
        public string? AddedPc { get; set; } = "Default AppUser";
        public string? UpdatedBy { get; set; } = "appUser";
        public DateTime? UpdatedDate { get; set; } = DateTime.Now;
        public string? UpDatedPc { get; set; } = "Default App User";
    }
}
