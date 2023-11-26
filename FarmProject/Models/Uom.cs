namespace FarmProject.Models
{
    public class Uom
    {
        public int UomId { get; set; }
        public string UomName { get; set; }
        public string? UomDescription { get; set; }
        public Boolean Status { get; set; } = true;
        public int? CompanyId { get; set; }
        public string AddedBy { get; set; } = "appUser";
        public DateTime AddedDate { get; set; } = DateTime.Now;
        public string AddedPc { get; set; } = "Default App User";
        public string UpdatedBy { get; set; } = "appUser";
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
        public string UpDatedPc { get; set; } = "Default App User";
    }
}
