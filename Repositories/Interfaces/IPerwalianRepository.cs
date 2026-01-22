using astratech_apps_backend.DTOs.Perwalian.Pelaksanaan;
using astratech_apps_backend.Models.Perwalian;
using DTOs.Perwalian.Pelaksanaan;

namespace astratech_apps_backend.Repositories.Interfaces;

public interface IPerwalianRepository
{
    Task<(IEnumerable<Perwalian> data, int total)> GetAll(GetAllPelaksanaanRequestDto req);
    Task<Perwalian?> GetById(int id);

    Task<Perwalian> Create(Perwalian entity);
    Task<bool> Update(int id, UpdatePelaksanaanRequestDto dto);
    Task<bool> Delete(int id);

    Task<IEnumerable<PerwalianDetail>> GetDetails(int pwaId);
    Task<PerwalianDetail> AddDetail(PerwalianDetail detail);

    Task<List<Dictionary<string, object>>> GetDropdownMahasiswa(string username);
    Task<List<Dictionary<string, object>>> GetDropdownDosen(string username);
    Task<List<Dictionary<string, object>>> GetListProdiAsync();
    Task<List<Dictionary<string, object>>> GetMahasiswaByProdi(int idkonsentrasi);
    Task<List<Dictionary<string, object>>> GetDropdownDosenByProdi(string prodi);
    Task<List<string>> GetDropdownAngkatanByProdi(string prodi);

}
