using astratech_apps_backend.DTOs.TatapMuka;

namespace astratech_apps_backend.Repositories.Interfaces
{
    public interface ITatapMukaRepository
    {
        Task CreateTatapMukaAsync(CreateTatapMukaDto dto);
        Task<IEnumerable<TatapMukaListItemDto>> GetDataTatapMukaAsync(
            string mode, string searchText, string orderBy, string konsentrasiId, string tahunAjaran, string semester, string role, string status, string username);
        Task<TatapMukaDetailDto?> GetDetailTatapMukaAsync(string id);
        Task EditTatapMukaAsync(EditTatapMukaDto dto);
        Task DeleteTatapMukaAsync(string tatapMukaId);
        Task ApproveTatapMukaAsync(ApproveTatapMukaRequestDto dto);
        Task CancelTatapMukaAsync(CancelTatapMukaRequestDto dto);
        Task RejectTatapMukaAsync(RejectTatapMukaRequestDto dto);
        Task SentTatapMukaAsync(SentTatapMukaRequestDto dto);
        Task<TahunAjaranActiveDto?> GetTahunAjaranAktifAsync();
        Task<IEnumerable<ListTahunAkademikActiveDto>> GetListTahunAkademikAsync();
        Task<IEnumerable<GetListRuanganDto>> GetListRuanganMahasiswaAsync();
        Task<List<KonsentrasiDto>> GetListKonsentrasiByDosenAsync(string username);
        Task<List<MataKuliahResponseDto>> GetListMatkulByDosenAsync(MataKuliahRequestDto dto, string username);
        Task<IEnumerable<KelasResponeDto>> GetKelasByMatkulSecAsync(KelasRequestDto dto);
        Task<IEnumerable<NamaDosenDto>> GetDosenByJadwalMatkulAsync(NamaDosenRequestDto dto);
        Task<IEnumerable<IdDosenDto>> GetIdDosenByUserAsync(string username);
        Task<IEnumerable<GrupDto>> GetListGrupAsync(GrupRequestDto dto);
        Task<IEnumerable<CheckBentrokResponseDto>> CheckTatapMukaAsync(CheckBentrokRequestDto dto);
        Task<IEnumerable<BentrokJadwalTatapMukaResponseDto>> CheckJadwalBentrokTatapMukaAsync(BentrokJadwalTatapMukaRequestDto dto);
        Task<IEnumerable<GetAllListProdi>> GetAllListProdiAsync();
        Task<IEnumerable<GetProdiByUser>> GetProdiByUserAsync(string username);
        Task<IEnumerable<GetProdiByNIM>> GetProdiByNIMAsync(string username);

    }
}
