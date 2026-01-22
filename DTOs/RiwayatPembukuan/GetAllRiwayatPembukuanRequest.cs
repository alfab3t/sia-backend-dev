using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.RiwayatPembukuan
{
    public class GetAllRiwayatPembukuanRequest
    {
        public string? Keyword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nomor halaman harus diisi.")]
        public int PageNumber { get; set; } = 1;

        [Required(ErrorMessage = "Ukuran data per halaman harus diisi.")]
        public int PageSize { get; set; } = 10;
        public string? TanggalMulai { get; set; } = string.Empty;
        public string? TanggalSampai { get; set; } = string.Empty;

        [Required(ErrorMessage = "Jenis urut harus diisi.")]
        public string? Urut { get; set; } = string.Empty;
        public string? ProgramStudi { get; set; } = string.Empty;
        public string? TahunAjaran { get; set; } = string.Empty;
    }
}