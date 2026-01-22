using astratech_apps_backend.DTOs.AlokasiKurikulum;
using astratech_apps_backend.Models;

namespace astratech_apps_backend.Repositories.Interfaces
{
    public interface IAlokasiKurikulumRepository
    {
        Task<(IEnumerable<AlokasiKurikulumMahasiswa>, int totalData)> GetMahasiswaKurikulumListAsync(GetAllKurikulumMahasiswaRequest request);
        Task<IEnumerable<AlokasiKurikulumMahasiswa>> GetProdiListAsync();
        Task<IEnumerable<AlokasiKurikulumMahasiswa>> GetAngkatanListAsync();
        Task<IEnumerable<AlokasiKurikulumMahasiswa>> GetKurikulumListAsync(string konid);
        Task<bool> UpdateAlokasiKurikulumAsync(UpdateAlokasiKurikulumRequest dto);
    }
}
