namespace astratech_apps_backend.Models.LihatAlokasiPembimbing
{
    public class Konsentrasi
    {
        public int KonsentrasiId { get; set; } 
        public string NamaKonsentrasi { get; set; } = string.Empty;
    }

    public class KonsentrasiByDosen
    {
        public int KonsentrasiId { get; set; }
        public string NamaKonsentrasi { get; set; } = string.Empty;
    }

    public class KonsentrasiByNPK
    {
        public int KonsentrasiId { get; set; }
        public string NamaKonsentrasi { get; set; } = string.Empty;
    }
}
