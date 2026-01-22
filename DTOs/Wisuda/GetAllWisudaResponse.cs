namespace astratech_apps_backend.DTOs.Wisuda
{
    public class GetAllWisudaResponse
    {
        public List<WisudaDto> Data { get; set; } = [];
        public int TotalData { get; set; } = 0;
        public int TotalHalaman { get; set; } = 0;
    }
}
