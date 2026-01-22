namespace astratech_apps_backend.DTOs.TatapMuka
{
    public class ApproveTatapMukaRequestDto
    {
        public string UserRole { get; set; } = string.Empty; 
        public string TatapMukaId { get; set; } = string.Empty;
        public string ApprovedBy { get; set; } = string.Empty;
        public string? RuangIdGA { get; set; } 
    }
}
