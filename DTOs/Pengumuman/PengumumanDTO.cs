namespace astratech_apps_backend.DTOs.Pengumuman
{
    public class PengumumanDto
    {
        public int IdPengumuman { get; set; }
        public string NamaAplikasi { get; set; } = string.Empty;
        public string SubyekPengumuman { get; set; } = string.Empty;
        public string StatusPengumuman { get; set; } = string.Empty;
        public DateTime TanggalPengumuman { get; set; }
        public string TanggalPengumumanF { get; set; } = string.Empty;
    }
}
