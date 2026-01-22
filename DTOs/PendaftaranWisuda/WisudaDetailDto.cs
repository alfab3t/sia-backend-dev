namespace astratech_apps_backend.DTOs.PendaftaranWisuda
{
    public class WisudaDetailDto
    {
        public string NIM { get; set; } = string.Empty;
        public string Nama { get; set; } = string.Empty;
        public string TempatLahir { get; set; } = string.Empty;
        public DateTime? TanggalLahir { get; set; }
        public string HP { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Foto { get; set; } = string.Empty;
        public long Nominal { get; set; } 
        public int? Undangan { get; set; } 
        public string StatusPembayaran { get; set; } = string.Empty;
        public string VirtualAccount { get; set; } = string.Empty;
        public DateTime? TanggalBayar { get; set; }
        public DateTime? TanggalDaftar { get; set; }
        public string StatusBerkas { get; set; } = string.Empty;
        public string NIK { get; set; } = string.Empty;
        public string Alamat { get; set; } = string.Empty;
    }
}
