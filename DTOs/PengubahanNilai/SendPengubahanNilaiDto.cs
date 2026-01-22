using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.PengubahanNilai
{
    public class SendPengubahanNilaiDto
    {
        [Required(ErrorMessage = "Tipe pengiriman harus diisi")]
        public string Tipe { get; set; } = string.Empty;
    }
}