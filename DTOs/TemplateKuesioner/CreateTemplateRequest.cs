using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.TemplateKuesioner
{
    public class CreateTemplateRequest
    {
        [Required(ErrorMessage = "Nama template harus diisi.")]
        [StringLength(50)]
        public string NamaTemplate { get; set; } = string.Empty;

        [Required(ErrorMessage = "ID Jenis Kuesioner harus diisi.")]
        public int IdJenisKuesioner { get; set; }

        [Required(ErrorMessage = "ID Skala Penilaian harus diisi.")]
        public int IdSkalaPenilaian { get; set; }
    }
}