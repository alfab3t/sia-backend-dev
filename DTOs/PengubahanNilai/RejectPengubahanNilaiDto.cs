using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.PengubahanNilai
{
    public class RejectPengubahanNilaiDto
    {
        [Required(ErrorMessage = "Alasan penolakan harus diisi")]
        public string AlasanTolak { get; set; } = string.Empty;
    }
}