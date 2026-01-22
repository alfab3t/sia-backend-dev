using astratech_apps_backend.DTOs.JamMinusPlus;
using astratech_apps_backend.Models;

namespace astratech_apps_backend.Repositories.Interfaces
{
    public interface IJamMinusPlusRepository
    {
        Task<(IEnumerable<JamMinusPlus> Data, int Total)>
        GetAllAsync(GetAllJamMinusPlusRequest dto);
        Task<List<JamMinusPlusPeriodeDto>> GetDetailPeriodeAsync(string mhsId);
        Task<List<KeyValuePair<string, string>>> GetListKonsentrasiAsync();
    }
}
