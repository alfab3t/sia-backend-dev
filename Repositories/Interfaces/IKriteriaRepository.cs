using astratech_apps_backend.DTOs.HasilStudiKriteria;
using astratech_apps_backend.Models.HasilStudiKriteria;

namespace astratech_apps_backend.Repositories.Interfaces
{
    public interface IKriteriaRepository
    {
        Task<(IEnumerable<Kriteria>, int totalData)> GetAllAsync(GetAllKriteriaRequest dto, string userId, string userRole);
        Task<Kriteria?> GetByIdAsync(string id);
        Task<string> CreateAsync(CreateKriteriaRequest dto, string userId, string userRole);
        Task<bool> UpdateAsync(UpdateKriteriaRequest dto, string userId);
        Task<bool> DeleteAsync(string id, string userId);
        Task<bool> KirimAsync(string id, string userId);
        Task<bool> ApproveAsync(string id, string userId);
        Task<bool> RejectAsync(ApproveRejectRequest dto, string userId);
        Task<List<LookupDto>> GetListKonsentrasiAsync();
        Task<List<LookupDto>> GetListTahunAjaranAsync();
        Task<List<LookupDto>> GetListMataKuliahAsync(string userId, string userRole, string tahunAjaran, string semester);
        Task<bool> CheckTotalPersentaseAsync(string kriteriaId);
        Task<string?> GetActivePeriodeByTanggalAsync();
        Task<string?> GetActiveTahunAkademikByTanggalAsync();
        Task<TemplateKriteriaResponse?> GetTemplateKriteriaAsync(TemplateKriteriaRequest dto, string userRole, string username);
    }
}