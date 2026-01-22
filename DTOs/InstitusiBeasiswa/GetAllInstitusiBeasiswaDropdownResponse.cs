namespace astratech_apps_backend.DTOs.InstitusiBeasiswa
{
    public class GetAllInstitusiBeasiswaDropdownResponse
    {
        public List<InstitusiBeasiswaDto> Data { get; set; } = [];
        public int TotalData { get; set; } = 0;
        public int TotalHalaman { get; set; } = 0;
    }
}
