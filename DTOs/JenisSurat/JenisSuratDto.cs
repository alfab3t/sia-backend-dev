namespace astratech_apps_backend.DTOs.JenisSurat
{
    public class JenisSuratDto
    {
        public int Id { get; set; }
        public string NamaSurat { get; set; } = string.Empty;
        public string FormatNoSurat { get; set; } = string.Empty;
        public string FormatSurat { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string AllowMahasiswaDisplay { get; set; } = string.Empty;
        public string Panel { get; set; } = string.Empty;
    }
}