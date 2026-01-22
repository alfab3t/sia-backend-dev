using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.InstitusiBeasiswa
{
    public class CreateInstitusiBeasiswaRequest
    {
        [Required(ErrorMessage = "Nama institusi harus diisi.")]
        [StringLength(100)]
        public string NamaInstitusiBeasiswa { get; set; } = string.Empty;

        [Required(ErrorMessage = "Alamat harus diisi.")]
        [StringLength(200)]
        public string Alamat { get; set; } = string.Empty;

        [StringLength(13, ErrorMessage = "Telepon maksimal 13 karakter.")]
        public string Telepon { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Format email tidak valid.")]
        [StringLength(50)]
        public string Email { get; set; } = string.Empty;
    }
}
