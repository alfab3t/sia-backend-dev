using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.Section
{
    public class UpdateSectionRequest
    {
        [Required(ErrorMessage = "ID section harus diisi.")]
        [Range(1, short.MaxValue, ErrorMessage = "ID section tidak valid.")]
        public required short Id { get; set; }

        [Required(ErrorMessage = "Nama section harus diisi.")]
        [StringLength(100)]
        public string NamaSection { get; set; } = string.Empty;
    }
}
