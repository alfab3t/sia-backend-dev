namespace astratech_apps_backend.Models
{
    public class TagihanCalonMahasiswa
    {
        public long Id { get; set; }
        public long RowNumber { get; set; }
        public string NIM { get; set; } = string.Empty;
        public string Nama { get; set; } = string.Empty;
        public string Prodi { get; set; } = string.Empty;
        public string Angkatan { get; set; } = string.Empty;
        public decimal Jumlah { get; set; }
        public string TotalTagihan { get; set; } = string.Empty;
        public decimal TotalPembayaran { get; set; }
        public decimal SisaTagihan { get; set; }
        public string Keterangan { get; set; } = string.Empty;
    }
}

