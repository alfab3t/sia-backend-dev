using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace astratech_apps_backend.DTOs.Kamar_Dormitory
{
    public class CreatePenghuniKamarRequest
    {
        [Required]
        public int IdKamar { get; set; }

        [Required]
        public string IdMahasiswa { get; set; }

        public string? NamaPenghuni { get; set; }

        [Required]
        public DateTime TanggalMasuk { get; set; }

        public string? AlamatKtp { get; set; }

        public DateTime? LastBilled { get; set; }

        public string? Nik { get; set; }

        public IFormFile? ScanKtp { get; set; }

        public string? ScanKtpFileName { get; set; }
    }
}
