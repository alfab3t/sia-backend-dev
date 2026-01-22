using astratech_apps_backend.DTOs.PengubahanNilai;
using System.ComponentModel;

namespace astratech_apps_backend.Repositories.Interfaces
{
    public interface IPengubahanNilaiRepository
    {
       
        Task<PengubahanNilaiResponse?> GetDataPengubahanNilaiAsync(PengubahanNilaiRequestDto dto, string username, string roleId);
        Task<BaseResponse?> CreatePengubahanNilaiAsync(CreatePengubahanNilaiDto dto, string username);
        Task<DetailPengubahanNilaiResponse?> GetDetailPengubahanNilaiAsync(int id);
        Task<BaseResponse?> SendPengubahanNilaiAsync(int id, SendPengubahanNilaiDto dto, string username, string roleId);
        Task<BaseResponse?> ApprovePengubahanNilaiAsync(int id, string username, string roleId);
        Task<BaseResponse?> RejectPengubahanNilaiAsync(int id, RejectPengubahanNilaiDto dto, string username, string roleId);
        Task<BaseResponse?> DeletePengubahanNilaiAsync(int id, string username, string roleId);
        Task<KonsentrasiResponse?> GetListKonsentrasiAsync(string username, string roleId, bool filterByUser = false);
        Task<TahunAjaranResponse?> GetListTahunAjaranAsync();
        Task<MataKuliahResponse?> GetListMataKuliahAsync(string tahunAjaran, string semester, string roleId, string username);
        Task<KelasResponse?> GetListKelasAsync(string tahunAjaran, string semester, string mataKuliahId, string roleId, string username);
        Task<TahunAkademikActiveResponse?> GetActiveTahunAkademikAsync();
        Task<DosenByPengubahanResponse?> GetDosenByPengubahanNilaiAsync(int ppnId);
        Task<ProdiByMatkulResponse?> GetProdiByMataKuliahAsync(string mkuId);
    }
}