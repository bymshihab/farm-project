namespace FarmProject.Models
{
    public class GetCompanyDetails
    {
        public int? CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string? VATRegNo { get; set; }
        public string? TINNo { get; set; }
        public string? TradeLicenseNo { get; set; }
        public string? Address { get; set; }
        public string? PhoneNo { get; set; }
        public string? Email { get; set; }
        //public string? imageFile { get; set; }
        public string? Logo { get; set; }
        public bool? IsMaster { get; set; } = false;
        public bool? IsActive { get; set; } = false;
    }
}
