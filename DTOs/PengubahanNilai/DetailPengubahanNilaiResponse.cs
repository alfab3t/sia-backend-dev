namespace astratech_apps_backend.DTOs.PengubahanNilai
{
    public class DetailPengubahanNilaiResponse
    {
        public string TahunAjaran { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public string MataKuliah { get; set; } = string.Empty;
        public string KelasId { get; set; } = string.Empty;
        public string Dosen { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string AlasanTolak { get; set; } = string.Empty;
        public string MataKuliahId { get; set; } = string.Empty;
        public string AlasanPengajuan { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
        public string DibuatOleh { get; set; } = string.Empty;
        public string TanggalDibuat { get; set; } = string.Empty;
    }
}