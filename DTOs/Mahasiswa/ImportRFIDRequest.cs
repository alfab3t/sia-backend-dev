using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.Mahasiswa
{
    public class ImportRfidRequest
    {
        [StringLength(10)]
        public string? KonsentrasiId { get; set; }

        [StringLength(50)]
        public string? Urut { get; set; } = "NIM asc";
    }
}