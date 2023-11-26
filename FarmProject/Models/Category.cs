namespace FarmProject.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryDescription { get; set; }
        public bool CategoryStatus { get; set; } = true;
        public int? CompanyId { get; set; }
        public string AddedBy { get; set; } = "appUser";
        public DateTime DateAdded { get; set; } = DateTime.Now;
        public string AddedPc { get; set; } = "appUser";
        public string UpdateBy { get; set; } = "appUser";
        public DateTime DateUpdatedBy { get; set; } = DateTime.Now;
        public string UpdatePc { get; set; } = "appUser";
    }
}
