using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.MeninggalDunia
{
    public class UpdateMeninggalDuniaRequest
    {
        [Required(ErrorMessage = "ID meninggal dunia harus diisi")]
        public string Id { get; set; } = "";
        
        [Required(ErrorMessage = "ID mahasiswa harus diisi")]
        public string MhsId { get; set; } = "";
        
        [Required(ErrorMessage = "Lampiran harus diisi")]
        public string Lampiran { get; set; } = "";
        
        [Required(ErrorMessage = "File lampiran harus diupload")]
        public IFormFile LampiranFile { get; set; } = null!;
    }
}
