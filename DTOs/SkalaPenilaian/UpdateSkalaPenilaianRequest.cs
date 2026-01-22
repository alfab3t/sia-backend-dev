using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.SkalaPenilaian
{
    public class UpdateSkalaPenilaianRequest
    {
        [Required(ErrorMessage = "ID harus diisi.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Definisi harus diisi.")]
        [StringLength(500, ErrorMessage = "Definisi tidak boleh lebih dari 500 karakter.")]
        public string Definisi { get; set; } = string.Empty;
    }
}