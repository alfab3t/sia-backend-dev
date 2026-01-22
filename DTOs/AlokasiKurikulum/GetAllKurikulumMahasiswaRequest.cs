using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.AlokasiKurikulum
{
    public class GetAllKurikulumMahasiswaRequest
    {
        [Required(ErrorMessage = "Nomor halaman harus diisi.")]
        public int PageNumber { get; set; } = 1;

        [Required(ErrorMessage = "Ukuran data per halaman harus diisi.")]
        public int PageSize { get; set; } = 10;

        public string SearchKeyword { get; set; } = string.Empty;
        public string Urut { get; set; } = "mhs_id asc"; 
        public string ProgramStudiId { get; set; } = string.Empty;
        public string Angkatan { get; set; } = string.Empty;
    }
}
