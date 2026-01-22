namespace astratech_apps_backend.DTOs.Jurusan
{
    public class JurusanDto
    {
        public short Id { get; set; }
        public string NamaJurusan { get; set; } = string.Empty;
        public string NamaKepalaJurusan { get; set; } = string.Empty;
        public string NoNPK { get; set; } = string.Empty;
        public string Deskripsi { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;

        public int JumlahProdi { get; set; }
    }
}

    