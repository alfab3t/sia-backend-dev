using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.LihatAlokasiPembimbing
{
    public class ExportLihatAlokasiPembimbingRequest
    {
        [Required(ErrorMessage = "Tahun akademik harus diisi.")]
        public string TahunAkademik { get; set; } = string.Empty;

        [Required(ErrorMessage = "Konsentrasi harus diisi.")]
        public string KonsentrasiId { get; set; } = string.Empty;
    }
}
