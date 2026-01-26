using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.CutiAkademik
{
    
    public class ApproveCutiAkademikByProdiRequest
    {
        
        [Required(ErrorMessage = "ID cuti akademik harus diisi")]
        public string Id { get; set; } = "";
        
        // Menimbang sekarang optional (tidak wajib)
        public string Menimbang { get; set; } = "";
        
       
        [Required(ErrorMessage = "ApprovedBy harus diisi")]
        public string ApprovedBy { get; set; } = "";
    }
}