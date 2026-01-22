namespace astratech_apps_backend.DTOs.TagihanCalonMahasiswa
{
    public class GetAllTagihanCalonMahasiswaResponse
    {
        public List<TagihanCalonMahasiswaDto> Data { get; set; } = [];
        public int TotalData { get; set; } = 0;
        public int TotalHalaman { get; set; } = 0;
    }
}
