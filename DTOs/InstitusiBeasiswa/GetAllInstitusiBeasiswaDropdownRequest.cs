using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.InstitusiBeasiswa
{
    public class GetAllInstitusiBeasiswaDropdownRequest
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SearchKeyword { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;

        [Required(ErrorMessage = "Jenis urut harus diisi.")]
        public string Urut { get; set; } = string.Empty;
    }
}
