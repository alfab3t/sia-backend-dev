using System.Text.Json.Serialization;

namespace astratech_apps_backend.Models
{
    public class FAQ
    {
        public int FaqId { get; set; }
        public int? KategoriId { get; set; }
        public string Pertanyaan { get; set; } = string.Empty;
        public string Jawaban { get; set; } = string.Empty;
        public int Urutan { get; set; }
        public string Status { get; set; } = string.Empty;
        public string NamaKategori { get; set; } = string.Empty;
        public List<string> AksesRoleIds { get; set; } = [];
    }

    public class RoleOptionFAQ
    {
        [JsonPropertyName("Value")]  
        public string Value { get; set; } = string.Empty;

        [JsonPropertyName("Text")]  
        public string Text { get; set; } = string.Empty;
    }
}