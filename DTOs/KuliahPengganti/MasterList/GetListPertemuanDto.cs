namespace astratech_apps_backend.DTOs.KuliahPengganti
{
    public class GetListPertemuanDto
    {
       
        public string KonsentrasiId { get; set; } = string.Empty;
        public string TahunAjaran { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public string MataKuliahId { get; set; } = string.Empty;
        public string GrupId { get; set; } = string.Empty;
        public string SectionId { get; set; } = string.Empty;

        
        public string PertemuanId { get; set; } = string.Empty;
        public string JadwalPertemuan { get; set; } = string.Empty;
    }
}
