using astratech_apps_backend.DTOs.MeninggalDunia;
using CutiAkademikDTOs = astratech_apps_backend.DTOs.CutiAkademik;
using astratech_apps_backend.Models;

namespace astratech_apps_backend.Repositories.Interfaces
{
    public interface IMeninggalDuniaRepository
    {

        Task<string> CreateAsync(CreateMeninggalDuniaRequest dto, string createdBy);
        Task<string> CreateWithMahasiswaDataAsync(string mhsId, string lampiranFileName, MahasiswaDetailDto mahasiswaData, string createdBy);
        Task<MahasiswaDetailDto?> GetMahasiswaDetailAsync(string mhsId);
        Task<string> FinalizeAsync(string draftId, string updatedBy);
        Task<IEnumerable<MahasiswaDropdownDto>> GetMahasiswaListAsync(string? search = null);
        Task<IEnumerable<ProgramStudiDropdownDto>> GetProgramStudiListAsync();
        Task<IEnumerable<MahasiswaDropdownSPDto>> GetMahasiswaDropdownSPAsync();
        Task<MahasiswaProdiDto?> GetMahasiswaProdiSPAsync(string mhsId);
        Task<(IEnumerable<MeninggalDuniaListDto> Data, int TotalData)>GetAllAsync(GetAllMeninggalDuniaRequest req);
        Task<MeninggalDuniaDetailResponse?> GetDetailAsync(string id);
        Task<MeninggalDuniaReportResponse?> GetReportAsync(string id);
        Task<MeninggalDunia?> GetByIdAsync(string id);
        Task<bool> UpdateAsync(string id, UpdateMeninggalDuniaRequest dto, string updatedBy);
        Task<bool> SoftDeleteAsync(string id, string updatedBy);
        Task<bool> UpdateSKAsync(string id, string sk, string spkb, string updatedBy);
        Task<bool> UploadSKAsync(string id, string sk, string spkb, string updatedBy);
        Task<(IEnumerable<RiwayatMeninggalDuniaListDto> Data, int TotalData)>   GetRiwayatAsync(GetRiwayatMeninggalDuniaRequest req);
        Task<IEnumerable<RiwayatMeninggalDuniaExcelResponse>> GetRiwayatExcelAsync(string sort,string konsentrasi);
        Task<bool> ApproveAsync(string id, ApproveMeninggalDuniaRequest dto);
        Task<bool> RejectAsync(string id, RejectMeninggalDuniaRequest dto);
        Task<string> DetectUserRoleAsync(string username);
        Task<IEnumerable<KonsentrasiDropdownDto>> GetKonsentrasiBySekprodAsync(string username);
        Task<IEnumerable<CutiAkademikDTOs.MahasiswaByKonsentrasiDto>> GetMahasiswaByKonsentrasiAsync(string username);
    }
}
