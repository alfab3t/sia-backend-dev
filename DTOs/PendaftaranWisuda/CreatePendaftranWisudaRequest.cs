using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.PendaftaranWisuda
{
    public class CreatePendaftaranWisudaRequest
    {
        [Required]
        public string TahunAkademik { get; set; } = string.Empty;

        [Required]
        public DateTime TanggalMulaiPendaftaran { get; set; }

        [Required]
        public DateTime TanggalAkhirPendaftaran { get; set; }

        [Required]
        public int KuotaUndanganTambahan { get; set; }
    }
}
