namespace astratech_apps_backend.DTOs.TatapMuka
{
    public class GetAllListProdi
    {
        public string KonsentrasiId { get; set; } = string.Empty;
        public string KonsentrasiNama { get; set; } = string.Empty;
    }

    public class GetProdiByUser
    {
        public string KonsentrasiId { get; set; } = string.Empty;
    }

    public class GetProdiByNIM
    {
        public string KonsentrasiId {get; set;} = string.Empty;
    }
}