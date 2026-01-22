using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.Kamar_Dormitory
{
    public class GetAllKamarDormitoryRequest
    {
        public string? SearchKeyword { get; set; }
        [Required]
        public string? Status { get; set; }
        [Required]
        public string? Urut { get; set; }
        public string? JenisKamar { get; set; }
        [Required]
        public int PageNumber { get; set; } = 1;
        [Required]
        public int PageSize { get; set; } = 10;
    }

}
