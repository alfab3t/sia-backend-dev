using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.JadwalUjian
{
    public class DeleteJadwalUjianDto
    {
        [Required(ErrorMessage = "IdJadwal wajib diisi")]
        public string IdJadwal { get; set; } = string.Empty;
        
    }
}