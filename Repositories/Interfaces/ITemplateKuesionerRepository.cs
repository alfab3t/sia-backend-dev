using astratech_apps_backend.DTOs.TemplateKuesioner;
using astratech_apps_backend.Models;

namespace astratech_apps_backend.Repositories.Interfaces
{
    public interface ITemplateKuesionerRepository
    {
        Task<(IEnumerable<TemplateKuesioner>, int totalData)> GetAllAsync(GetAllTemplateRequest dto);
        Task<TemplateKuesioner?> GetByIdAsync(int id);  
        Task<IEnumerable<Pertanyaan>> GetPertanyaanByTemplateIdAsync(int templateId);
        Task<int> CreateTemplateAsync(CreateTemplateRequest dto, string createdBy);
        Task<bool> UpdateTemplateAsync(UpdateTemplateRequest dto, string updatedBy);
        Task<bool> SetStatusTemplateAsync(int id, string newStatus, string updatedBy);
        Task<bool> SetFinalAsync(int id, string updatedBy);
        Task<int> CreatePertanyaanAsync(CreateUpdatePertanyaanRequest dto, string createdBy);
        Task<bool> UpdatePertanyaanAsync(CreateUpdatePertanyaanRequest dto, string updatedBy);
        Task<bool> SetStatusPertanyaanAsync(int id, string newStatus, string updatedBy);
        Task<bool> IsTemplateEmptyAsync(int id);
        Task<(bool Success, string Message)> ImportPertanyaanAsync(ImportPertanyaanRequest dto, string createdBy);
        Task<PreviewTemplateDto?> GetPreviewAsync(int id);

    }
}