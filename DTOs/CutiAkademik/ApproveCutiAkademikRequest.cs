using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.CutiAkademik
{
    
    public class ApproveCutiAkademikRequest
    {
        [Required(ErrorMessage = "ID cuti akademik harus diisi")]
        public string Id { get; set; } = "";
        
        [Required(ErrorMessage = "Role harus diisi")]
        public string Role { get; set; } = "";
        
        [Required(ErrorMessage = "ApprovedBy harus diisi")]
        public string ApprovedBy { get; set; } = "";
    }
}