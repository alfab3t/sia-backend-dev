using astratech_apps_backend.DTOs.Section;
using astratech_apps_backend.Models;

namespace astratech_apps_backend.Repositories.Interfaces
{
    public interface ISectionRepository
    {
        Task<(IEnumerable<Section>, int totalData)> GetAllAsync(GetAllSectionRequest dto);
        Task<int> CreateAsync(CreateSectionRequest dto, string createdBy);
        Task<bool> UpdateAsync(UpdateSectionRequest dto, string updatedBy);
        Task<bool> SetStatusAsync(int id, string updatedBy);
        Task<Section?> GetByIdAsync(int id);
    }
}
