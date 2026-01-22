using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.TagihanCalonMahasiswa
{
    public class GetAllTagihanCalonMahasiswaRequest
    {
        public string? Keyword { get; set; }
        public string? NIM { get; set; }           
        public string? Prodi { get; set; }         
        public string? Angkatan { get; set; }        
        public string? Status { get; set; }

        [Required(ErrorMessage = "Jenis urut harus diisi.")]
        public string? Urut { get; set; } = string.Empty;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
