namespace astratech_apps_backend.DTOs.Bantuan.FAQ
{
    public class FAQDto
    {
        public int FaqId { get; set; }
        public string Pertanyaan { get; set; } = string.Empty;
        public string Jawaban { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string NamaKategori { get; set; } = string.Empty;
    }
}