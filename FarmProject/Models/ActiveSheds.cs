namespace FarmProject.Models
{
    public class ActiveSheds
    {
        public int ShedId { get; set; }
        public string ShedName { get; set; }
        public string ShedDescription { get; set; }
        public string shedTypeName { get; set; }
        public bool Status { get; set; } = true;
    }
}
