namespace astratech_apps_backend.DTOs.JadwalUjian
{
    public class JadwalUjianDetailDto
    {
        public int IdDetailJadwalUjian { get; set; }
        public string Prodi { get; set; } = string.Empty;
        public string TahunAkademik { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public string Kelas { get; set; } = string.Empty;
        public string Grup { get; set; } = string.Empty;
        public string JenisUjian { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}