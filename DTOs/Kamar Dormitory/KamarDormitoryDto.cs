namespace astratech_apps_backend.DTOs.Kamar_Dormitory
{
    public class KamarDormitoryDto
    {
        public int Id { get; set; }
        public long RowNumber { get; set; }
        public short Lantai { get; set; }
        public string KodeKamar { get; set; } = string.Empty;
        public string JenisKamar { get; set; } = string.Empty;
        public string Foto { get; set; } = string.Empty;
        public string? Keterangan { get; set; }
        public string Status { get; set; } = string.Empty;
        public int Penghuni { get; set; }
        public string Kondisi { get; set; }
        public List<AtributKamarDto> Atribut { get; set; } = new();
        public int JumlahPenghuniAktif { get; set; }
    }
}
