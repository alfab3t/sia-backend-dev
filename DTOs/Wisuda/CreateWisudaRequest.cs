using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.Wisuda
{
    public class CreateTandaTerimaIjazahRequest
    {
        [Required]
        [StringLength(10)]
        public string MahasiswaId { get; set; } = string.Empty!;

        [Required]
        public string Foto { get; set; } = string.Empty!; 
    }
}
