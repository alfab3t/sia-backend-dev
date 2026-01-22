namespace astratech_apps_backend.Models
{
        public class Dormitory
        {
            public int Id { get; set; }
            public long RowNumber { get; set; }
            public short Lantai { get; set; }
            public string KodeKamar { get; set; }
            public string JenisKamar { get; set; }
            public string Foto { get; set; }
            public string? Keterangan { get; set; }
            public string Status { get; set; }
        }
}
