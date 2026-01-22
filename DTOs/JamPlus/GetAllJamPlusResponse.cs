namespace astratech_apps_backend.DTOs.JamPlus
{
    public class GetAllJamPlusResponse
    {
        public List<JamPlusDto> ListData { get; set; } = new();
        public int TotalData { get; set; } = 0;
        public int TotalHalaman { get; set; } = 0;
    }
}           
    