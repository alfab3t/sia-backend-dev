namespace astratech_apps_backend.DTOs.TagihanCalonMahasiswa
{
    public class ExportTagihanCalonMahasiswaResponse
    {
        public long RowNumber { get; set; }
        public string NIM { get; set; } = string.Empty;
        public string Nama { get; set; } = string.Empty;
        public string JalurMasuk { get; set; } = string.Empty;
        public string Prodi { get; set; } = string.Empty;
        public string Angkatan { get; set; } = string.Empty;
        public decimal TotalTagihan { get; set; } 
        public decimal TotalPembayaran { get; set; }
        public decimal SisaTagihan { get; set; }
    }
}
