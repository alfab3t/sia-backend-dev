namespace astratech_apps_backend.DTOs.KuliahPengganti
{
    public class GetKPRequest
    {
        public string SearchText { get; set; } = string.Empty;
        public string KonsentrasiId { get; set; } = string.Empty;
        public string TahunAjaran { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public string MataKuliahId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string ProdiCode { get; set; } = string.Empty;
        public string SortBy { get; set; } = "kpe_tanggal desc";
    }
}