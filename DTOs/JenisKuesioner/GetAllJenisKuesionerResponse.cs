namespace astratech_apps_backend.DTOs.JenisKuesioner
{
    public class GetAllJenisKuesionerResponse
    {
        public List<JenisKuesionerDto> Data { get; set; } = [];
        public int TotalData { get; set; } = 0;
        public int TotalHalaman { get; set; } = 0;
    }
}