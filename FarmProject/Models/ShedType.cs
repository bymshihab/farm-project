namespace FarmProject.Models
{
    public class ShedType
    {
        public int shedTypeId { get; set; }
        public string shedTypeName { get; set; }
        public string ShedTypeDescription { get; set; }
        public bool Status { get; set; } = false;
        public int? CompanyId { get; set; }
        public string? AddedBy { get; set; } = "appUser";
        public DateTime? AddedDate { get; set; } = DateTime.Now;
        public string? AddedPc { get; set; } = "Default App User";
        public string? UpdatedBy { get; set; } = "appUser";
        public DateTime? UpdatedDate { get; set; } = DateTime.Now;
        public string? UpDatedPc { get; set; } = "Default App User";
    }
}
