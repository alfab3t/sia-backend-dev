using astratech_apps_backend.DTOs.JadwalUjian;
using astratech_apps_backend.Models;

namespace astratech_apps_backend.Repositories.Interfaces
{
    public interface IJadwalUjianRepository
    {
        Task<int> CreateAsyncUjian(CreateJadwalUjianDto dto, string createdBy);
        Task<int> CreateAsyncDetail(CreateJadwalUjianDetailDto dto, string createdBy);
        Task<(IEnumerable<JadwalUjian>, int totalData)> GetAllAsync(GetAllJadwalUjianRequest dto);
        Task<JadwalUjianDetailDto?> GetDetailAsync(int idJadwalUjian);
        Task<IEnumerable<JadwalUjian>> GetDataAsyncDetail(int idJadwalUjian);
        Task<IEnumerable<GetDataMatkul>> GetDataMatkulAllAsync(GetDataMatkul dto);
        Task<bool> UpdateAsync(UpdateJadwalUjianRequest dto, string updatedBy);
        Task<bool> FinalizeAsync(string[] idJadwalUjian, string modifiedBy);
         Task<CheckBentrokResultDto> CheckJadwalBentrok(DateTime tanggal, TimeSpan mulai, TimeSpan selesai);
        Task<CheckJadwalUjianResultDto> CheckJadwalUjian(string IdGrup, string TahunAkademik, string Semester, string Jenis);
        Task<bool> DeleteJadwalUjianAsync(string idJadwal, string modifiedBy);


        Task<IEnumerable<GetAllListDosen>> GetAllListDosenAsync();
        Task<IEnumerable<GetListGrup>> GetListGrupAsync(string KonsentrasiId, string KelasId, string GrupTahunAjaran, string GrupSemester);
        Task<IEnumerable<GetListKelas>> GetListKelasAsync(string KonsentrasiId, string TahunAjaran, string jenis);
        Task<IEnumerable<GetAllListProdi>> GetAllListProdiAsync();  
        Task<IEnumerable<GetAllListRuangan>> GetAllListRuanganAsync();
        Task<IEnumerable<SectionDto>> GetSectionByGrupAsync(string KonsentrasiId, string KelasId, string GrupId, string TahunAjaran, string Semester);
        Task<IEnumerable<GetTahunSemesterActive>> GetTahunSemesterActiveAsync();




    }
}