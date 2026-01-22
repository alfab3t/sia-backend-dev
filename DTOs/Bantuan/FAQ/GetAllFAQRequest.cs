namespace astratech_apps_backend.DTOs.Bantuan.FAQ
{
    public class GetAllFAQRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchKeyword { get; set; }
        public string? Status { get; set; }
        public string? Urut { get; set; } = "urutan_asc";
        public int KategoriId { get; set; } = 0;
    }
}