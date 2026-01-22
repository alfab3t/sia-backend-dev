namespace astratech_apps_backend.DTOs.JamMinusPlus
{
    public class JamMinusPlusDto
    {
        public string MahasiswaId { get; set; } = string.Empty;
        public string Nama { get; set; } = string.Empty;
        public string Prodi { get; set; } = string.Empty;
        public string Kelas { get; set; } = string.Empty;
        public string TahunAkademik { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public decimal SisaKompensasi { get; set; }
        public decimal SisaMurni { get; set; }
    }
}
