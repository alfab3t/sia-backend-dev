namespace astratech_apps_backend.DTOs.JenisBeasiswa
{
    public class JenisBeasiswaDto
    {
        public int Id { get; set; }
        public string NamaInstitusi { get; set; } = string.Empty;
        public string NamaJenisBeasiswa { get; set; } = string.Empty;
        public int masaSemester { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
