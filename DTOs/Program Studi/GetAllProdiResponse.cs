using astratech_apps_backend.DTOs.Jurusan;

namespace astratech_apps_backend.DTOs.Program_Studi
{
    public class GetAllProdiResponse
    {
        public List<ProdiDto> Data { get; set; } = [];
        public int TotalData { get; set; } = 0;
        public int TotalHalaman { get; set; } = 0;
    }
}
