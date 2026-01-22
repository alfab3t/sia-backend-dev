namespace astratech_apps_backend.DTOs.PendaftaranWisuda
{
    public class GetWisudaMahasiswaResponse
    {
        public string NomorPendaftaran { get; set; } = string.Empty;
        public DateTime? TanggalDaftar { get; set; }

        public string Prodi { get; set; } = string.Empty;
        public string TahunAkademik { get; set; } = string.Empty;

        public string StatusPembayaran { get; set; } = string.Empty;
        public string StatusBerkas { get; set; } = string.Empty;

        public DateTime? TanggalBayar { get; set; }
        public int UndanganTambahan { get; set; }
    }
}
