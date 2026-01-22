namespace astratech_apps_backend.DTOs.WaliMahasiswa
{
    public class GetAllWaliMhsResponse
    {
        public List<WaliMahasiswaDto> Data { get; set; } = [];
        public int TotalData { get; set; } = 0;
        public int TotalHalaman { get; set; } = 0;
    }
}
