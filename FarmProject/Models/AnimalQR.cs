namespace FarmProject.Models
{
    public class AnimalQR
    {

            public int AnimalId { get; set; }
            public string AnimalName { get; set; }
            public string AnimalTagNo { get; set; }
            public string ProductName { get; set; }
            public string ShedName { get; set; }
            public bool IsDead { get; set; }
            public bool IsSold { get; set; }
            public bool IsVaccinated { get; set; }
            public int? MilkId { get; set; }
            public decimal? weight { get; set; }
            public DateTime? DOB { get; set; }
            public string GenderType { get; set; }
            public string Status { get; set; }
            public string QRCodeData { get; set; }
            public string AnimalImageBase64 { get; set; } 
            public string QRCodeImageBase64 { get; set; } 
    }
}
