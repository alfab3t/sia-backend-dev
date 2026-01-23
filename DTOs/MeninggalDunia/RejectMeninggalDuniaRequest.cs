using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.MeninggalDunia
{
    public class RejectMeninggalDuniaRequest
    {
        [Required(ErrorMessage = "Role harus diisi")]
        public string Role { get; set; } = "";      
        
        [Required(ErrorMessage = "Username harus diisi")]
        public string Username { get; set; } = "";  
    }
}
