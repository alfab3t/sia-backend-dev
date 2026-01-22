using astratech_apps_backend.DTOs.RiwayatPembukuan;
using astratech_apps_backend.Models;

namespace astratech_apps_backend.Repositories.Interfaces
{
    public interface IRiwayatPembukuanRepository
    {
        Task<(IEnumerable<RiwayatPembukuan>, int TotalData)> GetAllAsync(GetAllRiwayatPembukuanRequest dto);
        Task<IEnumerable<RiwayatPembukuanExportDto>> GetForExportAsync(GetAllRiwayatPembukuanRequest dto);
        Task<IEnumerable<(string Id, string Nama)>> GetListKonsentrasiAsync();
        Task<IEnumerable<string>> GetListTahunAjaranAsync();
        Task<string?> GetActiveTahunAjaranByTanggalAsync();
    }
}