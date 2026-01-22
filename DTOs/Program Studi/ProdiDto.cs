namespace astratech_apps_backend.DTOs.Program_Studi
{
    public class ProdiDto
    {
        public short Id { get; set; }
        public string NamaProdi { get; set; } = string.Empty;
        public string Singkatan { get; set; } = string.Empty;
        public string NamaKaprodi { get; set; } = string.Empty;
        public string Jenjang { get; set; } = string.Empty;
        public short MasaStudi { get; set; }
        public string ProNomor { get; set; } = string.Empty;
        public string ProNama { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
