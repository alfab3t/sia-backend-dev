namespace astratech_apps_backend.Models
{
    public class PenerimaBeasiswa
    {
        public long RowNumber { get; set; }
        public int beasiswaId { get; set; }
        public string Nim { get; set; } = string.Empty;
        public string NamaMahasiswa { get; set; } = string.Empty;
        public string NamaProdi { get; set; } = string.Empty;
        public string Tingkat { get; set; } = string.Empty;
    }
}
