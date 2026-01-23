using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.CutiAkademik
{
    public class CreateDraftCutiAkademikRequest
    {
        [Required(ErrorMessage = "ID mahasiswa harus diisi")]
        public string MhsId { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Tahun ajaran harus diisi")]
        public string TahunAjaran { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Semester harus diisi")]
        public string Semester { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lampiran surat pengajuan harus diupload")]
        public IFormFile LampiranSuratPengajuan { get; set; } = null!;

        [Required(ErrorMessage = "Lampiran harus diupload")]
        public IFormFile Lampiran { get; set; } = null!;

        [Required(ErrorMessage = "CreatedBy harus diisi")]
        public string CreatedBy { get; set; } = string.Empty;
    }
}