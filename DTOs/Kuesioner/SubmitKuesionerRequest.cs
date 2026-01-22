using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.Kuesioner
{
    public class SubmitKuesionerRequest
    {
        [Required]
        public int KuesionerId { get; set; }

        public string? KritikSaran { get; set; } = "";

        public List<JawabanItemDto> JawabanList { get; set; } = [];
    }

    public class JawabanItemDto
    {
        public int PertanyaanId { get; set; }
        public string Jawaban { get; set; } = string.Empty;
    }
}