using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.CutiAkademik
{
   
    public class UploadSKCutiAkademikRequest
    {
       
        [Required(ErrorMessage = "ID cuti akademik harus diisi")]
        public string Id { get; set; } = "";
        
       
        [Required(ErrorMessage = "File SK harus diupload")]
        public IFormFile FileSK { get; set; } = null!;
        
        
        [Required(ErrorMessage = "UploadBy harus diisi")]
        public string UploadBy { get; set; } = "";
    }
}