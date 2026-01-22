using astratech_apps_backend.DTOs.JenisKuesioner;
using astratech_apps_backend.Models;

namespace astratech_apps_backend.Repositories.Interfaces
{
    public interface IJenisKuesionerRepository
    {
        Task<(IEnumerable<JenisKuesioner>, int totalData)> GetAllAsync(GetAllJenisKuesionerRequest dto);
        Task<JenisKuesioner?> GetByIdAsync(int id);
        Task<int> CreateAsync(CreateJenisKuesionerRequest dto, string createdBy);
        Task<bool> UpdateAsync(UpdateJenisKuesionerRequest dto, string updatedBy);
        Task<bool> SetStatusAsync(int id, string newStatus, string updatedBy);
    }
}