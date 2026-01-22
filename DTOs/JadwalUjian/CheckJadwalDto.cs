namespace astratech_apps_backend.DTOs.JadwalUjian
{
    public class CheckJadwalUjianRequestDto
    {
        public string idGrup { get; set; } = string.Empty;
        public string TahunAkademik { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public string Jenis { get; set; } = string.Empty;
    }

    
    public class CheckJadwalUjianResponseDto
    {
        public int StatusCode { get; set; } 
        public int IdJadwalUjian { get; set; }
        public string StatusText => StatusCode == 2 ? "Final" : "Non-Final";
    }

    
    public class CheckJadwalUjianResultDto
    {
        public bool IsExist { get; set; }
        public List<CheckJadwalUjianResponseDto> JadwalList { get; set; } = new();
    }
}