namespace astratech_apps_backend.Models
{
    public class SkalaPenilaian
    {
        public int Id { get; set; }
        public long RowNumber { get; set; }
        public int Skala { get; set; }
        public string Definisi { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}