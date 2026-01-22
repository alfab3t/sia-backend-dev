namespace astratech_apps_backend.DTOs.KuliahPengganti
{
    public class EditPengajuanKPDto
    {
        public string KuliahPenggantiId { get; set; } = string.Empty;
        public string PertemuanId { get; set; } = string.Empty;
        public string RuanganId { get; set; } = string.Empty;
        public string TanggalPengajuan { get; set; } = string.Empty;
        public string JamAwal { get; set; } = string.Empty;
        public string JamAkhir { get; set; } = string.Empty;
        public string AlasanPengajuan { get; set; } = string.Empty;
        public string ModifiedBy { get; set; } = string.Empty; 
    }
}