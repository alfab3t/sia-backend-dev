using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.CutiAkademik
{
    
    public class CreateSKCutiAkademikRequest
    {
        
        [Required(ErrorMessage = "ID cuti akademik harus diisi")]
        public string Id { get; set; } = "";
        
       
        [Required(ErrorMessage = "Nomor SK atau filename harus diisi")]
        public string SkNumber { get; set; } = "";
        
       
        [Required(ErrorMessage = "ModifiedBy harus diisi")]
        public string ModifiedBy { get; set; } = "";
    }
}