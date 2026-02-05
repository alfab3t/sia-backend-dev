using astratech_apps_backend.DTOs.Wisuda;
using astratech_apps_backend.Models;

namespace astratech_apps_backend.Repositories.Interfaces
{
    public interface IWisudaRepository
    {
        Task<(IEnumerable<Wisuda>, int totalData)> GetAllAsync(GetAllWisudaRequest dto);

        Task<(IEnumerable<Wisuda>, int totalData)> GetAllTandaTerimaAsync(
            GetAllTandaTerimaRequest dto
        );

        Task<IEnumerable<Wisuda>> GetListLulusanAsync(int tahunLulus);

        Task<IEnumerable<Wisuda>> GetListKonsentrasiAsync(
            string username,
            string roleId
        );

        Task<bool> CreateTandaTerimaIjazahAsync(
            string createdBy,CreateTandaTerimaIjazahRequest dto
        );
    }
}
