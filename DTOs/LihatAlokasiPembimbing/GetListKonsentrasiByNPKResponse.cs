namespace astratech_apps_backend.DTOs.LihatAlokasiPembimbing
{
    public class GetListKonsentrasiByNPKResponse
    {
        public List<KonsentrasiByNPKDto> Data { get; set; } = [];
        public int Status { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
