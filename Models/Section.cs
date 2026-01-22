namespace astratech_apps_backend.Models
{
    public class Section
    {
        public int Id { get; set; }
        public long RowNumber { get; set; } = 0;
        public string NamaSection { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
