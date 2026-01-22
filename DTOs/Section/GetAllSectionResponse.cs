namespace astratech_apps_backend.DTOs.Section
{
    public class GetAllSectionResponse
    {
        public List<SectionDto> Data { get; set; } = [];
        public int TotalData { get; set; } = 0;
        public int TotalHalaman { get; set; } = 0;
    }
}
