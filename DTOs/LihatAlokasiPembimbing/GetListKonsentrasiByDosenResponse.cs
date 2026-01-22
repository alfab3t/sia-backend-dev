namespace astratech_apps_backend.DTOs.LihatAlokasiPembimbing
{
    public class GetListKonsentrasiByDosenResponse
    {
        public List<KonsentrasiByDosenDto> Data { get; set; } = [];
        public int Status { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
