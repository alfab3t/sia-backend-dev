using astratech_apps_backend.DTOs.Institusi;

namespace astratech_apps_backend.DTOs.Kamar_Dormitory
{
    public class GetAllKamarDormitoryResponse
    {
        public List<KamarDormitoryDto> Data { get; set; } = [];
        public int TotalData { get; set; } = 0;
        public int TotalHalaman { get; set; } = 0;
    }
}
