using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace astratech_apps_backend.DTOs.JamMinusPlus
{
    public class GetAllJamMinusPlusRequest
    {
        public string? Keyword { get; set; } = string.Empty;
        public string Sort { get; set; } = "mhs_id asc";
        public string? KonId { get; set; } = string.Empty;
        public string? KelasId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tahun Akademik harus diisi.")]
        public string TahunAkademik { get; set; } = string.Empty;

        [Required(ErrorMessage = "Semester harus diisi.")]
        public string Semester { get; set; } = string.Empty;

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        [JsonIgnore] public string? Username { get; set; }
        [JsonIgnore] public string? RoleID { get; set; }
    }
}
