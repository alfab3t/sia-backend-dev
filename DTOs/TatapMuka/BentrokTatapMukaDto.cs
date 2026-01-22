namespace astratech_apps_backend.DTOs.TatapMuka
{
    
    public class BentrokJadwalTatapMukaResponseDto
    {
        public string Subjek { get; set; } = string.Empty;
        public string MataKuliah { get; set; } = string.Empty;
        public string Tanggal { get; set; } = string.Empty;
        public string WaktuAwal { get; set; } = string.Empty;
        public string WaktuAkhir { get; set; } = string.Empty;
        public string? Konsentrasi { get; set; }
    }

    public class BentrokJadwalTatapMukaRequestDto
    {
        public string Mode { get; set; } = string.Empty;
        public string Tanggal { get; set; } = string.Empty;
        public string WaktuAwal { get; set; } = string.Empty;
        public string WaktuAkhir { get; set; } = string.Empty;
        public string DosenId { get; set; } = string.Empty;
        public string TahunAjaran { get; set; } = string.Empty;
        public string RuanganId { get; set; } = string.Empty;
        public string GrupId { get; set; } = string.Empty;
        public string SectionId { get; set; } = string.Empty;
        public string TatapMukaId { get; set; } = string.Empty;
    }
  
    public class CheckBentrokResponseDto
    {
        public string TatapMukaId { get; set; } = string.Empty;
    }

    public class CheckBentrokRequestDto
    {
        public string RuanganId { get; set; } = string.Empty;
        public string GrupId { get; set; } = string.Empty;
        public string MataKuliahId { get; set; } = string.Empty;
        public string SectionId { get; set; } = string.Empty;
        public string TahunAjaran { get; set; } = string.Empty;
        public string Tanggal { get; set; } = string.Empty;
    }

}