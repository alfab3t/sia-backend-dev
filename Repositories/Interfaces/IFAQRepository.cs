using astratech_apps_backend.DTOs.Bantuan.FAQ;
using astratech_apps_backend.Models;

namespace astratech_apps_backend.Repositories.Interfaces
{
    public interface IFAQRepository
    {
        Task<(IEnumerable<FAQ>, int totalData)> GetAllAsync(GetAllFAQRequest dto, string userRoleId, string username);
        Task<FAQ?> GetByIdAsync(int id, string userRoleId);
        Task<int> CreateAsync(CreateFAQRequest dto, string createdBy, string creatorRoleId);
        Task<bool> UpdateAsync(UpdateFAQRequest dto, string updatedBy, string currentRoleId);
        Task<bool> DeleteAsync(int id, string deletedBy);
        Task<IEnumerable<RoleOptionFAQ>> GetRolesAsync(); 
    }
}