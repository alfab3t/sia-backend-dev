namespace astratech_apps_backend.DTOs.JenisBeasiswa
{
    public class GetAllJenisBeasiswaDropdownResponse
    {
        public List<JenisBeasiswaDto> Data { get; set; } = [];
        public int TotalData { get; set; } = 0;
        public int TotalHalaman { get; set; } = 0;
    }
}
