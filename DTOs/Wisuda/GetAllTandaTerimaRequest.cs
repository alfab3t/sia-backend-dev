using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.Wisuda
{
    public class GetAllTandaTerimaRequest
    {
        [Required(ErrorMessage = "Nomor halaman harus diisi.")]
        public int PageNumber { get; set; } = 1;

        [Required(ErrorMessage = "Ukuran data per halaman harus diisi.")]
        public int PageSize { get; set; } = 10;

        public string SearchKeyword { get; set; } = string.Empty;
        public string KonsentrasiId { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Tahun Wajib Diisi.")]
        public string TahunLulus { get; set; } = string.Empty;

        public string StatusTerima { get; set; } = string.Empty;

        [Required(ErrorMessage = "Jenis urut harus diisi.")]
        public string Urut { get; set; } = string.Empty;
    }
}
