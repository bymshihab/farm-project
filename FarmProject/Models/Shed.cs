using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace FarmProject.Models
{
    public class Shed
    {
        public int ShedId { get; set; }
        public string ShedName { get; set; }
        public string ShedDescription { get; set; }
        public bool Status { get; set; }
        public int ShedTypeId { get; set; }
        public int? CompanyId { get; set; }
        public string? AddedBy { get; set; } = "appUser";
        public DateTime? AddedDate { get; set; } = DateTime.Now;
        public string? AddedPc { get; set; } = "Default App User";
        public string? UpdatedBy { get; set; } = "appUser";
        public DateTime? UpdatedDate { get; set; } = DateTime.Now;
        public string? UpDatedPc { get; set; } = "Default App User";
    }
}
