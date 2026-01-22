namespace astratech_apps_backend.Models
{
    public class BuktiPelaksanaanPerkuliahan
    {
        public string RpsId { get; set; } = string.Empty;
        public string KodeNamaDosen { get; set; } = string.Empty;
        public string ProgramStudi { get; set; } = string.Empty;
        public string TahunAkademik { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public string MataKuliah { get; set; } = string.Empty;
        public string Kelas { get; set; } = string.Empty;

        public string UsernameDosen { get; set; } = string.Empty; 
        public int JumlahPertemuan { get; set; }
        public int AktualPertemuan { get; set; }
        public string PersentasePertemuan { get; set; } = string.Empty;
    }
}