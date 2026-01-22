using Microsoft.AspNetCore.Http;

namespace astratech_apps_backend.DTOs.Perwalian.Pelaksanaan;

public class CreatePelaksanaanDetailRequestDto
{
    public int IdPerwalian { get; set; }
    public string Pesan { get; set; } = "";
    public string? Berkas { get; set; }
    public string Tipe { get; set; } = "TEXT";
    public IFormFile? File { get; set; }
}
