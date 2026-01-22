using astratech_apps_backend.DTOs.TagihanCalonMahasiswa;

namespace astratech_apps_backend.Repositories.Interfaces
{
    public interface ITagihanCalonMahasiswaRepository
    {
        Task<GetAllTagihanCalonMahasiswaResponse> GetAllAsync(GetAllTagihanCalonMahasiswaRequest dto);
        Task<IEnumerable<ExportTagihanCalonMahasiswaResponse>> GetForExportAsync(GetAllTagihanCalonMahasiswaRequest dto);
        Task<IEnumerable<(string Id, string Nama)>> GetListProdiAsync();
        Task<IEnumerable<string>> GetListAngkatanAsync();
    }
}
