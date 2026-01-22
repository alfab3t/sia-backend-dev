
namespace astratech_apps_backend.DTOs.TatapMuka
{
    public class TatapMukaListItemDto
    {
        public string TatapMukaId { get; set; } = string.Empty;
        public string KonsentrasiSingkatan { get; set; } = string.Empty;
        public string TahunAjaran { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public string MataKuliah { get; set; } = string.Empty;
        public string DosenPengampu { get; set; } = string.Empty;
        public string JadwalTatapMuka { get; set; } = string.Empty;
        public string Ruangan { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Tanggal { get; set; } = string.Empty;
        public string KonsentrasiId { get; set; } = string.Empty;
        public string Kelas { get; set; } = string.Empty;
        public string RuanganId { get; set; } = string.Empty;
    }

    public class GetDataTatapMukaParams
    {
        public string Mode { get; set; } = string.Empty;
        public string SearchText { get; set; } = string.Empty;
        public string OrderBy { get; set; } = string.Empty;
        public string KonsentrasiId { get; set; } = string.Empty;
        public string TahunAjaran { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
    }

    public class MataKuliahResponseDto
    {
        public string MataKuliahId { get; set; } = string.Empty;
        public string MataKuliah { get; set; } = string.Empty;
    }

    public class MataKuliahRequestDto
    {
        public string KonsentrasiId { get; set; } = string.Empty;
        public string TahunAjaran { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
    }

    public class NamaDosenDto
    {
        public string Dosen1 { get; set; } = string.Empty;
        public string Dosen2 { get; set; } = string.Empty;
        public string Dosen3 { get; set; } = string.Empty;
    }

    public class NamaDosenRequestDto
    {
        public string MataKuliahId { get; set; } = string.Empty;
        public string SectionId { get; set; } = string.Empty;
        public string TahunAjaran { get; set; } = string.Empty;
        public string KelasId { get; set; } = string.Empty;
    }

    public class IdDosenDto
    {
        public string DosenId { get; set; } = string.Empty;
    }

}

    



