using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.TemplateKuesioner
{
    public class ImportPertanyaanRequest
    {
        [Required]
        public int IdTemplate { get; set; }

        [Required]
        public IFormFile File { get; set; } = null!;
    }
}