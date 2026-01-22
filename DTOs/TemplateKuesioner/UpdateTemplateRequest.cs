using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.TemplateKuesioner
{
    public class UpdateTemplateRequest
    {
        [Required(ErrorMessage = "ID Template harus diisi.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nama template harus diisi.")]
        [StringLength(50)]
        public string NamaTemplate { get; set; } = string.Empty;

        [Required(ErrorMessage = "ID Jenis Kuesioner harus diisi.")]
        public int IdJenisKuesioner { get; set; }

        [Required(ErrorMessage = "ID Skala Penilaian harus diisi.")]
        public int IdSkalaPenilaian { get; set; }
    }
}