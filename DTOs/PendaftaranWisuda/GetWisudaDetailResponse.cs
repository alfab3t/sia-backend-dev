namespace astratech_apps_backend.DTOs.PendaftaranWisuda
{
    public class GetWisudaDetailResponse
    {
        public string Nim { get; set; } = string.Empty;
        public string Nama { get; set; } = string.Empty;
        public string Nik { get; set; } = string.Empty;

        public string TempatLahir { get; set; } = string.Empty;
        public DateTime? TanggalLahir { get; set; }

        public string Hp { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public string VirtualAccount { get; set; } = string.Empty;

        public DateTime? TanggalDaftar { get; set; }
        public DateTime? TanggalBayar { get; set; }

        public string StatusPembayaran { get; set; } = string.Empty;
        public string StatusBerkas { get; set; } = string.Empty;

        public int Undangan { get; set; }
        public decimal Nominal { get; set; }

        public string Foto { get; set; } = string.Empty;
        public string Alamat { get; set; } = string.Empty;
    }
}
