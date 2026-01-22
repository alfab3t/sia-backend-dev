using astratech_apps_backend.DTOs.MasterIndustri;
using astratech_apps_backend.Models;

namespace astratech_apps_backend.Repositories.Interfaces
{
    public interface IMasterIndustriRepository
    {
        Task<(IEnumerable<MasterIndustri>, int totalData)> GetAllAsync(GetAllMasterIndustriRequest dto);
        Task<MasterIndustri?> GetByIdAsync(int id);
        Task<int> CreateAsync(CreateMasterIndustriRequest dto, string createdBy);
        Task<bool> UpdateAsync(UpdateMasterIndustriRequest dto, string updatedBy);
        Task<bool> SetStatusAsync(int id, string updatedBy);
    }
}