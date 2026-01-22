namespace astratech_apps_backend.DTOs.RiwayatPembukuan
{
    public class GetAllRiwayatPembukuanResponse
    {
        public int TotalData { get; set; } = 0;
        public int TotalHalaman { get; set; } = 0;
        public List<RiwayatPembukuanDto> Data { get; set; } = [];
    }
}
