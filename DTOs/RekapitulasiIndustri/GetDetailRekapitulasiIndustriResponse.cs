namespace astratech_apps_backend.DTOs.RekapitulsiIndustri
{
    public class GetDetailRekapitulasiIndustriResponse
    {
        public List<RekapitulasiIndustriMahasiswaDto> Data { get; set; } = [];
        public int TotalData { get; set; } = 0;
    }
}
