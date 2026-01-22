using astratech_apps_backend.DTOs.SkalaPenilaian;
using astratech_apps_backend.Models;

namespace astratech_apps_backend.Repositories.Interfaces
{
    public interface ISkalaPenilaianRepository
    {
        Task<(IEnumerable<SkalaPenilaian>, int totalData)> GetAllAsync(GetAllSkalaPenilaianRequest dto);
        Task<SkalaPenilaian?> GetByIdAsync(int id);
        Task<int> CreateAsync(CreateSkalaPenilaianRequest dto, string createdBy);
        Task<bool> UpdateAsync(UpdateSkalaPenilaianRequest dto, string updatedBy);
        Task<bool> SetStatusAsync(int id, string newStatus, string updatedBy);
    }
}   