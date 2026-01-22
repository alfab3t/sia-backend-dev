namespace astratech_apps_backend.DTOs.Mahasiswa
{
    public class JenisMahasiswaResponse
    {
        public List<JenisMahasiswaItemDto> Data { get; set; } = [];
        public bool Error { get; set; }
        public required string Message { get; set; }
    }
}
