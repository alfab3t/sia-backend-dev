namespace astratech_apps_backend.DTOs.Kamar_Dormitory
{
    public class PenghuniKamarDto
    {
        public long RowNumber { get; set; }
        public int Id { get; set; }
        public string Nama { get; set; } = "";
        public string Prodi { get; set; } = "";
        public string HP { get; set; } = "";
        public string TanggalMasuk { get; set; } = "";
        public string TerakhirDitagih { get; set; } = "";
        public string? TanggalKeluar { get; set; }
    }
}