using astratech_apps_backend.DTOs.BuktiPelaksanaanPerkuliahan;
using astratech_apps_backend.Models;

namespace astratech_apps_backend.Repositories.Interfaces
{
    public interface IBuktiPelaksanaanPerkuliahanRepository
    {
        Task<(IEnumerable<BuktiPelaksanaanPerkuliahan>, int totalData)> GetAllAsync(GetAllBuktiPelaksanaanPerkuliahanRequest dto, string username, string role);
        Task<IEnumerable<KonsentrasiDto>> GetListKonsentrasiAsync(string username, string role);
        Task<TahunAkademikDto?> GetTahunAkademikAktifAsync();
        Task<IEnumerable<SemesterDto>> GetListSemesterAsync();
        Task<BuktiPelaksanaanPerkuliahanDetail?> GetDetailAsync(string rpsId, string kelasId);
    }
}