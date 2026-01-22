namespace astratech_apps_backend.Models.Perwalian;

using System.Text.Json.Serialization;

public class PerwalianDetail
{
    public int IdPerwalianDetail { get; set; }
    public int IdPerwalian { get; set; }
    public string? Pesan { get; set; }
    public string? Tipe { get; set; }
    public string? Berkas { get; set; }
    public string? Status { get; set; }

    public string? CreatedBy { get; set; }


    [JsonIgnore]
    public Perwalian? Perwalian { get; set; }
}

