using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.AlokasiKurikulum
{
    public class UpdateAlokasiKurikulumRequest
    {
        [Required(ErrorMessage = "Kurikulum harus dipilih.")]
        public string KurikulumId { get; set; } = string.Empty;

        public List<string> ListMahasiswaId { get; set; } = [];
    }
}
