using astratech_apps_backend.DTOs.Mahasiswa;
using astratech_apps_backend.Models;
using System.Data;

namespace astratech_apps_backend.Repositories.Interfaces
{
    public interface IMahasiswaRepository
    {
        Task<(IEnumerable<Mahasiswa>, int totalData)> GetAllAsync(GetAllMahasiswaRequest dto);
        Task<Mahasiswa?> GetByIdAsync(string mahasiswaId);
        Task<Mahasiswa?> GetProfilAsync(string mahasiswaId);

        Task<bool> UpdateAsync(UpdateDataMahasiswaRequest dto, string updatedBy);

        Task<string?> CreateRfidMahasiswaAsync(string mahasiswaId, string rfid, string createdBy);
        Task<string?> SetStatusRfidAsync(string status, string rmaId);
        Task<IEnumerable<Mahasiswa>> GetDataRfidAsync(string mahasiswaId);
        Task<byte[]> ExportMahasiswaRFIDExcelAsync(string konsentrasiId, string urut);

        Task<byte[]> ExportMahasiswaExcelAsync(ExportMahasiswaRequest request, string roleId, string username);

        Task<IEnumerable<(string Semester, string Status, int Sks)>> GetDataRiwayatKuliahAsync(string mahasiswaId);
        Task<IEnumerable<(string Semester, string MataKuliahKode, string MataKuliahNama, int MataKuliahSks, string Nilai, string MataKuliahTipe, int Rownum, int JumlahSks)>> GetDataRiwayatStudiAsync(string mahasiswaId);

        Task<IEnumerable<(string Semester, decimal? Ip, decimal? Ipk, string Publish)>> GetDataPerformaIpAsync(string mahasiswaId);
        Task<IEnumerable<(string Semester, int Persentase)>> GetDataPerformaKehadiranAsync(string mahasiswaId);
        Task<IEnumerable<(string Semester, decimal Minus)>> GetDataPerformaJamMinusAsync(string mahasiswaId);
        Task<IEnumerable<(string Semester, int Total, int Teguran, int Sp1, int Sp2, int Sp3)>> GetDataPerformaPelanggaranAsync(string mahasiswaId);

        Task<string> CheckLastStatusBeasiswaDOResignAsync(string mahasiswaId);

        Task<IEnumerable<string>> GetListAngkatanAktifAsync();
        Task<IEnumerable<(string KonsentrasiId, string KonsentrasiNama)>> GetListKonsentrasiAsync(string username, string roleId);
        Task<IEnumerable<string>> GetListProvinsiAsync();
        Task<IEnumerable<string>> GetListKabupatenAsync(string provinsi);
        Task<IEnumerable<string>> GetListKecamatanAsync(string provinsi, string kabupaten);
        Task<IEnumerable<string>> GetListKelurahanAsync(string provinsi, string kabupaten, string kecamatan);

        Task<List<Mahasiswa>> GetLaporanKegiatanMahasiswaAsync(GetLaporanKegiatanRequest request);
        Task<bool> CreateRekeningAsync(string mahasiswaId, string atasNama, string norek, string namaBank, string createdBy);
        Task<IEnumerable<string>> CreateNotifikasiAsync(string action, string refId, string appId, string title, string createdBy, string description);
        Task<IEnumerable<(int Tahun, int Bulan, int TotalAbsen)>> GetDashboardAbsensiSatgasAsync(string createdBy);

        Task<string> CheckBebasTanggunganAsync(string mahasiswaId);
        Task<bool> CheckCompletedKuesionerAsync(string mahasiswaId);
        Task<string?> GetNamaFileFotoAsync(string mahasiswaId);
    }
}