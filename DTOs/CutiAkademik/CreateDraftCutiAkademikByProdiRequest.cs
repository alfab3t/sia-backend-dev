using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.CutiAkademik
{
    public class CreateDraftCutiAkademikByProdiRequest
    {
        [Required(ErrorMessage = "Tahun ajaran harus diisi")]
        public string TahunAjaran { get; set; } = "";
        
        [Required(ErrorMessage = "Semester harus diisi")]
        public string Semester { get; set; } = "";
        
        [Required(ErrorMessage = "Lampiran surat pengajuan harus diupload")]
        public IFormFile LampiranSuratPengajuan { get; set; } = null!;
        
        [Required(ErrorMessage = "Lampiran harus diupload")]
        public IFormFile Lampiran { get; set; } = null!;
        
        [Required(ErrorMessage = "ID mahasiswa harus diisi")]
        public string MhsId { get; set; } = "";
        
        [Required(ErrorMessage = "Menimbang/pertimbangan harus diisi")]
        public string Menimbang { get; set; } = "";
        
        [Required(ErrorMessage = "Approval prodi harus diisi")]
        public string ApprovalProdi { get; set; } = "";
        
        [Required(ErrorMessage = "CreatedBy harus diisi")]
        public string CreatedBy { get; set; } = "";
    }
}