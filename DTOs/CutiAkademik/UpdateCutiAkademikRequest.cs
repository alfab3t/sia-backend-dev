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

        [Required(ErrorMessage = "Lampiran surat pengajuan harus diupload")]
        public IFormFile LampiranSuratPengajuan { get; set; } = null!;

        [Required(ErrorMessage = "Lampiran harus diupload")]
        public IFormFile Lampiran { get; set; } = null!;

        [Required(ErrorMessage = "ModifiedBy harus diisi")]
        public string ModifiedBy { get; set; } = string.Empty;
    }
}
