namespace FarmProject.Models
{
    public class Company
    {
        public int? CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string? VATRegNo { get; set; }
        public string? TINNo { get; set; }
        public string? TradeLicenseNo { get; set; }
        public string Address { get; set; }
        public string PhoneNo { get; set; }
        public string? Email { get; set; }
        public string? Logo { get; set; }
        public IFormFile? ImageFile { get; set; }
        public bool? IsMaster { get; set; } = false;
        public bool? IsActive { get; set; } = false;
        public string? AddedBy { get; set; } = "User";
        public DateTime? DateAdded { get; set; } = DateTime.Now;
        public string? AddedPC { get; set; } = "UserPC";
        public string? UpdatedBy { get; set; } = "User";
        public DateTime? DateUpdated { get; set; } = DateTime.Now;
        public string? UpdatedPC { get; set; } = "UserPC";
    }
}
