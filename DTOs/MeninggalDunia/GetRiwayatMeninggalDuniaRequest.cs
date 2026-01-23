using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.MeninggalDunia
{
    public class GetRiwayatMeninggalDuniaRequest
    {
        //optional
        public string Keyword { get; set; } = "";
        public string Sort { get; set; } = "mdu_created_date desc";
        public string Konsentrasi { get; set; } = "";
        public string RoleId { get; set; } = "";

        [Range(1, int.MaxValue, ErrorMessage = "Page number harus lebih dari 0")]
        public int PageNumber { get; set; } = 1;
        
        [Range(1, 100, ErrorMessage = "Page size harus antara 1-100")]
        public int PageSize { get; set; } = 10;
    }
}
