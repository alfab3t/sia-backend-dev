namespace astratech_apps_backend.Models
{
    public class PengubahanNilai
    {
        public int PermintaanPengubahanNilaiId { get; set; }
        public long RowNumber { get; set; } = 0;
        public string KonsentrasiId { get; set; } = string.Empty;
        public string KonsentrasiNama { get; set; } = string.Empty;
        public string KonsentrasiSingkatan { get; set; } = string.Empty;
        public string TahunAjaran { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public string MataKuliahId { get; set; } = string.Empty;
        public string MataKuliahNama { get; set; } = string.Empty;
        public string KelasId { get; set; } = string.Empty;
        public string DosenNama { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string AlasanPengajuan { get; set; } = string.Empty;
        public string AlasanTolak { get; set; } = string.Empty;
        public bool OpenEdit { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public int PengubahanNilaiId { get; internal set; }
    }

    public class Konsentrasi
    {
        public string Id { get; set; } = string.Empty;
        public string Nama { get; set; } = string.Empty;
    }

    public class MataKuliah
    {
        public string Id { get; set; } = string.Empty;
        public string Nama { get; set; } = string.Empty;
    }
}