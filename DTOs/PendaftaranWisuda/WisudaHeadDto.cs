public class WisudaHeadDto
{
    public int Id { get; set; }
    public string TahunAkademik { get; set; } = string.Empty;
    public DateTime TanggalMulaiPendaftaran { get; set; }
    public DateTime TanggalAkhirPendaftaran { get; set; }
    public int KuotaUndanganTambahan { get; set; }
}