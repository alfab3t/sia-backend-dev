namespace astratech_apps_backend.DTOs.Kuesioner
{
    public class KuesionerDetailDto
    {
        public int Id { get; set; }
        public string NamaKuesioner { get; set; } = string.Empty;
        public int SkalaMax { get; set; }
        public string SkalaDefinisi { get; set; } = string.Empty;
        public string? KritikSaran { get; set; }
        public bool SudahDiisi { get; set; }
        public List<PertanyaanJawabanDto> PertanyaanList { get; set; } = [];
    }

    public class PertanyaanJawabanDto
    {
        public int Id { get; set; }
        public string Pertanyaan { get; set; } = string.Empty;
        public bool IsHeader { get; set; }
        public string Jenis { get; set; } = string.Empty;
        public string? Jawaban { get; set; }
    }
}