namespace astratech_apps_backend.DTOs.PengubahanNilai
{
    public class MataKuliahResponse
    {
        public List<MataKuliahDto> Data { get; set; } = [];
        public string ErrorMessage { get; set; } = string.Empty;
    }

    public class MataKuliahDto
    {
        public string MataKuliahId { get; set; } = string.Empty;
        public string MataKuliahNama { get; set; } = string.Empty;
    }
}