using astratech_apps_backend.DTOs.Kamar_Dormitory;

namespace astratech_apps_backend.Repositories.Interfaces
{
    public interface IKamarDormitoryRepository
    {
        Task<(IEnumerable<KamarDormitoryDto>, int totalData)> GetAllAsync(GetAllKamarDormitoryRequest dto);
        Task<KamarDormitoryDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(CreateKamarDormitoryRequest dto, string createdBy);
        Task<bool> UpdateAsync(UpdateKamarDormitoryRequest dto, string updatedBy);
        Task<bool> SetStatusAsync(int id);
        Task<IEnumerable<AtributKamarDto>> GetDefaultAttributesAsync();
        Task<(IEnumerable<PenghuniKamarDto> list, int totalData, int jumlahAktif)> GetPenghuniKamarAsync(GetPenghuniKamarRequest dto);
        Task<DetailPenghuniKamarDto?> GetDetailPenghuniKamarAsync(int idPenghuni);
        Task<int> GetJumlahPenghuniAktifAsync(int idKamar);
        Task<bool> CreatePenghuniAsync(CreatePenghuniKamarRequest dto, string createdBy);
        Task<bool> PindahPenghuniAsync(PindahPenghuniRequest dto, string updatedBy);
        Task<bool> CheckoutPenghuniAsync(CheckoutPenghuniRequest dto, string updatedBy);
        Task<IEnumerable<MahasiswaDto>> GetAllMahasiswaAsync(GetAllMahasiswaRequest request);
        Task<IEnumerable<KamarDormitoryDto>> GetAllKamarDropdownAsync();
        Task<List<KamarDormitoryExportDto>> GetDataForExportAsync(ExportKamarDormitoryRequest request);

    }
}
