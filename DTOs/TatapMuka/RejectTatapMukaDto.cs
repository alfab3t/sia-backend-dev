namespace astratech_apps_backend.DTOs.TatapMuka
{
    public class RejectTatapMukaRequestDto
    {
      public string TatapMukaId { get; set; } = string.Empty;
      public string ModifBy { get; set; } = string.Empty;
      public string AlasanTolak { get; set; } = string.Empty;
    }
}