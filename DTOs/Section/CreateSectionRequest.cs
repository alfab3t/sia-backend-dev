using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.Section
{
    public class CreateSectionRequest
    {
        [Required(ErrorMessage = "Nama section harus diisi.")]
        [StringLength(100)]
        public string NamaSection { get; set; } = string.Empty;
    }
}
