namespace astratech_apps_backend.DTOs.RekapitulsiIndustri
{
    public class GetAllRekapitulasiIndustriResponse
    {
        public List<RekapitulasiIndustriDto> Data { get; set; } = [];
        public int TotalData { get; set; } = 0;
        public int TotalHalaman { get; set; } = 0;
    }
}
