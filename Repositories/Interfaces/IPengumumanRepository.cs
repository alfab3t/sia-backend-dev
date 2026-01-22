using astratech_apps_backend.DTOs.Pengumuman;
using astratech_apps_backend.Models;

namespace astratech_apps_backend.Repositories.Interfaces
{
    public interface IPengumumanRepository
    {
        Task<int> CreatePengumumanRepo(CreatePengumumanRequest dto, string IdRole, string DibuatOleh);
        Task<(IEnumerable<Pengumuman>, int totalData)>GetDataPengumumanRepo(GetDataPengumumanRequest dto, string DibuatOleh, string IdAplikasi);

        Task<bool> UpdatePegumumanRepo(UpdatePengumumanRequest dto, string DiubahOleh);
        Task<bool> SetStatusHapusPengumuman(int IdPengumuman, string DiubahOleh);
        Task<(IEnumerable<Pengumuman>, int totalData)> GetAllDataPengumumanRepo(GetDataPengumumanRequest dto);
        Task<GetDetailPengumumanResponse?> GetDetailPengumumanRepo(int IdPengumuman);

        Task<bool> HidePengumumanRepo(int IdPengumuman,string DiubahOleh);
        Task<bool> ShowPengumumanRepo(int IdPengumuman,string DiubahOleh);

        Task<IEnumerable<string>>GetListRoleRepo(string IdPengguna,string IdRole);
        Task<IEnumerable<Aplikasi>> GetListAplikasiRepo();
        Task<IEnumerable<Aplikasi>> GetListRoleByAplikasiRepo(string IdAplikasi);
    }
}
