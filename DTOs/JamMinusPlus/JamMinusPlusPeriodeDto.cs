namespace astratech_apps_backend.DTOs.JamMinusPlus
{
    public class JamMinusPlusPeriodeDto
    {
        public string TahunAkademik { get; set; } = string.Empty;
        public List<JamMinusPlusDetailDto> History { get; set; } = new();
        public JamMinusPlusSummaryDto Summary { get; set; } = new();
    }

}
