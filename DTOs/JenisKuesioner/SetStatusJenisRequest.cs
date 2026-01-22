using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.JenisKuesioner
{
    public class SetStatusJenisRequest
    {
        [Required(ErrorMessage = "ID harus diisi.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Status harus diisi.")]
        public string Status { get; set; } = string.Empty;
    }
}