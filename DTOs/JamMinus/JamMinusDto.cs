namespace astratech_apps_backend.DTOs.Jam_Minus
{
    public class JamMinusDto
    {
        public string MahasiswaId { get; set; } = string.Empty;

        public string Nama { get; set; } = string.Empty;

        public string Prodi { get; set; } = string.Empty;

        public string TahunAkademik { get; set; } = string.Empty;

        public string Semester { get; set; } = string.Empty;

        public string Kelas { get; set; } = string.Empty;

        public decimal TotalJamMinus { get; set; }

        public int IdKonsentrasi { get; set; }

        public string KelId { get; set; } = string.Empty;
    }
}
