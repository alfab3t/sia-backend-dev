namespace astratech_apps_backend.DTOs.JamMinusPlus
{
    public class GetAllJamMinusPlusResponse
    {
        public List<JamMinusPlusDto> ListData { get; set; } = new();
        public int TotalData { get; set; }
        public int TotalHalaman { get; set; }
    }
}
