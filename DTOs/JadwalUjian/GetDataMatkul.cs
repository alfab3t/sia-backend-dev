namespace astratech_apps_backend.DTOs.JadwalUjian
{
    public class GetDataMatkul
    {
        public string TahunAkademik { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public string MataKuliah { get; set; } = string.Empty;
        public string JenisUjian { get; set; } = string.Empty;
        public string GrupId { get; set; } = string.Empty;
        public int IdMataKuliah { get; set; }
    }
}