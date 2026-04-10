using astratech_apps_backend.DTOs.CutiAkademik;

namespace astratech_apps_backend.Repositories.Interfaces
{
    public interface ICutiAkademikRepository
    {
        Task<string?> CreateDraftAsync(CreateDraftCutiAkademikRequest dto);
        Task<string?> GenerateIdAsync(GenerateIdFinalCutiAkademikRequest dto);
        Task<IEnumerable<CutiAkademikListResponse>> GetAllAsync(string mhsId, string status, string userId, string role, string search = "");
        Task<CutiAkademikDetailResponse?> GetDetailAsync(string id);
        Task<bool> UpdateAsync(string id, UpdateCutiAkademikRequest dto);
        Task<bool> DeleteAsync(string id, string modifiedBy);
        Task<string?> CreateDraftByProdiAsync(CreateDraftCutiAkademikByProdiRequest dto);
        Task<string?> GenerateIdByProdiAsync(GenerateIdFinalCutiAkademikByProdiRequest dto);
        Task<IEnumerable<CutiAkademikListResponse>> GetRiwayatAsync(string userId, string status, string search);
        Task<IEnumerable<CutiAkademikRiwayatExcelResponse>> GetRiwayatExcelAsync(string userId);
        Task<bool> ApproveCutiAsync(ApproveCutiAkademikRequest dto);
        Task<bool> ApproveProdiCutiAsync(ApproveCutiAkademikByProdiRequest dto);
        Task<bool> RejectCutiAsync(RejectCutiAkademikRequest dto);
        Task<bool> UploadSKAsync(UploadSKCutiAkademikRequest dto);
        
        // New methods for additional functionality
        Task<MahasiswaDetailDto?> GetDetailMahasiswaAsync(string mahasiswaId);
        Task<BebasTanggunganDto?> CheckBebasTanggunganAsync(string userId);
        Task<ProfilMahasiswaDto?> GetProfilMahasiswaAsync(string nim);
        Task<IEnumerable<KonsentrasiDropdownDto>> GetKonsentrasiBySekprodAsync(string username);
        Task<MahasiswaByNimDto?> GetMahasiswaByNimAsync(string nim);
        Task<IEnumerable<MahasiswaByKonsentrasiDto>> GetMahasiswaByKonsentrasiAsync(string username);
    }
}