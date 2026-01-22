namespace astratech_apps_backend.DTOs.LihatAlokasiPembimbing
{
    public class GetListKonsentrasiResponse
    {
        public List<KonsentrasiDto> Data { get; set; } = [];
        public int Status { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}