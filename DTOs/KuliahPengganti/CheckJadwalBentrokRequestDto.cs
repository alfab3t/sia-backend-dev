namespace astratech_apps_backend.DTOs.KuliahPengganti
{
    public class CheckJadwalBentrokRequestDto
    {
        public string ModeBentrok { get; set; } = string.Empty;          
        public string TanggalPengajuan { get; set; } = string.Empty;       
        public string JamAwal { get; set; } = string.Empty;     
        public string JamAkhir { get; set; } = string.Empty;    
        public string DosenId { get; set; } = string.Empty;
        public string RuanganId { get; set; } = string.Empty;
        public string GrupId { get; set; } = string.Empty;
        public string SectionId { get; set; } = string.Empty;
        public string? JadwalDetailId { get; set; }     
    }
}
