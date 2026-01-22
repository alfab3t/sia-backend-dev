namespace astratech_apps_backend.DTOs.JamMinusPlus
{
    public class JamMinusPlusDetailDto
    {
        public string TahunAkademik { get; set; } = string.Empty;
        public string Tanggal { get; set; } = string.Empty;
        public string Keterangan { get; set; } = string.Empty;
        public decimal KompensasiMinus { get; set; }
        public decimal KompensasiPlus { get; set; }
        public decimal MurniMinus { get; set; }
        public decimal MurniPlus { get; set; }
    }
}
