using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.Kuesioner
{
    public class GetAllKuesionerRequest
    {
        [Required]
        public int PageNumber { get; set; } = 1;
        [Required]
        public int PageSize { get; set; } = 10;

        public string? SearchKeyword { get; set; } = "";
        public string? TahunAkademik { get; set; } = "";
        public string? Semester { get; set; } = "";
        public string? Prodi { get; set; } = "";

        [Required]
        public string Urut { get; set; } = "";
    }
}