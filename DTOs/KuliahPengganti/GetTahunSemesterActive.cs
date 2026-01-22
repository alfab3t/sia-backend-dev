namespace astratech_apps_backend.DTOs.KuliahPengganti
{
    public class GetTahunSemesterActive
    {
        public string TahunAjaran { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public DateTime TanggalMulai { get; set; }
    }
}