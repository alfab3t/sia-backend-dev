namespace astratech_apps_backend.Models
{
    public class InstitusiBeasiswa
    {
        public int Id { get; set; }
        public long RowNumber { get; set; } = 0;
        public string NamaInstitusiBeasiswa { get; set; } = string.Empty;
        public string Alamat { get; set; } = string.Empty;
        public string Telepon { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
