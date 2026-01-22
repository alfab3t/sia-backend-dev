namespace astratech_apps_backend.DTOs.PendaftaranWisuda
{
    public class PendaftaranWisudaDto
    {
        public int Id { get; set; }
        public string TahunAkademik { get; set; } = string.Empty;
        public string TanggalMulaiPendaftaran { get; set; } = string.Empty;
        public string TanggalAkhirPendaftaran { get; set; } = string.Empty;
        public int? KuotaUndanganTambahan { get; set; }
    }
}
