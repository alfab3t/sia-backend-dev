using astratech_apps_backend.DTOs.Institusi;

namespace astratech_apps_backend.DTOs.Mahasiswa
{
    public class GetAllMahasiswaResponse
    {
        public List<MahasiswaDto> Data { get; set; } = [];
        public int TotalData { get; set; } = 0;
        public int TotalHalaman { get; set; } = 0;
    }
}
