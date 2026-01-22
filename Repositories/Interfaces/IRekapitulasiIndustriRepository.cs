using astratech_apps_backend.DTOs.RekapitulsiIndustri;
using astratech_apps_backend.Models;

namespace astratech_apps_backend.Repositories.Interfaces
{
    public interface IRekapitulasiIndustriRepository
    {
        Task<GetAllRekapitulasiIndustriResponse> GetAllAsync(GetAllRekapitulasiIndustriRequest request);

        Task<IEnumerable<RekapitulasiIndustriPivotDto>> GetPivotAsync(GetAllRekapitulasiIndustriPivotRequest request);

        Task<IEnumerable<RekapitulasiIndustriMahasiswaDto>> GetDetailMahasiswaAsync(string namaIndustri, string tahunAjaran);

        Task<IEnumerable<string>> GetListTahunAjaranAsync();Task<string> GetActiveTahunAjaranAsync();
    }

}
