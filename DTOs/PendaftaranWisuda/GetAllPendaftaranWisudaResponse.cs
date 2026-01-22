namespace astratech_apps_backend.DTOs.PendaftaranWisuda
{
    public class GetAllPendaftaranWisudaResponse
    {
        public List<PendaftaranWisudaDto> Data { get; set; } = [];
        public int TotalData { get; set; } = 0;
        public int TotalHalaman { get; set; } = 0;
    }
}
