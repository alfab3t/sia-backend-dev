
namespace astratech_apps_backend.Models
{
    public class Prodi
    {
        public short Id { get; set; }
        public long RowNumber { get; set; } = 0;
        public short JurusanId { get; set; }
        public string NamaProdi { get; set; } = string.Empty;
        public string Singkatan { get; set; } = string.Empty;
        public string NamaKaprodi { get; set; } = string.Empty;
        public string NpkKaprodi { get; set; } = string.Empty;
        public string Jenjang { get; set; } = string.Empty;
        public short MasaStudi { get; set; }
        public DateTime? TanggalBerdiri { get; set; }
        public string NoSkDikti { get; set; } = string.Empty;
        public DateTime? SkDiktiFrom { get; set; }
        public DateTime? SkDiktiUntil { get; set; }
        public string NoBanpt { get; set; } = string.Empty;
        public DateTime? BanptFrom { get; set; }
        public DateTime? BanptUntil { get; set; }
        public string NomorProdi { get; set; } = string.Empty;
        public string Akreditasi { get; set; } = string.Empty;
        public string Deskripsi { get; set; } = string.Empty;
        public string Visi { get; set; } = string.Empty;
        public string Misi { get; set; } = string.Empty;
        public string ProfilLulusan { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;


        public List<string> SekprodiList { get; set; } = new();
        public List<string> NpkSekprodiList { get; set; } = new();
        public List<string> SingkatanMkList { get; set; } = new();
        public string Jurusan { get; internal set; } = string.Empty;
    }
}
