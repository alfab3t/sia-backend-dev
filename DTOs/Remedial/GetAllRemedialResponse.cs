namespace astratech_apps_backend.DTOs.Remedial
{
    public class GetAllRemedialResponse
    {
        public List<RemedialDto> Data { get; set; } = new();
        public int TotalData { get; set; } = 0;
        public int TotalHalaman { get; set; } = 0;
    }
}