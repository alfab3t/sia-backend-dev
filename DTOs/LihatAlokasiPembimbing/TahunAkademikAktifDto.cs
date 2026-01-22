namespace astratech_apps_backend.DTOs.LihatAlokasiPembimbing
{
    public class TahunAkademikAktifDto
    {
        public string TahunAjaran { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public DateTime TanggalMulai { get; set; }
    }
}
