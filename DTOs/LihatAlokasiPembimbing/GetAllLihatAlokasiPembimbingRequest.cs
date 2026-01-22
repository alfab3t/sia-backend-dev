using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.LihatAlokasiPembimbing
{
    public class GetAllLihatAlokasiPembimbingRequest
    {
        [Required(ErrorMessage = "Nomor halaman harus diisi.")]
        public int PageNumber { get; set; } = 1;

        [Required(ErrorMessage = "Ukuran data per halaman harus diisi.")]
        public int PageSize { get; set; } = 10;

        public string? Username { get; set; }
        public string? SearchKeyword { get; set; }

        [Required(ErrorMessage = "Jenis urut harus diisi.")]
        public string Urut { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tahun akademik harus diisi.")]
        public string TahunAkademik { get; set; } = string.Empty;

        public string? Konsentrasi { get; set; } 
        public string? Status { get; set; }
        public string? Role { get; set; }
        public string? SekretarisProdi { get; set; }
    }
}
