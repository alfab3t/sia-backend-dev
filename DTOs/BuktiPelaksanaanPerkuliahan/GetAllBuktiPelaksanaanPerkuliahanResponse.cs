namespace astratech_apps_backend.DTOs.BuktiPelaksanaanPerkuliahan
{
    public class GetAllBuktiPelaksanaanPerkuliahanResponse
    {
        public List<BuktiPelaksanaanPerkuliahanDto> Data { get; set; } = [];
        public int TotalData { get; set; } = 0;
        public int TotalHalaman { get; set; } = 0;
    }
}