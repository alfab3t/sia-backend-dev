namespace astratech_apps_backend.DTOs.PendaftaranWisuda
{
    public class CreateWisudaRequest
{
    public string NIM { get; set; } = string.Empty;
    public string NamaMahasiswa { get; set; } = string.Empty;

    public string TempatLahir { get; set; } = string.Empty;
    public DateTime TanggalLahir { get; set; }

    public string NomorHandphone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public string FotoWisuda { get; set; } = string.Empty;
    public short JumlahUndangan { get; set; }

    public string BiayaWisuda { get; set; } = string.Empty;
    public string VirtualAccount { get; set; } = string.Empty;
    public string FormatNoWisuda { get; set; } = string.Empty;

    public string Nik { get; set; } = string.Empty;
    public string Alamat { get; set; } = string.Empty;

    public int IdPeriodeWisuda { get; set; }
    }
}

