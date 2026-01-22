namespace astratech_apps_backend.DTOs.Remedial
{
    public class GetAllRemedialRequest
    {
        public string? SearchKeyword { get; set; }
        public string? KonsentrasiId { get; set; }
        public string? TahunAjaran { get; set; }
        public string? Semester { get; set; }
        public string? IdMataKuliah { get; set; }
        public string? Kelas { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}