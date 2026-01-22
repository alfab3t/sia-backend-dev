namespace astratech_apps_backend.Models
{
    public class Pengumuman
    {
        public int IdPengumuman { get; set; }
        public string IdAplikasi { get; set; } = string.Empty;
        public string NamaAplikasi { get; set; } = string.Empty;
        public int IdUserPengumuman { get; set; }
        public int IdSekProdi { get; set; }
        public string KpdPengumuman { get; set; } = string.Empty;
        public string SubyekPengumuman { get; set; } = string.Empty;
        public string IsiPengumuman { get; set; } = string.Empty;
        public int StatusBaca { get; set; }

        public DateTime TanggalMulaiPengumuman { get; set; }
        public DateTime TanggalSelesaiPengumuman { get; set; }
        public string StatusPengumuman { get; set; } = string.Empty;
        public string DibuatOleh { get; set; } = string.Empty;
        public DateTime TanggalDibuat { get; set; }
        public string DiubahOleh { get; set; } = string.Empty;
        public DateTime TanggalDiubah { get; set; }

        public long RowNumber { get; set; }

        public string TanggalPengumumanF { get; set; } = string.Empty;

    }
}
