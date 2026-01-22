using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.Pengumuman
{
    public class GetDetailPengumumanResponse
    {
        [Required(ErrorMessage = "Data Id User Pengumuman tidak ditemukan.")]
        public int IdPengumuman { get; set; }

        [Required(ErrorMessage = "Data Id App tidak ditemukan.")]
        [StringLength(5, ErrorMessage = "Id App melebihi panjang maksimal 5 karakter.")]
        public string IdAplikasi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Data Id User Pengumuman tidak ditemukan.")]
        public string NamaAplikasi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Data Subyek Pengumuman tidak tersedia.")]
        public string SubyekPengumuman { get; set; } = string.Empty;

        [Required(ErrorMessage = "Data tujuan pengumuman (KpdPengumuman) tidak tersedia.")]
        public string KpdPengumuman { get; set; } = string.Empty;

        [Required(ErrorMessage = "Data Tanggal Mulai Pengumuman tidak tersedia.")]
        public string TanggalMulaiPengumuman { get; set; } = string.Empty;

        [Required(ErrorMessage = "Data Tanggal Selesai Pengumuman tidak tersedia.")]
        public string TanggalSelesaiPengumuman { get; set; } = string.Empty;

        [Required(ErrorMessage = "Data Tanggal Mulai Pengumuman tidak tersedia.")]
        public string TanggalMulaiPengumumanIndo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Data Tanggal Selesai Pengumuman tidak tersedia.")]
        public string TanggalSelesaiPengumumanIndo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Data Status Baca tidak ditemukan.")]
        public byte StatusBaca { get; set; }

        [Required(ErrorMessage = "Data Isi Pengumuman tidak tersedia.")]
        public string IsiPengumuman { get; set; } = string.Empty;
    }
}
