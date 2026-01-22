namespace astratech_apps_backend.Models
{
    public class PendaftaranWisuda
    {
        public int Id { get; set; }

        public required string TahunAkademik { get; set; }
        public required string TanggalMulaiPendaftaran { get; set; }
        public required string TanggalAkhirPendaftaran { get; set; }

        public int KuotaUndanganTambahan { get; set; }
    }
}
