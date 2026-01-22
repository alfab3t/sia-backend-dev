namespace astratech_apps_backend.Models
{
    public class JenisKuesioner
    {
        public int Id { get; set; }
        public long RowNumber { get; set; }
        public string NamaJenis { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}