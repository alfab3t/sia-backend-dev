namespace astratech_apps_backend.DTOs.PendaftaranWisuda
{
    public class DetailPendaftaranWisudaDto
    {
        public string TahunAkademik { get; set; } = string.Empty;

        public DateTime TanggalMulaiPendaftaran { get; set; }

        public DateTime TanggalAkhirPendaftaran { get; set; }

        public int KuotaTerpakai { get; set; }

        public int KuotaTotal { get; set; }
    }
}
