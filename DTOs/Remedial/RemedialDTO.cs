namespace astratech_apps_backend.DTOs.Remedial
{
    public class RemedialDto
    {
        public long RowNumber { get; set; }
        public int NilaiMahasiswaId { get; set; }
        public string MahasiswaId { get; set; } = string.Empty;
        public string MahasiswaNama { get; set; } = string.Empty;
        public double NilaiAkhir { get; set; }
        public string AngkaMutu { get; set; } = string.Empty;
        public double? NilaiRemedial { get; set; }
        public string? AngkaMutuRemedial { get; set; }
        public bool CanEdit { get; set; }
    }
}