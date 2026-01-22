namespace astratech_apps_backend.DTOs.PeriodeDaftarUlang
{
    public class PeriodeDaftarUlangDto
    {
        public long RowNumber { get; set; }
        public int Id { get; set; }
        public string TahunAjaran { get; set; } = string.Empty;
        public DateTime TanggalMulai { get; set; }
        public DateTime TanggalAkhir { get; set; }
    }
}
