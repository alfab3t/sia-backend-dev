namespace astratech_apps_backend.DTOs.Pengumuman
{
    public class GetAllPengumumanResponse
    {
        public List<PengumumanDto> Data { get; set; } = [];
        public int TotalData { get; set; } = 0;
        public int TotalHalaman { get; set; } = 0;
    }
}
