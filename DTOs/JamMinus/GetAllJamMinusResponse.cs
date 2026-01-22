using astratech_apps_backend.DTOs.JamPlus;

namespace astratech_apps_backend.DTOs.Jam_Minus
{
    public class GetAllJamMinusResponse
    {
        public List<JamMinusDto> ListData { get; set; } = new();
        public int TotalData { get; set; }
        public int TotalHalaman { get; set; }
    }
}
