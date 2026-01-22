using astratech_apps_backend.DTOs.LihatAlokasiPembimbing;
using astratech_apps_backend.Models.LihatAlokasiPembimbing;
using System.Data;

namespace astratech_apps_backend.Repositories.Interfaces
{
    public interface ILihatAlokasiPembimbingRepository
    {
        Task<(IEnumerable<LihatAlokasiPembimbing>, int totalData)> GetAllAsync(GetAllLihatAlokasiPembimbingRequest dto);
        Task<DataTable> ExportTA(string tahunAkademik, string konsentrasiId);
        Task<IEnumerable<Konsentrasi>> GetListKonsentrasiAsync(string username, string roleId);
        Task<IEnumerable<KonsentrasiByNPK>> GetListKonsentrasiByNPKAsync(GetListKonsentrasiByNPKRequest request);
        Task<IEnumerable<KonsentrasiByDosen>> GetListKonsentrasiByDosenAsync(string username);
        Task<TahunAkademikAktif?> GetActiveTahunAkademikAsync();
    }
}
