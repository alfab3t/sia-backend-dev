using astratech_apps_backend.DTOs.PeriodeDaftarUlang;
using astratech_apps_backend.Models; 

namespace astratech_apps_backend.Repositories.Interfaces
{
    public interface IPeriodeDaftarUlangRepository
    {
        Task<(IEnumerable<PeriodeDaftarUlang>, int totalData)> GetAllAsync(GetAllPeriodeDaftarUlangRequest dto);
        Task<PeriodeDaftarUlang?> GetByIdAsync(int id);
        Task<bool> CreateAsync(CreatePeriodeDaftarUlangRequest dto, string createdBy);
        Task<bool> UpdateAsync(UpdatePeriodeDaftarUlangRequest dto, string updatedBy);
        Task<bool> CheckPeriodeDaftarUlangAsync(string tahunAjaran);
        Task<IEnumerable<TahunAjaranDto>> GetListTahunAjaranAsync();
    }
}
