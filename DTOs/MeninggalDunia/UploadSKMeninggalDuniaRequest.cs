using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.MeninggalDunia
{
    public class UploadSKMeninggalDuniaRequest
    {
        [Required(ErrorMessage = "ID meninggal dunia harus diisi")]
        public string MduId { get; set; } = "";
        
        [Required(ErrorMessage = "File SK harus diupload")]
        public IFormFile SK { get; set; } = null!;        
               
        public IFormFile? SKPB { get; set; }     
    
        [Required(ErrorMessage = "ModifiedBy harus diisi")]
        public string ModifiedBy { get; set; } = "";
    }
}