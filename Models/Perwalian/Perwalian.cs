namespace astratech_apps_backend.Models.Perwalian;

public class Perwalian
{
    public int IdPerwalian { get; set; }
    public string? IdMahasiswa { get; set; }
    public int? IdDosen { get; set; }
    public string? Subjek { get; set; }
    public string? Status { get; set; }
    public ICollection<PerwalianDetail>? Details { get; set; }
}
