using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.JenisBeasiswa
{
    public class UpdateJenisBeasiswaRequest
    {
        [Required(ErrorMessage = "ID institusi harus diisi.")]
        [Range(1, short.MaxValue, ErrorMessage = "ID institusi tidak valid.")]
        public required short Id { get; set; }

        [Required(ErrorMessage = "ID institusi harus diisi.")]
        [Range(1, short.MaxValue, ErrorMessage = "ID institusi tidak valid.")]
        public required short InstitusiId { get; set; }

        [Required(ErrorMessage = "Nama jenis Beasiswa harus diisi.")]
        [StringLength(100)]
        public string NamaJenisBeasiswa { get; set; } = string.Empty;

        [Required(ErrorMessage = "Masa semester harus diisi.")]
        [Range(1, 8, ErrorMessage = "Masa semester harus antara 1 hingga 8.")]
        public int? masaSemester { get; set; }


    }
}
