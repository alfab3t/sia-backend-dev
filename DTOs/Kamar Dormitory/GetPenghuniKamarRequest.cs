using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.Kamar_Dormitory
{
    public class GetPenghuniKamarRequest
    {
        [Required]
        public int IdKamar { get; set; }
        public string? Status { get; set; } 
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}