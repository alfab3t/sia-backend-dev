using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.RekapitulsiIndustri
{
    public class GetAllRekapitulasiIndustriPivotRequest
    {
        [Required(ErrorMessage = "Nomor halaman harus diisi.")]
        public int PageNumber { get; set; } = 1;

        [Required(ErrorMessage = "Ukuran data per halaman harus diisi.")]
        public int PageSize { get; set; } = 10;
        public string? Keyword { get; set; }       
        public string TahunAjaran { get; set; } = string.Empty;
        public string? OrderBy { get; set; }
    }
}
