namespace astratech_apps_backend.DTOs.PengubahanNilai
{
    public class PengubahanNilaiResponse
    {
        public List<PengubahanNilaiDto> Data { get; set; } = [];
        public int TotalRecords { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public string? ErrorMessage { get; set; } = string.Empty;
    }

    public class PengubahanNilaiDto
    {
        public int RowNum { get; set; }
        public int PermintaanPengubahanNilaiId { get; set; }
        public string Konsentrasi { get; set; } = string.Empty;
        public string TahunAjaran { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public string MataKuliah { get; set; } = string.Empty;
        public string Dosen { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string KonsentrasiId { get; set; } = string.Empty;
        public string KelasId { get; set; } = string.Empty;
        public bool OpenEdit { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public List<string>? Aksi { get; set; }
    }
}