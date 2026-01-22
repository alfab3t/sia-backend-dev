using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.LihatAlokasiPembimbing
{
    public class GetListKonsentrasiByDosenRequest
    {
        [Required(ErrorMessage = "Nama pengguna harus diisi.")]
        public string Username { get; set; } = string.Empty;
    }
}
