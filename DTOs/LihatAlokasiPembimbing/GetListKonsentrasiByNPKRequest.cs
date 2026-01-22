using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.LihatAlokasiPembimbing
{
    public class GetListKonsentrasiByNPKRequest
    {
        [Required(ErrorMessage = "Nama pengguna harus diisi.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role harus diisi.")]
        public string Role { get; set; } = string.Empty;
    }
}
