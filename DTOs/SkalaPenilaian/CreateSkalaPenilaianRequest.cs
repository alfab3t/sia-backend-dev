using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.SkalaPenilaian
{
    public class CreateSkalaPenilaianRequest
    {
        [Required(ErrorMessage = "Skala harus diisi.")]
        [Range(1, 100, ErrorMessage = "Skala harus antara 1 dan 100.")]
        public int Skala { get; set; }

        [Required(ErrorMessage = "Definisi harus diisi.")]
        [StringLength(500, ErrorMessage = "Definisi tidak boleh lebih dari 500 karakter.")]
        public string Definisi { get; set; } = string.Empty;
    }
}