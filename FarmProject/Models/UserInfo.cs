namespace FarmProject.Models
{
    public class UserInfo
    {
        public string UserCode { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int? EmployeeId { get; set; }
        public string? Email { get; set; }
        public int CompanyId { get; set; }
        public bool? IsAdmin { get; set; }=false;
        public bool? IsAudit { get; set; } = false;
        public bool? IsActive { get; set; } = true;
        public string? AddedBy { get; set; } = "User";
        public DateTime? DateAdded { get; set; }=DateTime.Now;
        public string? AddedPC { get; set; } = "User";
        public string? UpdatedBy { get; set; } = "User";
        public DateTime? DateUpdated { get; set; }= DateTime.Now;
        public string? UpdatedPC { get; set; } = "User";
    }
}
