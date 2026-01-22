using System.Collections.Generic;

namespace astratech_apps_backend.DTOs.JenisSurat
{
    public class GetAllJenisSuratResponse
    {
        public List<JenisSuratDto> Data { get; set; } = new();
        public int TotalData { get; set; } = 0;
        public int TotalHalaman { get; set; } = 0;
    }
}