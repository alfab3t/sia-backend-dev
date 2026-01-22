namespace astratech_apps_backend.DTOs.LihatAlokasiPembimbing
{
    public class GetAllLihatAlokasiPembimbingResponse
    {
        public List<LihatAlokasiPembimbingDto> Data { get; set; } = [];
        public int TotalData { get; set; } = 0;
        public int TotalHalaman { get; set; } = 0;
    }
}
