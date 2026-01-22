using astratech_apps_backend.DTOs.WaliMahasiswa;
using astratech_apps_backend.Models;

namespace astratech_apps_backend.Repositories.Interfaces
{
    public interface IWaliMhsRepository
    {
    Task<(IEnumerable<WaliMahasiswa> list, int totalData)> GetAllAsync(GetAllWaliMhsRequest dto);

    Task<WaliMahasiswa?> GetDetailAsync(
        int dosenId,
        string angkatan,
        int prodiId,
        string status
    );

    Task<int> CreateAsync(CreateWaliMhsRequest dto, string createdBy);

    Task<IEnumerable<MahasiswaItem>> GetMahasiswaAvailableAsync(string angkatan, int prodiId);

    Task<bool> UpdateAsync(
        int dosenId,
        string angkatan,
        int prodiId,
        string status,
        UpdateWaliMhsRequest dto,
        string updatedBy
    );

    Task<bool> DeleteAsync(
        int dosenId,
        string angkatan,
        int prodiId,
        string status
    );

    Task<bool> SendApprovalAsync(
        int dosenId,
        string angkatan,
        int prodiId,
        string status,
        string updatedBy
    );

    Task<bool> ApproveAsync(
        int dosenId,
        string angkatan,
        int prodiId,
        string status,
        string updatedBy
    );

    Task<bool> RejectAsync(
        int dosenId,
        string angkatan,
        int prodiId,
        string status,
        string reason,
        string updatedBy
    );

    Task<bool> SetStatusAsync(
        int dosenId,
        string angkatan,
        int prodiId,
        string status,
        string updatedBy
    );

    Task<List<string>> GetListAngkatanAsync();
    Task<List<Dictionary<string, object>>> GetListProdiAsync();
    Task<List<Dictionary<string, object>>> GetListDosenAsync();
    }        
}

