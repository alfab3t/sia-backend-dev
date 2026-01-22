namespace astratech_apps_backend.Models
{
    public class KuesionerDetail
    {
        public int Id { get; set; }
        public string NamaKuesioner { get; set; } = string.Empty;
        public string SkalaDefinisi { get; set; } = string.Empty;
        public int SkalaMax { get; set; }
        public string? KritikSaran { get; set; }
        public List<PertanyaanJawaban> PertanyaanList { get; set; } = [];
    }
}