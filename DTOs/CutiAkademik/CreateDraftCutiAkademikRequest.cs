using Microsoft.AspNetCore.Http;

namespace astratech_apps_backend.DTOs.CutiAkademik
{
    public class CreateDraftCutiAkademikRequest
    {
        public string MhsId { get; set; } = string.Empty;
        public string TahunAjaran { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;

        public IFormFile? LampiranSuratPengajuan { get; set; }

        public IFormFile? Lampiran { get; set; }

        public string CreatedBy { get; set; } = string.Empty;
    }
}