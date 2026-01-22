namespace astratech_apps_backend.DTOs.MasterIndustri
{
    public class MasterIndustriDto
    {
        public int Id { get; set; }
        public long RowNumber { get; set; }
        public string NamaIndustri { get; set; } = string.Empty;
        public string Cabang { get; set; } = string.Empty;
        public string Grup { get; set; } = string.Empty;
        public string Alamat { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
