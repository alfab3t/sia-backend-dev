namespace astratech_apps_backend.DTOs.Kuesioner
{
    public class GetExportDataRequest
    {
        public string JenisExport { get; set; } = string.Empty;
        public string TahunAkademik { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string RoleId { get; set; } = string.Empty;
    }
}