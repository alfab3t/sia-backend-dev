using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.CutiAkademik
{
    public class GenerateIdFinalCutiAkademikRequest
    {
        [Required(ErrorMessage = "Draft ID harus diisi")]
        public string DraftId { get; set; } = "";
        
        [Required(ErrorMessage = "ModifiedBy harus diisi")]
        public string ModifiedBy { get; set; } = "";
    }
}