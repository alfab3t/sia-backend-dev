namespace astratech_apps_backend.Models
{
    public class TemplateKuesioner
    {
        public int Id { get; set; }
        public long RowNumber { get; set; } = 0;
        public string NamaTemplate { get; set; } = string.Empty;
        public string JenisKuesioner { get; set; } = string.Empty;
        public string Skala { get; set; } = string.Empty;

        public int IdJenisKuesioner { get; set; }
        public int IdSkala { get; set; }
    
        public DateTime? TanggalFinal { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}