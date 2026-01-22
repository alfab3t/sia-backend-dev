using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.JamMinus
{
    public class ImportJamMinusRequest
    {
        [Required(ErrorMessage = "File Excel wajib diupload")]
        public IFormFile File { get; set; }

        [Required(ErrorMessage = "Tahun Akademik wajib diisi")]
        public string TahunAkademik { get; set; } = string.Empty;

        [Required(ErrorMessage = "Semester wajib diisi")]
        public string Semester { get; set; } = string.Empty;
    }
}   