namespace astratech_apps_backend.DTOs.RiwayatPembukuan
{
    public class RiwayatPembukuanDto
    {
        public long RowNumber { get; set; }
        public int Id { get; set; }
        public string ProgramStudi { get; set; } = string.Empty;
        public string VirtualAccount { get; set; } = string.Empty;
        public string TahunAkademik { get; set; } = string.Empty;
        public string NIM { get; set; } = string.Empty;
        public string NamaMahasiswa { get; set; } = string.Empty;
        public DateTime Tanggal { get; set; }
        public decimal Jumlah { get; set; }
        public string Keterangan { get; set; } = string.Empty;
    }
}
