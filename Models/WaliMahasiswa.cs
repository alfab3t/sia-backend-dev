namespace astratech_apps_backend.Models;

public class WaliMahasiswa
{
    public int Id { get; set; }
    public int DosenId { get; set; }
    public int IdMahasiswa { get; set; }
    public short Prodi { get; set; }
    public int prodiId { get; set; }
    public string Angkatan { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;
    public string AlasanTolak { get; set; } = string.Empty;

    public string NamaDosen { get; set; } = string.Empty;
    public string NamaProdi { get; set; } = string.Empty;

    public List<MahasiswaItem> DaftarMahasiswa { get; set; } = new();
}
    public class MahasiswaItem
{
    public string NIM { get; set; } = string.Empty;
    public string Nama { get; set; } = string.Empty;
    public string Kelas { get; set; } = string.Empty;
}
