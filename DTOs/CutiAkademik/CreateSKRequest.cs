using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.CutiAkademik
{
    
    public class CreateSKRequest
    {
        
        [Required(ErrorMessage = "ID cuti akademik harus diisi")]
        public string Id { get; set; } = "";
        
       
        public string? NoSK { get; set; }
        
       
        [Required(ErrorMessage = "CreatedBy harus diisi")]
        public string CreatedBy { get; set; } = "";
    }
}
