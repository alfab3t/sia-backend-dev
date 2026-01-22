using System;

namespace astratech_apps_backend.Models
{
    public class JenisSurat
    {
        public int Id { get; set; } 
        public string NamaSurat { get; set; } = string.Empty; 
        public string FormatNoSurat { get; set; } = string.Empty; 
        public string FormatSurat { get; set; } = string.Empty; 
        public int? AllowMahasiswa { get; set; }
        public int? Diajukan { get; set; } 
        public string Panel { get; set; } = string.Empty; 
        public string Status { get; set; } = string.Empty; 
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? CreatedDate { get; set; } 
        public string ModifBy { get; set; } = string.Empty; 
        public DateTime? ModifDate { get; set; } 
    }
}