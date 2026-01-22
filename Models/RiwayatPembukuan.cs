namespace astratech_apps_backend.Models
{
    public class RiwayatPembukuan
    {
        public long? RowNumber { get; set; }
        public int Id { get; set; }
        public string Program { get; set; } = string.Empty;
        public string VirtualAccount { get; set; } = string.Empty;
        public string AcademicYear { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public short ProgramId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string CreatedDateStr { get; set; } = string.Empty;
        public string? ModifiedBy { get; set; }
    }
}