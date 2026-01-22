namespace astratech_apps_backend.Models
{
    public class PeriodeDaftarUlang
    {
        public int Id { get; set; }
        public long RowNumber { get; set; } 
        public string TahunAjaran { get; set; } = string.Empty;
        public DateTime TanggalMulai { get; set; }
        public DateTime TanggalAkhir { get; set; }
        public string Status { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string? ModifiedBy { get; set; } 
        public DateTime? ModifiedDate { get; set; } 
    }
}