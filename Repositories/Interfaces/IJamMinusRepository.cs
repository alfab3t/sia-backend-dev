using astratech_apps_backend.DTOs.Jam_Minus;
using astratech_apps_backend.DTOs.JamMinusController;
using astratech_apps_backend.Models;

namespace astratech_apps_backend.Repositories.Interfaces
{
    public interface IJamMinusRepository
    {
        Task<(IEnumerable<JamMinus>, int totalData)> GetAllAsync(GetAllJamMinusRequest dto);
        Task<JamMinus?> GetByIdAsync(string id, string tahunAjaran, string semester);
        Task<int> CreateAsync(CreateJamMinus dto, string createdBy);
        Task<bool> UpdateAsync(UpdateJamMinusRequest dto, string updatedBy);
        Task<bool> DeleteAsync(DeleteJamMinusRequest deleteDto, string modifBy);
        Task<IEnumerable<JamMinus>> GetAllAsync(string id, string tahunAjaran, string semester);
        Task<List<GetListKonsentrasiDto>> GetListKonsentrasiAsync(string username, string roleId);
        Task<List<GetListKelasDto>> GetListKelasAsync(string IdProdi, string tahunAjaran);
        Task<List<MahasiswaKelasDto>> GetListMahasiswaAsync(string IdKelas);
        Task<TahunAkademikDto> GetActiveTahunAkademikAsync(DateTime dateToCheck);

    }
}
