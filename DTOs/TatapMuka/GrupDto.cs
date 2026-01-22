namespace astratech_apps_backend.DTOs.TatapMuka
{
    public class GrupDto
    {
        public string GrupId { get; set; } = string.Empty;
        public string Grup { get; set; } = string.Empty;
    }

    public class GrupRequestDto
    {
        public string MataKuliahId { get; set; } = string.Empty;
        public string SectionId { get; set; } = string.Empty;
        public string KelasId { get; set; } = string.Empty;
    }
}
