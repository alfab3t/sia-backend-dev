using System.ComponentModel.DataAnnotations;
namespace astratech_apps_backend.DTOs.Mahasiswa
{
    public class GetAllMahasiswaRequest
    {
        public string? SearchKeyword { get; set; } = "";
        public string? Status { get; set; } = "";

        [Required(ErrorMessage = "Jenis urut harus diisi.")]
        public string? Urut { get; set; } = "mhs_id";
        public string? ProdiId { get; set; } = "";
        public string? Angkatan { get; set; } = "";
        public string? RoleId { get; set; } = "";
        public string? JenisMahasiswaId { get; set; } = "";
        public string? Username { get; set; } = "";

        [Required(ErrorMessage = "Nomor halaman harus diisi.")]
        public int? PageNumber { get; set; } = 1;

        [Required(ErrorMessage = "Ukuran data per halaman harus diisi.")]
        public int? PageSize { get; set; } = 10;
    }
}