using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.CutiAkademik
{
    
    public class ApproveProdiCutiRequest
    {
        
        [Required(ErrorMessage = "ID cuti akademik harus diisi")]
        public string Id { get; set; } = "";
        
       
        [Required(ErrorMessage = "Menimbang/pertimbangan harus diisi")]
        [MinLength(10, ErrorMessage = "Menimbang minimal 10 karakter")]
        public string Menimbang { get; set; } = "";
        
       
        [Required(ErrorMessage = "ApprovedBy harus diisi")]
        public string ApprovedBy { get; set; } = "";
    }
}