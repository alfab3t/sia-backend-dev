namespace astratech_apps_backend.DTOs.RekapitulsiIndustri
{
    public class RekapitulasiIndustriPivotDto
    {
        public string ProgramStudi { get; set; } = string.Empty;
        public Dictionary<string, int> JumlahPerIndustri { get; set; }= new();
        public int Total { get; set; }
        public int JumlahMahasiswa { get; set; }
        public int Sisa { get; set; }
    }
}
