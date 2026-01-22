using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.TemplateKuesioner
{
    public class SetStatusTemplateRequest
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Status harus diisi.")]
        public string Status { get; set; } = String.Empty;
    }

}
