using System.ComponentModel.DataAnnotations;
namespace astratech_apps_backend.DTOs.SkalaPenilaian
{
    public class SetStatusSkalaRequest
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Status harus diisi.")]
        public string Status { get; set; } = string.Empty;
    }
}