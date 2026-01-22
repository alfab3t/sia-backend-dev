namespace astratech_apps_backend.DTOs.Perwalian.Pelaksanaan;

public class GetAllPelaksanaanResponseDto
{
    public IEnumerable<PelaksanaanListItemDto> Data { get; set; } = new List<PelaksanaanListItemDto>();
    public int Total { get; set; }
}
