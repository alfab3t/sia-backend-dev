using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.CutiAkademik
{
    public class UpdateCutiAkademikRequest
    {
        [Required(ErrorMessage = "ID cuti akademik harus diisi")]
        public string Id { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Tahun ajaran harus diisi")]
        public string TahunAjaran { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Semester harus diisi")]
        public string Semester { get; set; } = string.Empty;

        public IFormFile? LampiranSuratPengajuan { get; set; }

        public IFormFile? Lampiran { get; set; }

        [Required(ErrorMessage = "ModifiedBy harus diisi")]
        public string ModifiedBy { get; set; } = string.Empty;
    }
}
