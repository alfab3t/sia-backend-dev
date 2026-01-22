namespace astratech_apps_backend.DTOs.RekapitulsiIndustri
{
    public class RekapitulasiIndustriDto
    {
        public string TahunAjaran { get; set; } = string.Empty;
        public string NamaIndustri { get; set; } = string.Empty;
        public string GrupIndustri { get; set; } = string.Empty;
        public int p4 { get; set; }
        public int tpm { get; set; }
        public int mi { get; set; }
        public int to { get; set; }
        public int mk { get; set; }
        public int tab { get; set; }
        public int tphp { get; set; }
        public int jumlah { get; set; }
    }
}
