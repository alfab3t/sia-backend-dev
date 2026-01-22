namespace astratech_apps_backend.DTOs.JenisKuesioner
{
    public class JenisKuesionerDto
    {
        public int Id { get; set; }
        public long RowNumber { get; set; }
        public string NamaJenis { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}