using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.MeninggalDunia
{
    public class UpdateMeninggalDuniaRequest
    {
        // ID tidak perlu di DTO karena sudah ada di route parameter
        
        // MhsId optional - tidak wajib untuk update
        public string? MhsId { get; set; }
        
        // Lampiran string optional - tidak wajib untuk update  
        public string? Lampiran { get; set; }
        
        // File lampiran optional - user bisa choose untuk update atau tidak
        public IFormFile? LampiranFile { get; set; }
    }
}
