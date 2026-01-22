using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.JadwalUjian
{
public class GetAllJadwalUjianRequest
{
    public string SearchTerm { get; set; } = string.Empty;
    public string SortColumn { get; set; } = "id";

    public string TahunAkademik { get; set; } = string.Empty;
    public string Semester { get; set; } = string.Empty;

    public string KonsentrasiId { get; set; } = string.Empty;
    public string KelasId { get; set; } = string.Empty;
    public string GrupId { get; set; } = string.Empty;
    public string JenisUjian { get; set; } = string.Empty;
    public string UserRole { get; set; } = string.Empty;
}

}

