namespace astratech_apps_backend.DTOs.Bantuan.FAQ
{
    public class GetAllFAQResponse
    {
        public List<FAQDto> Data { get; set; } = [];
        public int TotalData { get; set; } = 0;
        public int TotalHalaman { get; set; } = 0;
    }
}