namespace astratech_apps_backend.DTOs.Kamar_Dormitory
{
    public class AtributKamarRequest
    {
        public string NamaAtribut { get; set; } = string.Empty;
        public int Jumlah { get; set; }
        public bool KondisiBaik { get; set; }
        public string? Keterangan { get; set; }
    }

}
