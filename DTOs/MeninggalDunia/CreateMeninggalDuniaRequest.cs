using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.MeninggalDunia
{
    public class CreateMeninggalDuniaRequest
    {
        [Required(ErrorMessage = "Mahasiswa harus dipilih")]
        public string MhsId { get; set; } = "";
        
        [Required(ErrorMessage = "Lampiran file harus diupload")]
        public IFormFile LampiranFile { get; set; } = null!;
    }
}
