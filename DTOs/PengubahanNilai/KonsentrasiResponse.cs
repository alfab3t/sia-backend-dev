namespace astratech_apps_backend.DTOs.PengubahanNilai;

public class KonsentrasiResponse : BaseResponse
{
    public List<KonsentrasiDto> Data { get; set; } = new();
}

public class KonsentrasiDto
{
    public string KonsentrasiId { get; set; } = "";
    public string KonNama { get; set; } = "";
}