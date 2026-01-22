namespace astratech_apps_backend.Models
{
    public class AlokasiKurikulumMahasiswa
    {
        public long RowNum { get; set; } = 0;
        public string Id { get; set; } = string.Empty;
        public string Nama { get; set; } = string.Empty;
        public string ProgramStudi { get; set; } = string.Empty;
        public string Angkatan { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public short IdProgramStudi { get; set; } = 0;
        public string Kurikulum { get; set; } = string.Empty;
        public string IdKurikulum { get; set; } = string.Empty;
    }
}
