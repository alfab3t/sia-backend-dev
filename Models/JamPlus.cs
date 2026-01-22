namespace astratech_apps_backend.Models
{
    public class JamPlus
    {
        public long RowNum { get; set; }
        public string MahasiswaId { get; set; } = string.Empty;
        public string Nim { get; set; } = string.Empty;
        public string Nama { get; set; } = string.Empty;
        public string Prodi { get; set; } = string.Empty;
        public string TahunAkademik { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public string Kelas { get; set; } = string.Empty;
        public decimal TotalJamPlus { get; set; }
        public int KonId { get; set; } = 0;
        public string KelId { get; set; } = string.Empty;
        public int IdJamPlus { get; set; } = 0;
        public string Deskripsi { get; set; } = string.Empty;
        public string Jenis { get; set; } = string.Empty;
        public string Tanggal { get; set; } = string.Empty;
    }
}