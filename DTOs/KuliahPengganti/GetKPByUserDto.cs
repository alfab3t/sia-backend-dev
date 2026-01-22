namespace astratech_apps_backend.DTOs.KuliahPengganti
{
    
    public class GetKPByDosenRequestDto
    {
        public string? SearchText { get; set; } = string.Empty;
        public string? OrderBy { get; set; } = "kpe_tanggal DESC";
        public string? KonsentrasiId { get; set; } = string.Empty;
        public string? TahunAjaran { get; set; } = string.Empty;
        public string? Semester { get; set; } = string.Empty;
        public string? MataKuliahId { get; set; } = string.Empty;
    }

    
    public class GetKPByDosenResponseDto
    {
        public string KuliahPenggantiId { get; set; } = string.Empty;
        public string KonsentrasiSingkatan { get; set; } = string.Empty;
        public string TahunAjaran { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public string NamaMataKuliah { get; set; } = string.Empty;
        public string DosenKode { get; set; } = string.Empty;
        public string JadwalPertemuan { get; set; } = string.Empty;
        public string JadwalRencana { get; set; } = string.Empty;
        public string JadwalPengganti { get; set; } = string.Empty;
        public string NamaRuangan { get; set; } = string.Empty;
        public string status { get; set; } = string.Empty;
        public string TanggalPengajuan { get; set; } = string.Empty;
    }

    public class GetKPByNimRequestDto
    {
        public string? NIM { get; set; }
        public string? SearchText { get; set; } = string.Empty;
        public string? OrderBy { get; set; } = "kpe_tanggal DESC";
        public string? KonsentrasiId { get; set; } = string.Empty;
        public string? TahunAjaran { get; set; } = string.Empty;
        public string? Semester { get; set; } = string.Empty;
        public string? MataKuliahId { get; set; } = string.Empty;
    }

    public class GetKPByNimResponseDto
    {
        public string KuliahPenggantiId { get; set; } = string.Empty;
        public string KonsentrasiSingkatan { get; set; } = string.Empty;
        public string TahunAjaran { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public string NamaMataKuliah { get; set; } = string.Empty;
        public string DosenKode { get; set; } = string.Empty;
        public string JadwalPertemuan { get; set; } = string.Empty;
        public string JadwalRencana { get; set; } = string.Empty;
        public string JadwalPengganti { get; set; } = string.Empty;
        public string NamaRuangan { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string TanggalPengajuan { get; set; } = string.Empty;
    }
}