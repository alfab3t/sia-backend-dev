namespace astratech_apps_backend.DTOs.KuliahPengganti
{
    public class ApprovePengajuanKPDto
    {
        public string KuliahPenggantiId { get; set; } = string.Empty;
        public string Approver { get; set; } = string.Empty;   
        public string Role { get; set; } = string.Empty;       
        public string Status { get; set; } = string.Empty;     
        public string RuanganId { get; set; } = string.Empty;   
    }
}
