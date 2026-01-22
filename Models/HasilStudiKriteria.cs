namespace astratech_apps_backend.Models.HasilStudiKriteria
{
    public class Kriteria
    {
        public string Id { get; set; } = string.Empty;
        public long RowNumber { get; set; } = 0;
        public string MataKuliahId { get; set; } = string.Empty;
        public string TahunAjaran { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public string DosenId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string AlasanTolak { get; set; } = string.Empty;
        public string TipeKriteria { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
        public DateTime UpdatedDate { get; set; }
        public string MataKuliah { get; set; } = string.Empty;
        public string Dosen { get; set; } = string.Empty;
        public string Konsentrasi { get; set; } = string.Empty;
        public string KonsentrasiId { get; set; } = string.Empty;
        public List<KriteriaDetail> Details { get; set; } = new List<KriteriaDetail>();
    }

    public class KriteriaDetail
    {
        public string Id { get; set; } = string.Empty;
        public string KriteriaId { get; set; } = string.Empty;
        public string Kriteria { get; set; } = string.Empty;
        public int Persentase { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
    }
}