namespace astratech_apps_backend.DTOs.TatapMuka
{
    public class KelasResponeDto
    {
        public string KelasId { get; set; } = string.Empty;
    }

    public class KelasRequestDto
    {
        public string MataKuliahId { get; set; } = string.Empty;
        public string SectionId { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
    }

}
