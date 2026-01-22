using System.ComponentModel.DataAnnotations;
namespace astratech_apps_backend.DTOs.JamPlus
{
    public class CreateJamplus
    {
        [Required(ErrorMessage = "MahasiswaId wajib diisi")]
        public string MahasiswaId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Jumlah jam wajib diisi")]
        public decimal Jumlah { get; set; }

        [Required(ErrorMessage = "Jenis jam plus wajib dipilih")]
        public string Jenis { get; set; } = string.Empty;

        public string? Deskripsi { get; set; }

        [Required(ErrorMessage = "Tanggal wajib diisi")]
        public DateTime Tanggal { get; set; }

        [Required(ErrorMessage = "Tahun Akademik wajib diisi")]
        public string TahunAkademik { get; set; } = string.Empty;

        [Required(ErrorMessage = "Semester wajib diisi")]
        public string Semester { get; set; } = string.Empty;
    }
}