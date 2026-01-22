using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.CutiAkademik
{
   
    public class RejectCutiAkademikRequest
    {
        
        [Required(ErrorMessage = "ID cuti akademik harus diisi")]
        public string Id { get; set; } = "";
        
        
        [Required(ErrorMessage = "Role harus diisi")]
        public string Role { get; set; } = "";
        
       
        [Required(ErrorMessage = "Username harus diisi")]
        public string Username { get; set; } = "";
    }
}