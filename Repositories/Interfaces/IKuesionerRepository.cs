using astratech_apps_backend.DTOs.Kuesioner;

namespace astratech_apps_backend.Repositories.Interfaces
{
    public interface IKuesionerRepository
    {
        Task<(IEnumerable<KuesionerDto>, int totalData)> GetAllAsync(GetAllKuesionerRequest dto, string username, string roleId);
        Task<(bool Success, string Message)> GenerateKuesionerAsync(GenerateKuesionerRequest dto, string createdBy);
        Task<KuesionerResultDto?> GetResultForAdminAsync(int kuesionerId);
        Task<KuesionerDetailDto?> GetDetailForUserAsync(int kuesionerId, string username);
        Task<(bool Success, string Message)> SubmitJawabanAsync(SubmitKuesionerRequest dto, string username);
        Task<List<Dictionary<string, object>>> GetExportDataAsync(GetExportDataRequest request);
    }
}