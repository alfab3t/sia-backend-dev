using astratech_apps_backend.Models;

namespace astratech_apps_backend.DTOs.WaliMahasiswa;
public class WaliMahasiswaDto
{
    public int Id { get; set; }
    public int DosenId { get; set; }
    public short Prodi { get; set; }
    public int prodiId { get; set; }

    public string Angkatan { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;

    public string WaliMahasiswa { get; set; } = string.Empty;
    public string ProgramStudi { get; set; } = string.Empty;

    public string NamaDosen { get; set; } = string.Empty;
    public string NamaProdi { get; set; } = string.Empty;
    public List<MahasiswaItem> DaftarMahasiswa { get; set; } = new();

}
