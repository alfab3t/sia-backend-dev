using astratech_apps_backend.DTOs.JenisSurat;
using astratech_apps_backend.Models;

namespace astratech_apps_backend.Repositories.Interfaces
{
    public interface IJenisSuratRepository
    {
        Task<(IEnumerable<JenisSurat>, int totalData)> GetAllAsync(GetAllJenisSuratRequest dto);
        Task<JenisSurat?> GetByIdAsync(int id);
        Task<bool> UpdateAsync(UpdateJenisSuratRequest dto, string updatedBy);
        Task<bool> SetStatusAsync(int id, string updatedBy);
    }
}
