namespace astratech_apps_backend.Models
{
    public class JenisBeasiswa
    {
        public int Id { get; set; }
        public long RowNumber { get; set; } = 0;
        public int institusiId { get; set; }
        public string NamaInstitusi { get; set; } = string.Empty;
        public string NamaJenisBeasiswa { get; set; } = string.Empty;
        public int masaSemester { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
