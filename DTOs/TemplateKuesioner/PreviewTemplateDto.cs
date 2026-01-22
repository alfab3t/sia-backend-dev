namespace astratech_apps_backend.DTOs.TemplateKuesioner
{
    public class PreviewTemplateDto
    {
        public int TemplateId { get; set; }
        public int SkalaMax { get; set; }

        public List<PreviewPertanyaanDto> PertanyaanList { get; set; } = [];
    }

    public class PreviewPertanyaanDto
    {
        public int Id { get; set; }
        public string Pertanyaan { get; set; } = string.Empty;
        public bool IsHeader { get; set; }
        public string Jenis { get; set; } = string.Empty;
    }
}