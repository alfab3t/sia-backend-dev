namespace astratech_apps_backend.DTOs.KuliahPengganti
{
    public class CheckJadwalBentrokResponseDto
    {
        public bool IsBentrok { get; set; }
        public string Subjek { get; set; } = string.Empty;
        public string? NamaMataKuliah { get; set; }
        public string? TanggalPengajuan { get; set; }
        public string? JamAwal { get; set; }
        public string? JamAkhir { get; set; }
        public string? NamaKonsentrasi { get; set; }
    }
}
