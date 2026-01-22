using astratech_apps_backend.DTOs.JamPlus;
using astratech_apps_backend.DTOs.JamPlusController;
using astratech_apps_backend.Models;

namespace astratech_apps_backend.Repositories.Interfaces
{
    public interface IJamPlusRepository
    {
        Task<(IEnumerable<JamPlus>, int totalData)> GetAllAsync(GetAllJamPlusRequest dto);
        Task<JamPlus?> GetByIdAsync(string id, string tahunAjaran, string semester);
        Task<int> CreateAsync(CreateJamplus dto, string createdBy);
        Task<bool> UpdateAsync(UpdateJamPlusRequest dto, string updatedBy);
        Task<IEnumerable<JamPlus>> GetAllAsync(string id, string tahunAjaran, string semester);
        Task<List<GetListKonsentrasiDto>> GetListKonsentrasiAsync(string username, string roleId);
        Task<List<GetListKelasDto>> GetListKelasAsync(string IdProdi, string tahunAjaran);
        Task<List<MahasiswaKelasDto>> GetListMahasiswaAsync(string IdKelas);
        Task<bool> DeleteJamPlusAsync(DeleteJamPlus deleteDto, string modifBy);
        Task<TahunAkademikDto> GetActiveTahunAkademikAsync(DateTime dateToCheck);



    }
}
