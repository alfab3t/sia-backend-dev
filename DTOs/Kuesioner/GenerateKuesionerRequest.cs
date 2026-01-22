using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.Kuesioner
{
    public class GenerateKuesionerRequest
    {
        [Required]
        public string TahunAkademik { get; set; } = string.Empty;

        [Required]
        public string Semester { get; set; } = string.Empty;

        [Required]
        public int IdJenisKuesioner { get; set; }

        [Required]
        public int IdTemplateKuesioner { get; set; }

        public int? PersentaseKehadiran { get; set; }
    }
}