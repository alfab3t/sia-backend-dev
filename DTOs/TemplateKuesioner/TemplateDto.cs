namespace astratech_apps_backend.DTOs.TemplateKuesioner
{
    public class TemplateDto
    {
        public int Id { get; set; }
        public long RowNumber { get; set; }
        public string NamaTemplate { get; set; } = string.Empty;
        public string JenisKuesioner { get; set; } = string.Empty;
        public string Skala { get; set; } = string.Empty;
        public string TanggalFinal { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}