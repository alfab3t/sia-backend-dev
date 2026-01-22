namespace astratech_apps_backend.DTOs.Kamar_Dormitory
{
    public class AtributKamarDto
    {
        public int Id { get; set; }
        public string NamaAtribut { get; set; } = "";
        public int Jumlah { get; set; }
        public bool KondisiBaik { get; set; }
        public string Keterangan { get; set; } = "";
    }

}
