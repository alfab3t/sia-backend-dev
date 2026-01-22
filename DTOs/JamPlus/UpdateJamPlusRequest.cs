namespace astratech_apps_backend.DTOs.JamPlusController
{
    public class UpdateJamPlusRequest
    {
        public string Id { get; set; } = string.Empty;
        public string Jenis { get; set; } = string.Empty;
        public string Deskripsi { get; set; } = string.Empty;
        public int JumlahJam { get; set; } = 0;
    }
}