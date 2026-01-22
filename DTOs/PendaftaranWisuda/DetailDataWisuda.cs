namespace astratech_apps_backend.DTOs.PendaftaranWisuda
{
    public class DetailDataWisuda
    {
        public int Id { get; set; }
        public string NomorPendaftaran { get; set; } = string.Empty;
        public DateTime? TanggalDaftar { get; set; }
        public string Prodi { get; set; } = string.Empty;
        public string NIM { get; set; } = string.Empty;
        public string Nama { get; set; } = string.Empty;
        public string KelengkapanBerkas { get; set; } = string.Empty;
        public string StatusPembayaran { get; set; } = string.Empty;
        public DateTime? TanggalBayar { get; set; }
        public int UndanganTambahan { get; set; } 
    }

}
