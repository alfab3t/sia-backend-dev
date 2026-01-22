using astratech_apps_backend.DTOs.Program_Studi;
using astratech_apps_backend.Models;

namespace astratech_apps_backend.Repositories.Interfaces
{
    public interface IProdiRepository 
    {
        Task<(IEnumerable<Prodi>, int totalData)> GetAllAsync(GetAllProdiRequest dto);
        Task<Prodi?> GetByIdAsync(short id);
        Task<int> CreateAsync(CreateProdiRequest dto, string createdBy);
        Task<bool> UpdateAsync(UpdateProdiRequest dto, string updatedBy);
        Task<bool> SetStatusAsync(short id, string updatedBy);
        Task<bool> CheckNPKAsync(string npk);
        Task<ProdiDto> CheckNomerProdiAsync(string nomorProdi);

        Task<List<KaryawanDto>> GetListKaryawanAsync();

    }
}
