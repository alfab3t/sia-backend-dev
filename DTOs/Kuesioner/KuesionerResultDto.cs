namespace astratech_apps_backend.DTOs.Kuesioner
{
    public class KuesionerResultDto
    {
        public string NamaKuesioner { get; set; } = string.Empty;
        public string TahunAkademik { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public int SkalaMax { get; set; }
        public List<KuesionerSummaryDto> Summary { get; set; } = [];
        public List<KritikSaranDto> KritikSaran { get; set; } = [];
    }

    public class KuesionerSummaryDto
    {
        public int PertanyaanId { get; set; }
        public string PertanyaanText { get; set; } = string.Empty;
        public bool IsHeader { get; set; }
        public string JenisPertanyaan { get; set; } = string.Empty;

        public Dictionary<string, int> PilihanJawabanCount { get; set; } = [];

        public int TotalResponden { get; set; }
        public decimal RataRata { get; set; }
    }

    public class KritikSaranDto
    {
        public string Komentar { get; set; } = string.Empty;
    }
}