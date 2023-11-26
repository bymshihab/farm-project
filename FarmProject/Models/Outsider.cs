namespace FarmProject.Models
{
    public class Outsider
    {
        public int OutsiderId { get; set; }
        public string OutsiderName { get; set; }
        public string OutsiderCatagory { get; set; }
        public string OutsiderAddress { get; set; }
        public string PhoneNumber { get; set; }
        public int? CompanyId { get; set; }
        public string? AddedBy { get; set; } = "appUser";
        public DateTime? AddedDate { get; set; } = DateTime.Now;
        public string? AddedPc { get; set; } = "Default App User";
        public string? UpdatedBy { get; set; } = "appUser";
        public DateTime? UpdatedDate { get; set; } = DateTime.Now;
        public string? UpDatedPc { get; set; } = "Default App User";
    }
}
