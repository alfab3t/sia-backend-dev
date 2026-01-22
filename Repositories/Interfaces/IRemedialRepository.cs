using astratech_apps_backend.DTOs.Remedial;
using astratech_apps_backend.Models;

namespace astratech_apps_backend.Repositories.Interfaces
{
    public interface IRemedialRepository
    {
        Task<(IEnumerable<RemedialDto>, int totalData)> GetDataRemedialAsync(GetAllRemedialRequest dto, string username, string userRole);

        Task<IEnumerable<DropdownResponse>> GetListKonsentrasiAsync(string username, string userRole);
        Task<IEnumerable<DropdownResponse>> GetListTahunAjaranAsync();
        Task<IEnumerable<DropdownResponse>> GetListMataKuliahAsync(string konsentrasiId, string tahunAjaran, string semester, string username, string userRole);
        Task<IEnumerable<DropdownResponse>> GetListKelasAsync(string mataKuliahId, string username, string userRole);
        Task<string> GetDosenByKelasAsync(string mataKuliahId, string kelas);
        Task<(string TahunAjaran, string Semester)> GetActivePeriodeAsync();
        Task<(string TahunAjaran, string Semester)> GetActivePenilaianPeriodAsync();
        Task<(string TahunAjaran, string Semester)> GetActiveAcademicYearAsync(DateTime? tanggalReferensi = null);

        Task<bool> CanEditRemedialAsync(string tahunAjaran, string semester, string mataKuliahId, string kelas, string username, string userRole);
        Task<bool> UpdateNilaiRemedialAsync(UpdateRemedialRequest request, string username, string userRole);
    }
}