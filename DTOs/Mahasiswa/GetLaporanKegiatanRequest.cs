using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.Mahasiswa
{
    public class GetLaporanKegiatanRequest
    {
        [StringLength(200)]
        public string Search { get; set; } = string.Empty;

        [StringLength(50)]
        public string Status { get; set; } = string.Empty;

        [StringLength(100)]
        public string OrderBy { get; set; } = string.Empty;

        [StringLength(10)]
        public string Tahun { get; set; } = string.Empty;

        [StringLength(50)]
        public string KegiatanId { get; set; } = string.Empty;

        [StringLength(50)]
        public string IdMahasiswa { get; set; } = string.Empty;

        [StringLength(50)]
        public string Role { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int Halaman { get; set; } = 1;

        [Range(1, 1000)]
        public int Limit { get; set; } = 10;
    }
}