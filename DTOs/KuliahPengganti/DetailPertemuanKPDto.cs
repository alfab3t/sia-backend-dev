namespace astratech_apps_backend.DTOs.KuliahPengganti
{
    public class DetailPertemuanKPDto
    {
        public string PertemuanId { get; set; } = string.Empty;
        public string JadwalPertemuan { get; set; } = string.Empty;
        public string MateriPertemuan { get; set; } = string.Empty;
        public int JamAwal { get; set; }
        public int JamAkhir { get; set; }
        public int MenitAwal { get; set; }
        public int MenitAkhir { get; set; }
    }

}