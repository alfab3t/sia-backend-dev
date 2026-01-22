using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.RekapitulsiIndustri
{
    public class GetDetailRekapitulasiIndustriRequest

    {
        public string NamaIndustri { get; set; } = string.Empty;
        public string TahunAjaran { get; set; } = string.Empty;
    }
}
