public class ExportPesertaWisudaDto
{
    public string NomorPendaftaran { get; set; } = string.Empty;
    public DateTime? TanggalDaftar { get; set; }
    public string Prodi { get; set; } = string.Empty;
    public string NIM { get; set; } = string.Empty;
    public string NamaMahasiswa { get; set; } = string.Empty;
    public string KelengkapanBerkas { get; set; } = string.Empty;
    public string StatusPembayaran { get; set; } = string.Empty;
    public DateTime? TanggalBayar { get; set; }
    public int UndanganTambahan { get; set; } = 0;
}
