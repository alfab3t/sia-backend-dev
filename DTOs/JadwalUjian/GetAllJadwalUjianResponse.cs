namespace astratech_apps_backend.DTOs.JadwalUjian
{
    public class GetAllJadwalUjianResponse
    {
        public List<JadwalUjianDto> Data { get; set; } = [];
        public int TotalData { get; set; } = 0;
        public int TotalHalaman { get; set; } = 0;
    }
}
