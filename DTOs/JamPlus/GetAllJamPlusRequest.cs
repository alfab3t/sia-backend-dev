using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace astratech_apps_backend.DTOs.JamPlus
{
    public class GetAllJamPlusRequest
    {
       
        public string? Keyword { get; set; } = "";

        public string Sort { get; set; } = "mhs_id asc";
               
        public string? Prodi { get; set; } = "";
              
        [Required(ErrorMessage = "Tahun Akademik harus diisi.")]
        public string TahunAkademik { get; set; } = string.Empty;

        [Required(ErrorMessage = "Semester harus diisi.")]
        public string Semester { get; set; } = string.Empty;
           
        public string? Kelas { get; set; } = "";

        [JsonIgnore]
        public string? Username { get; set; } = "";

        [JsonIgnore]
        public string? RoleID { get; set; } = "";
        public int PageSize { get; set; } = 10;
        public int Page { get; set; } = 1;
    }
}