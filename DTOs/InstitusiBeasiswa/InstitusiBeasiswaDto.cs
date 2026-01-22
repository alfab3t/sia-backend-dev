namespace astratech_apps_backend.DTOs.InstitusiBeasiswa
{
    public class InstitusiBeasiswaDto
    {
        public int Id { get; set; }
        public string NamaInstitusiBeasiswa { get; set; } = string.Empty;
        public string Alamat { get; set; } = string.Empty;
        public string Telepon { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
