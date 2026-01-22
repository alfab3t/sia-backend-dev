namespace astratech_apps_backend.DTOs.PendaftaranWisuda
{
    public class GetDetailPendaftaranWisudaResponse
    {
       
        public PendaftaranWisudaDto Head { get; set; } = new();
        public List<DetailDataWisuda> Data { get; set; } = new();

        public int TotalData { get; set; }
        public int TotalHalaman { get; set; }
    }
}
