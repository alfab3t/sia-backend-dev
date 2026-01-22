using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.SkalaPenilaian
{
    public class GetAllSkalaPenilaianRequest
    {
        [Required]
        public int PageNumber { get; set; } = 1;

        [Required]
        public int PageSize { get; set; } = 10;

        public string SearchKeyword { get; set; } = string.Empty;
        public string Status { get; set; } = "Aktif";

        [Required]
        public string Urut { get; set; } = string.Empty;
    }
}