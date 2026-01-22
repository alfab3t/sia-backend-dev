namespace astratech_apps_backend.DTOs.LihatAlokasiPembimbing
{
    public class KonsentrasiDto
    {
        public int KonsentrasiId { get; set; }
        public string NamaKonsentrasi { get; set; } = string.Empty;
    }

    public class KonsentrasiByDosenDto
    {
        public int KonsentrasiId { get; set; }
        public string NamaKonsentrasi { get; set; } = string.Empty;
    }

    public class KonsentrasiByNPKDto
    {
        public int KonsentrasiId { get; set; }
        public string NamaKonsentrasi { get; set; } = string.Empty;
    }
}
