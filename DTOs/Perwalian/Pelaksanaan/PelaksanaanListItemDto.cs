namespace astratech_apps_backend.DTOs.Perwalian.Pelaksanaan;

public class PelaksanaanListItemDto
{
    public int IdPerwalian { get; set; }
    public string IdMahasiswa { get; set; } = "";
    public int IdDosen { get; set; }
    public string Subjek { get; set; } = "";
    public string Status { get; set; } = "";
    public List<PelaksanaanDetailListDto> Details { get; set; } = new();
}
