using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.PeriodeDaftarUlang
{
    public class CreatePeriodeDaftarUlangRequest
    {
        [Required(ErrorMessage = "Tahun ajaran harus dipilih.")]
        [StringLength(9)]
        public string TahunAjaran { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tanggal mulai harus diisi.")]
        [Range(typeof(DateTime), "1900-01-01", "2999-12-31", ErrorMessage = "Format Tanggal Mulai tidak valid.")]
        public DateTime? TanggalMulai { get; set; }

        [Required(ErrorMessage = "Tanggal akhir harus diisi.")]
        [Range(typeof(DateTime), "1900-01-01", "2999-12-31", ErrorMessage = "Format Tanggal Akhir tidak valid.")]
        public DateTime? TanggalAkhir { get; set; }
    }
}