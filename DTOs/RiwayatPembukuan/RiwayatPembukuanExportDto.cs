using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.RiwayatPembukuan
{
    public class RiwayatPembukuanExportDto
    {
        public int No { get; set; }
        public string VirtualAccount { get; set; } = string.Empty;
        public string NIMOrNoDaftar { get; set; } = string.Empty;
        public string Prodi { get; set; } = string.Empty;
        public string Nama { get; set; } = string.Empty;
        public string TahunAjaran { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tanggal pembukuan harus diisi.")]
        [Range(typeof(DateTime), "1900-01-01", "2999-12-31", ErrorMessage = "Format tanggal pembukuan tidak valid.")]
        public DateTime WaktuPembukuan { get; set; }
        public decimal Jumlah { get; set; }
        public string Keterangan { get; set; } = string.Empty;
    }
}