using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.JenisSurat
{
    public class UpdateJenisSuratRequest
    {
        [Required(ErrorMessage = "Id jenis surat harus diisi")]
        [Range(1, int.MaxValue, ErrorMessage = "Id jenis surat tidak valid")]
        public required int Id { get; set; }

        [Required(ErrorMessage = "Nama jenis surat harus diisi")]
        [StringLength(100, ErrorMessage = "Nama jenis surat maksimal 100 karakter")]
        public string NamaSurat { get; set; } = string.Empty;

        [Required(ErrorMessage = "Format nomor jenis surat harus diisi")]
        [StringLength(100, ErrorMessage = "Format nomor jenis surat maksimal 100 karakter")]
        public string FormatNoSurat { get; set; } = string.Empty;

        [Required(ErrorMessage = "Allow mahasiswa harus diisi")]
        [Range(0, 1, ErrorMessage = "Nilai allow mahasiswa harus 0 atau 1")]
        public int AllowMahasiswa { get; set; }
    }
}