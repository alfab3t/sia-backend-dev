namespace astratech_apps_backend.DTOs.Kuesioner
{
    public class KuesionerDto
    {
        public int Id { get; set; }
        public long RowNumber { get; set; }
        public string NamaKuesioner { get; set; } = string.Empty;
        public string TahunAkademik { get; set; } = string.Empty;
        public int Semester { get; set; }
        public string Prodi { get; set; } = string.Empty;
        public string Kelas { get; set; } = string.Empty;
        public string StatusPengisian { get; set; } = string.Empty;
    }
}