namespace astratech_apps_backend.Models
{
    public class BuktiPelaksanaanPerkuliahanDetail
    {
        public string RpsId { get; set; } = string.Empty;
        public string NamaProdi { get; set; } = string.Empty; 
        public string TahunAjaran { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public string MataKuliah { get; set; } = string.Empty;
        public string Dosen1 { get; set; } = string.Empty;
        public string Dosen2 { get; set; } = string.Empty;
        public string Dosen3 { get; set; } = string.Empty;
        public int JumlahPertemuan { get; set; }
        public string JadwalSemester { get; set; } = string.Empty;
    }
}