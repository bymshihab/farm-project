using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace FarmProject.Models
{
    public class Animal
    {
        public int? AnimalId { get; set; }
        public string AnimalName { get; set; }
        public string? AnimalTagNo { get; set; }
        public int ProductId { get; set; }
        public int ShedId { get; set; }
        public Boolean IsDead { get; set; } = false;
        public Boolean IsSold { get; set; } = false;
        public Boolean? IsVaccinated { get; set; } = false;
        public string? QRCodeData { get; set; }
        public int? MilkId { get; set; }
        public double weight { get; set; }
        public DateTime? DOB { get; set; }
        public int GenderId { get; set; }
        public Boolean Status { get; set; } = true;
        public int? CompanyId { get; set; }
        public string? AnimalImage { get; set; }
        public IFormFile? ImageFile { get; set; }
        public string? AddedBy { get; set; } = "appUser";
        public DateTime? AddedDate { get; set; } = DateTime.Now;
        public string? AddedPc { get; set; } = "Default App User";
        public string? UpdatedBy { get; set; } = "appUser";
        public DateTime? UpdatedDate { get; set; } = DateTime.Now;
        public string? UpDatedPc { get; set; } = "Default App User";
    }
}
