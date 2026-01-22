using astratech_apps_backend.DTOs.JenisBeasiswa;
using astratech_apps_backend.Models;

namespace astratech_apps_backend.Repositories.Interfaces
{
    public interface IJenisBeasiswaRepository
    {
        Task<(IEnumerable<JenisBeasiswa>, int totalData)> GetAllAsync(GetAllJenisBeasiswaRequest dto);
        Task<(IEnumerable<JenisBeasiswa>, int totalData)> GetAllDropdownAsync(GetAllJenisBeasiswaDropdownRequest dto);
        Task<JenisBeasiswa?> GetByIdAsync(short id);
        Task<int> CreateAsync(CreateJenisBeasiswaRequest dto, string createdBy);
        Task<bool> UpdateAsync(UpdateJenisBeasiswaRequest dto, string updatedBy);
        Task<bool> SetStatusAsync(short id, string updatedBy);
    }
}
