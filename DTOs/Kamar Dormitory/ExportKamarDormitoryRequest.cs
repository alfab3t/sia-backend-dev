using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.Kamar_Dormitory
{
    public class ExportKamarDormitoryRequest
    {
        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
        public string? Prodi { get; set; }
        public string? Angkatan { get; set; }
        public string? Status { get; set; }
        public string? NoKamar { get; set; }
    }
}
