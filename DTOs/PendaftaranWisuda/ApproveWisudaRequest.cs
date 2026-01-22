namespace astratech_apps_backend.DTOs.PendaftaranWisuda
{
    public class ApproveWisudaRequest
    {
        public string WisudaId { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime? TanggalBayar { get; set; }
    }
}