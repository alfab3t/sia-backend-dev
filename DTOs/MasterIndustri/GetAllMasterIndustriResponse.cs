namespace astratech_apps_backend.DTOs.MasterIndustri
{
    public class GetAllMasterIndustriResponse
    {
        public List<MasterIndustriDto> Data { get; set; } = [];
        public int TotalData { get; set; } = 0;
        public int TotalHalaman { get; set; } = 0;
    }
}
