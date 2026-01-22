namespace astratech_apps_backend.Models
{
    public class JadwalUjian
    {
        public int IdJadwalUjian { get; set; }
        public int IdMataKuliah { get; set; }         
        public string Prodi { get; set; } = string.Empty;
        public string TahunAkademik { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public string Kelas { get; set; } = string.Empty;
        public string JenisUjian { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string MataKuliah { get; set; } = string.Empty;          
        public string Pengawas1 { get; set; } = string.Empty;          
        public string Pengawas2 { get; set; } = string.Empty;          
        public string Ruangan { get; set; } = string.Empty;            
        public string TanggalUjian { get; set; } = string.Empty;    
        public string WaktuUjian { get; set; } = string.Empty;         
    }
}
