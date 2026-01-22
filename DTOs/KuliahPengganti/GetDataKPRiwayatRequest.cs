namespace astratech_apps_backend.DTOs.KuliahPengganti
{
    public class GetKPRiwayatRequest
    {
        public string SearchText { get; set; } = string.Empty;
        public string SortBy { get; set; } = "kpe_id";
        public string KonsentrasiId { get; set; } = string.Empty;
        public string TahunAjaran { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public string ProdiCode { get; set; } = string.Empty;
        public string RoleCode { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }

}
