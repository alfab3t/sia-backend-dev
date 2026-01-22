namespace astratech_apps_backend.DTOs.PeriodeDaftarUlang
{
    public class GetAllPeriodeDaftarUlangResponse
    {
        public List<PeriodeDaftarUlangDto> Data { get; set; } = [];
        public int TotalData { get; set; } = 0;
        public int TotalHalaman { get; set; } = 0;
    }
}
