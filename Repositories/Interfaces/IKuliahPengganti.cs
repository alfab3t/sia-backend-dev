using astratech_apps_backend.DTOs.KuliahPengganti;

namespace astratech_apps_backend.Repositories.Interfaces
{
    public interface IKuliahPenggantiRepository
    {
        Task<bool> CreatePengajuanKPAsync(CreatePengajuanKPDto dto);
        Task<bool> EditPengajuanKPAsync(EditPengajuanKPDto dto);
        Task<bool> ApprovePengajuanKPAsync(ApprovePengajuanKPDto dto);
        Task<bool> SentPengajuanKPAsync(SentPengajuanKPDto dto);
        Task DeletePengajuanKPAsync(string KuliahPenggantiId, string ModifiedBy);
        Task<bool> RejectPengajuanKPAsync(RejectPengajuanKPDto dto);
        Task<bool> CancelPengajuanKPAsync(CancelPengajuanKPDto dto);
        Task<IEnumerable<GetDataKPRiwayat>> GetDataKPRiwayatAsync(GetKPRiwayatRequest request);
        Task<IEnumerable<GetDataKP>> GetDataKPAsync(GetKPRequest request);
        Task<IEnumerable<GetKPByDosenResponseDto>> GetDataKPByDosenAsync(string KaryawanUsername, GetKPByDosenRequestDto request);
        Task<IEnumerable<GetKPByNimResponseDto>> GetDataKPByNIMAsync(GetKPByNimRequestDto request);
        Task<DetailKPDto> GetDetailKPAsync(string Id);
        Task<DetailPertemuanKPDto> GetDetailPertemuanKPAsync(string Id);
        Task<string?> CheckJadwalBentrokKPAsync(CheckJadwalBentrokKPRequestDto dto);
        Task<IEnumerable<CheckJadwalBentrokResponseDto>> CheckJadwalBentrokAsync(CheckJadwalBentrokRequestDto dto);

        Task<TahunAjaranActiveDto?> GetTahunAjaranAktifAsync();
        Task<IEnumerable<GetTahunSemesterActive>> GetTahunSemesterActiveAsync();

        Task<IEnumerable<GetAllListDosen>> GetAllListDosenAsync();
        Task<IEnumerable<GetListGrup>> GetListGrupAsync(string KonsentrasiId, string KelasId, string GrupTahunAjaran, string GrupSemester);
        Task<IEnumerable<GetListKelas>> GetListKelasAsync(string KonsentrasiId, string TahunAjaran, string Jenis);
        Task<IEnumerable<GetKelasByNimDto>> GetKelasByNIMAsync(string KonsentrasiId, string TahunAjaran, string Nim);
        Task<IEnumerable<GetKelasByDosenDto>> GetKelasByDosenAsync(string KonsentrasiId, string TahunAjaran, string Username);
        Task<List<GetMataKuliahKPDto>> GetMataKuliahKPAsync(string KonsentrasiId,string KelasId);

        Task<GetMataKuliahDetailDto> GetMataKuliahDetailAsync(string MataKuliahId,string KonsentrasiId,string TahunAjaran,string Semester,string GrupId,string SectionId);

        Task<List<GetMataKuliahByNimDto>> GetMataKuliahByNIMAsync(string NimMahasiswa, string TahunAjaran, string Semester);

        Task<List<GetMataKuliahByDosenDto>> GetMataKuliahByDosenAsync(string UsernameDosen, string KelasId, string TahunAjaran,string Semester, string SectionId);
        Task<IEnumerable<PertemuanListDto>> GetListPertemuanAsync(string KonsentrasiId, string TahunAjaran, string Semester, string MataKuliahId, string GrupId, string SectionId);
        Task<IEnumerable<GetAllListProdi>> GetAllListProdiAsync();
        Task<IEnumerable<GetKonsentrasiByNimDto>> GetKonsentrasiByNIMAsync(string Nim);
        Task<IEnumerable<GetKonsentrasiByDosenDto>> GetKonsentrasiByDosenAsync(string Username);
        Task<IEnumerable<GetAllListRuangan>> GetAllListRuanganAsync();
        Task<IEnumerable<SectionDto>> GetSectionByGrupAsync(string KonsentrasiId, string KelasId, string GrupId, string TahunAjaran, string Semester);



    }

}