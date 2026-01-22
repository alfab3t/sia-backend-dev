using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.Kamar_Dormitory
{
    public class PindahPenghuniRequest
    {
        [Required]
        public int IdPenghuni { get; set; }

        [Required]
        public int IdKamarBaru { get; set; }
    }
}
