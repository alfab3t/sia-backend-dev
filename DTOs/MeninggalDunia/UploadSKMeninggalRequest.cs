namespace astratech_apps_backend.DTOs.MeninggalDunia
{
    public class UploadSKMeninggalRequest
    {
        public string MduId { get; set; } = "";
        public IFormFile? SK { get; set; }        
        public IFormFile? SKPB { get; set; }     
        public string ModifiedBy { get; set; } = "";
    }
}
