using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace astratech_apps_backend.DTOs.Jam_Minus
{
    public class GetAllJamMinusRequest
    {
        public string Keyword { get; set; } = string.Empty;
        public string Sort { get; set; } = "mhs_id asc";
        public string Prodi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tahun Akademik harus diisi.")]
        public string TahunAkademik { get; set; } = string.Empty;

        [Required(ErrorMessage = "Semester harus diisi.")]
        public string Semester { get; set; } = string.Empty;
        public string Kelas { get; set; } = string.Empty;

        [JsonIgnore]
        public string Username { get; set; } = string.Empty;

        [JsonIgnore]
        public string RoleID { get; set; } = string.Empty;
        public int PageSize { get; set; } = 10;
        public int Page { get; set; } = 1;
    }
}
