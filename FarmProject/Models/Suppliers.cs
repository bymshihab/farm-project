using System.ComponentModel.DataAnnotations.Schema;

namespace FarmProject.Models
{
    public class Suppliers
    {
        public int SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string? PhoneNo { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public Boolean Status { get; set; } = false;
        public int? CompanyId { get; set; }
        public string? AddedBy { get; set; } = "appUser";
        public DateTime? AddedDate { get; set; } = DateTime.Now;
        public string? AddedPc { get; set; } = "Default App User";
        public string? UpdatedBy { get; set; } = "appUser";
        public DateTime? UpdatedDate { get; set; } = DateTime.Now;
        public string? UpDatedPc { get; set; } = "Default App User";
    }
}
