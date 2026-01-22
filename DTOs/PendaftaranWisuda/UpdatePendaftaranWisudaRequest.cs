using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.PendaftaranWisuda
{
    public class UpdatePendaftaranWisudaRequest
    {
        [Required]
        public int Id { get; set; }   
        [Required]
        public DateTime TanggalMulai { get; set; }
        [Required]
        public DateTime TanggalAkhir { get; set; } 
        [Required]
        public int KuotaTambahan { get; set; } 
    }
}
