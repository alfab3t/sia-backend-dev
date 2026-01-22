namespace astratech_apps_backend.Models
{
    public class Pertanyaan
    {
        public int Id { get; set; }
        public string PertanyaanText { get; set; } = string.Empty;
        public bool IsHeader { get; set; }
        public string Jenis { get; set; } = string.Empty;
    }
}