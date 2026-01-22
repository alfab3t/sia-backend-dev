using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.WaliMahasiswa
{
    public class RejectRequest 
    {
        [Required(ErrorMessage = "Alasan penolakan wajib diisi.")]
        public string Reason { get; set; } = string.Empty;
    }
}