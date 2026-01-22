namespace astratech_apps_backend.DTOs.SkalaPenilaian
{
    public class GetAllSkalaPenilaianResponse
    {
        public List<SkalaPenilaianDto> Data { get; set; } = [];
        public int TotalData { get; set; }
        public int TotalHalaman { get; set; }
    }
}