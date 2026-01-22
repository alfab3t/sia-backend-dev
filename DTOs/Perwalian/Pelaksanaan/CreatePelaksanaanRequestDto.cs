namespace astratech_apps_backend.DTOs.Perwalian.Pelaksanaan;

public class CreatePelaksanaanRequestDto
{
    public string IdMahasiswa { get; set; } = "";
    public int IdDosen { get; set; }
    public string Subjek { get; set; } = "";
}
