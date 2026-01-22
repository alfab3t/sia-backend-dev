using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.JenisBeasiswa
{
    public class CreateJenisBeasiswaRequest
    {
        [Required(ErrorMessage = "Jenis Beasiswa harus di isi .")]
        public int InstitusiId { get; set; }

        [Required(ErrorMessage = "Nama Beasiswa harus diisi.")]
        [StringLength(100, ErrorMessage = "Nama Beasiswa maksimal 100 karakter.")]
        public string NamaJenisBeasiswa { get; set; } = string.Empty;

        [Required(ErrorMessage = "Masa semester harus diisi.")]
        [Range(1, 8, ErrorMessage = "Masa semester harus antara 1 hingga 8.")]
        public int? masaSemester { get; set; }

    }
}
