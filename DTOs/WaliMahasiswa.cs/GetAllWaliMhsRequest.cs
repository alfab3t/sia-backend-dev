using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.WaliMahasiswa
{
    public class GetAllWaliMhsRequest
    {
        [Required(ErrorMessage = "Nomor halaman harus diisi.")]
        public int PageNumber { get; set; } = 1;

        [Required(ErrorMessage = "Ukuran data per halaman harus diisi.")]
        public int PageSize { get; set; } = 10;

        public string SearchKeyword { get; set; } = string.Empty;
        
        public string Status { get; set; } = string.Empty;

        [Required(ErrorMessage = "Jenis urut harus diisi.")]
        public string Urut { get; set; } = string.Empty;

        public string Angkatan { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string ProdiId { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;

    }
}
