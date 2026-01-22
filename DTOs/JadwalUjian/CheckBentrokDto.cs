namespace astratech_apps_backend.DTOs.JadwalUjian
{
    public class CheckBentrokResponseDto
    {
        public int IdJadwalUjian { get; set; }
        public string Kelas { get; set; } = string.Empty;
        public string MataKuliah { get; set; } = string.Empty;
        public int IdMataKuliah { get; set; }
        public int? IdSection { get; set; }
        public string IdGrup { get; set; } = string.Empty;
        public int IdRuang { get; set; }
        public int IdDosen1 { get; set; }
        public int? IdDosen2 { get; set; }
        public DateTime Tanggal { get; set; }
        public TimeSpan WaktuMulai { get; set; }
        public TimeSpan WaktuSelesai { get; set; }
        public string Konsentrasi { get; set; } = string.Empty;
    }

    public class CheckBentrokResultDto
    {
        public bool IsBentrok { get; set; }
        public List<CheckBentrokResponseDto> JadwalBentrok { get; set; } = new();
    }
}