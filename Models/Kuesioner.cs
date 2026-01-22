namespace astratech_apps_backend.Models
{
    public class Kuesioner
    {
        public int Id { get; set; }
        public long RowNumber { get; set; }
        public string NamaKuesioner { get; set; } = string.Empty;
        public string TahunAkademik { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public string Prodi { get; set; } = string.Empty;
        public string Kelas { get; set; } = string.Empty;
        public string StatusPengisian { get; set; } = string.Empty;
    }
}