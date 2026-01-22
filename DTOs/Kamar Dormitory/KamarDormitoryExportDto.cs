namespace astratech_apps_backend.DTOs.Kamar_Dormitory
{
    public class KamarDormitoryExportDto
    {
        public string NIM { get; set; }
        public string Nama { get; set; }
        public string Prodi { get; set; }
        public string Angkatan { get; set; }
        public string NoKamar { get; set; }
        public DateTime? TanggalMasuk { get; set; }
        public DateTime? TanggalKeluar { get; set; }
        public DateTime? TerakhirDitagih { get; set; }
        public string NIK { get; set; }
        public decimal Tagihan { get; set; }
        public decimal Pembayaran { get; set; }
        public decimal SisaTagihan { get; set; }
    }
}
