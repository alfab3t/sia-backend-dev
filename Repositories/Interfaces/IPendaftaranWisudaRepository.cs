using astratech_apps_backend.DTOs.PendaftaranWisuda;
using astratech_apps_backend.Models;

namespace astratech_apps_backend.Repositories.Interfaces
{
    public interface IPendaftaranWisudaRepository
    {
        
        Task<string> CreatePendaftaranWisudaAsync(
            CreatePendaftaranWisudaRequest dto,
            string createdBy
        );

        Task<(IEnumerable<PendaftaranWisuda>, int totalData)> GetAllAsync(
            GetAllPendaftaranWisudaRequest dto
        );

        Task<(IEnumerable<PendaftaranWisuda?>, int totalData)> GetByIdAsync(int id);

        Task<bool> UpdateAsync(
            UpdatePendaftaranWisudaRequest dto,
            string updatedBy
        );

        Task<(List<DetailDataWisuda>, int totalData)> GetDetailPesertaAsync(
            int wisudaHeadId,
            GetDetailPesertaWisudaRequest req
        );

        Task<List<ExportPesertaWisudaDto>> ExportPesertaWisudaAsync(
            int wisudaHeadId
        );

        
        Task<int> CreateWisudaMahasiswaAsync(
            CreateWisudaRequest dto,
            string createdBy
        );

        Task<DetailDataWisuda?> GetByMahasiswaAsync(string mhsId);

        Task<GetWisudaDetailResponse?> GetDetailWisudaAsync(string wisId);

        Task ApproveAsync(
            string wisudaId,
            string userLogin,
            string role,
            DateTime? tanggalBayar
        );

        
        Task<bool> CheckKuotaTambahanAsync(
            string tahunAjaran,
            short jumlahUndangan,
            string mhsId
        );

        Task<string> GetKonsentrasiByNimAsync(string mhsId);

        Task<(string status, string fileName)> GetQrInfoAsync(
            string mhsId,
            string guid
        );

        Task<List<string>> GetListTahunAkademikAsync();
    }
}
