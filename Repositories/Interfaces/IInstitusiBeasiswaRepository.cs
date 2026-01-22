using astratech_apps_backend.DTOs.InstitusiBeasiswa;
using astratech_apps_backend.Models;

namespace astratech_apps_backend.Repositories.Interfaces
{
    public interface IInstitusiBeasiswaRepository
    {
        Task<(IEnumerable<InstitusiBeasiswa>, int totalData)> GetAllAsync(GetAllInstitusiBeasiswaRequest dto);
        Task<(IEnumerable<InstitusiBeasiswa>, int totalData)> GetAllDropDownAsync(GetAllInstitusiBeasiswaDropdownRequest dto);
        Task<InstitusiBeasiswa?> GetByIdAsync(short id);
        Task<int> CreateAsync(CreateInstitusiBeasiswaRequest dto, string createdBy);
        Task<bool> UpdateAsync(UpdateInstitusiBeasiswaRequest dto, string updatedBy);
        Task<bool> SetStatusAsync(short id, string updatedBy);
    }
}
