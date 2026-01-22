using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.PeriodeDaftarUlang
{
    public class UpdatePeriodeDaftarUlangRequest
    {
        [Required(ErrorMessage = "ID periode harus diisi.")]
        [Range(1, int.MaxValue, ErrorMessage = "ID periode tidak valid.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tanggal mulai harus diisi.")]
        [Range(typeof(DateTime), "1900-01-01", "2999-12-31", ErrorMessage = "Format Tanggal Mulai tidak valid.")]
        public DateTime? TanggalMulai { get; set; }

        [Required(ErrorMessage = "Tanggal akhir harus diisi.")]
        [Range(typeof(DateTime), "1900-01-01", "2999-12-31", ErrorMessage = "Format Tanggal Akhir tidak valid.")]
        public DateTime? TanggalAkhir { get; set; }
    }
}