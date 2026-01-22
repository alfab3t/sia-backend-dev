using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.TemplateKuesioner
{
    public class CreateUpdatePertanyaanRequest
    {
        [Required(ErrorMessage = "ID Template harus diisi.")]
        public int Id{ get; set; }

        [Required(ErrorMessage = "Teks pertanyaan harus diisi.")]
        [StringLength(200)]
        public string PertanyaanText { get; set; } = string.Empty;

        [Required(ErrorMessage = "Status header harus diisi.")]
        public bool IsHeader { get; set; }

        public string Jenis { get; set; } = string.Empty;
    }
}