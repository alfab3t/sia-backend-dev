namespace astratech_apps_backend.DTOs.KuliahPengganti
{
    public class GetListMataKuliahDto
    {
        public string Semester { get; set; } = string.Empty;
        public string TahunAjaran { get; set; } = string.Empty;
        public string TipeMataKuliah { get; set; } = string.Empty;


        public string MataKuliahId { get; set; } = string.Empty;
        public string NamaMataKuliah { get; set; } = string.Empty;
    }
}
