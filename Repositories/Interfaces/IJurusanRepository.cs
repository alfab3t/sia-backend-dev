
using astratech_apps_backend.DTOs.Jurusan;
using astratech_apps_backend.Models;

namespace astratech_apps_backend.Repositories.Interfaces
{
    public interface IJurusanRepository
    {
        Task<(IEnumerable<Jurusan>, int totalData)> GetAllAsync(GetAllJurusanRequest dto);
        Task<Jurusan?> GetByIdAsync(short id);
        Task<IEnumerable<Jurusan>> GetListJurusanAsync();
        Task<int> CreateAsync(CreateJurusanRequest dto, string createdBy);
        Task<bool> UpdateAsync(UpdateJurusanRequest dto, string updatedBy);
        Task<bool> SetStatusAsync(short id, string updatedBy);
        Task<List<KaryawanDto>> GetListKaryawanAsync();
    }
}
