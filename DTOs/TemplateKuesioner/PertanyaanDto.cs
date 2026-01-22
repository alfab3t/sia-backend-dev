namespace astratech_apps_backend.DTOs.TemplateKuesioner
{
    public class PertanyaanDto
    {
        public int Id { get; set; }
        public string PertanyaanText { get; set; } = string.Empty;
        public string IsHeader { get; set; } = string.Empty;
        public string Jenis { get; set; } = string.Empty;
    }
}