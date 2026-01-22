namespace astratech_apps_backend.Models
{
    public class Jurusan
    {
        public short Id { get; set; }
        public long RowNumber { get; set; } = 0;
        public string NamaJurusan { get; set; } = string.Empty;
        public string NamaKepalaJurusan { get; set; } = string.Empty;
        public string NoNPK { get; set; } = string.Empty;
        public string Deskripsi { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int JumlahProdi { get; set; }
        public List<ListJurusanProdi> ListProdi { get; set; } = new();
    }
}
