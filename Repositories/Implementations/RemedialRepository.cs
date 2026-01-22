using astratech_apps_backend.DTOs.Remedial;
using astratech_apps_backend.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Globalization;

namespace astratech_apps_backend.Repositories.Implementations
{
    public class RemedialRepository(IConfiguration config) : IRemedialRepository
    {
        private readonly string _conn = PolmanAstraLibrary.PolmanAstraLibrary.Decrypt(config.GetConnectionString("DefaultConnection")!, Environment.GetEnvironmentVariable("DECRYPT_KEY_CONNECTION_STRING"));
        private const string ParamUsername = "@Username";
        private const string ParamMataKuliahId = "@MataKuliahId";
        private const string ParamSectionId = "@SectionId";
        private const string ParamKelas = "@Kelas";
        private const string ParamTahunAjaran = "@TahunAjaran";
        private const string ParamSemester = "@Semester";
        private const string ParamKonsentrasiId = "@KonsentrasiId";

        public async Task<(IEnumerable<RemedialDto>, int totalData)> GetDataRemedialAsync(
            GetAllRemedialRequest dto, string username, string userRole)
        {
            var list = new List<RemedialDto>();
            int totalData = 0;

            try
            {
                var (IdMatakuliah, _) = ParseMataKuliahId(dto.IdMataKuliah ?? "");

                await using var conn = new SqlConnection(_conn);
                await using var cmd = new SqlCommand("sia_getDataMahasiswaRemedial", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@SearchKeyword", dto.SearchKeyword ?? "");
                cmd.Parameters.AddWithValue(ParamKonsentrasiId, dto.KonsentrasiId ?? "");
                cmd.Parameters.AddWithValue(ParamTahunAjaran, dto.TahunAjaran ?? "");
                cmd.Parameters.AddWithValue(ParamSemester, dto.Semester ?? "");
                cmd.Parameters.AddWithValue(ParamMataKuliahId, IdMatakuliah?.ToString() ?? "");
                cmd.Parameters.AddWithValue(ParamSectionId, "");
                cmd.Parameters.AddWithValue(ParamKelas, dto.Kelas ?? "");
                cmd.Parameters.AddWithValue(ParamUsername, username);

                await conn.OpenAsync();
                await using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var angkaMutu = reader["angka_mutu"]?.ToString() ?? "";
                    list.Add(new RemedialDto
                    {
                        NilaiMahasiswaId = Convert.ToInt32(reader["nma_id"]),
                        MahasiswaId = reader["mhs_id"].ToString() ?? "",
                        MahasiswaNama = reader["mhs_nama"].ToString() ?? "",
                        NilaiAkhir = Convert.ToDouble(reader["nilai_akhir"]),
                        AngkaMutu = reader["angka_mutu"].ToString() ?? "",
                        NilaiRemedial = reader["nilai_remedial"] == DBNull.Value ? null : (double?)Convert.ToDouble(reader["nilai_remedial"]),
                        AngkaMutuRemedial = reader["angka_mutu_remedial"] == DBNull.Value ? null : reader["angka_mutu_remedial"].ToString(),
                        CanEdit = (angkaMutu == "D" || angkaMutu == "E")
                        
                    });
                }
                
                totalData = list.Count;
                return (list, totalData);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error getting remedial data: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<DropdownResponse>> GetListKonsentrasiAsync(string username, string userRole)
        {
            var list = new List<DropdownResponse>();
            try
            {
                await using var conn = new SqlConnection(_conn);
                string spName = (userRole == "ROL71" || userRole == "ROL72") ? "sia_getListKonsentrasiByNPK" : "sia_getListKonsentrasi";

                await using var cmd = new SqlCommand(spName, conn) { CommandType = CommandType.StoredProcedure };

                if (userRole == "ROL71" || userRole == "ROL72")
                {
                    cmd.Parameters.AddWithValue("@UserRole", userRole);
                    cmd.Parameters.AddWithValue(ParamUsername, username);
                }

                await conn.OpenAsync();
                await using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    list.Add(new DropdownResponse
                    {
                        Value = reader["kon_id"]?.ToString() ?? "",
                        Text = reader["kon_nama"]?.ToString() ?? ""
                    });
                }
                return list;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error getting konsentrasi list: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<DropdownResponse>> GetListTahunAjaranAsync()
        {
            var list = new List<DropdownResponse>();
            try
            {
                await using var conn = new SqlConnection(_conn);
                await using var cmd = new SqlCommand("sia_getListTahunAjaran", conn) { CommandType = CommandType.StoredProcedure };
                await conn.OpenAsync();
                await using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    list.Add(new DropdownResponse
                    {
                        Value = reader["kak_tahun_ajaran"]?.ToString() ?? "",
                        Text = reader["kak_tahun_ajaran"]?.ToString() ?? ""
                    });
                }
                return list;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error getting tahun ajaran list: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<DropdownResponse>> GetListMataKuliahAsync(string konsentrasiId, string tahunAjaran, string semester, string username, string userRole)
        {
            var list = new List<DropdownResponse>();
            try
            {
                if (string.IsNullOrEmpty(tahunAjaran) || string.IsNullOrEmpty(semester)) return list;

                await using var conn = new SqlConnection(_conn);
                await using var cmd = new SqlCommand("sia_getListMatKulSecByDosen2", conn) { CommandType = CommandType.StoredProcedure };

                cmd.Parameters.AddWithValue(ParamKonsentrasiId, konsentrasiId ?? "");
                cmd.Parameters.AddWithValue(ParamTahunAjaran, tahunAjaran ?? "");
                cmd.Parameters.AddWithValue(ParamSemester, semester ?? "");
                cmd.Parameters.AddWithValue(ParamUsername, username ?? "");
                cmd.Parameters.AddWithValue("@RoleId", userRole ?? "");

                await conn.OpenAsync();
                await using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var combinedValue = reader["mku_id"]?.ToString() ?? "";
                    var MatakuliahNama = reader["mku_nama"]?.ToString() ?? "";

                    var parts = MatakuliahNama.Split('!');
                    var displayText = parts.Length >= 2 && !string.IsNullOrEmpty(parts[1]) ? $"{parts[0]} ({parts[1]})" : parts[0];

                    list.Add(new DropdownResponse { Value = combinedValue, Text = displayText });
                }
                return list;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error getting mata kuliah list: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<DropdownResponse>> GetListKelasAsync(string mataKuliahId, string username, string userRole)
        {
            var list = new List<DropdownResponse>();
            try
            {
                var (IdMatakuliah, section) = ParseMataKuliahId(mataKuliahId);
                if (!IdMatakuliah.HasValue) return list;

                await using var conn = new SqlConnection(_conn);
                await using var cmd = new SqlCommand("sia_getListKelasByMatkulSec", conn) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("IdMatakuliah", IdMatakuliah.Value.ToString());
                cmd.Parameters.AddWithValue("@IdRole", userRole);
                cmd.Parameters.AddWithValue("@IdSection", section?.ToString() ?? "");
                cmd.Parameters.AddWithValue("@KaryawanUsername", username);

                await conn.OpenAsync();
                await using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    string namaKelas = reader["kel_id"]?.ToString() ?? ""; 
                    
                    list.Add(new DropdownResponse
                    {
                        Value = namaKelas,
                        Text = namaKelas
                    });
                }
                return list;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error getting kelas list: {ex.Message}", ex);
            }
        }

        public async Task<string> GetDosenByKelasAsync(string mataKuliahId, string kelas)
        {
            try
            {
                var (IdMatakuliah, sectionId) = ParseMataKuliahId(mataKuliahId);
                if (!IdMatakuliah.HasValue || string.IsNullOrEmpty(kelas)) return "-";

                await using var conn = new SqlConnection(_conn);
                await using var cmd = new SqlCommand("sia_getDosenByMatkulSecKel", conn) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue(ParamMataKuliahId, IdMatakuliah.Value.ToString());
                cmd.Parameters.AddWithValue("@Section", sectionId?.ToString() ?? "");
                cmd.Parameters.AddWithValue(ParamKelas, kelas);

                await conn.OpenAsync();
                var result = await cmd.ExecuteScalarAsync();
                return result?.ToString() ?? "-";
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error getting dosen name: {ex.Message}", ex);
            }
        }

        public async Task<(string TahunAjaran, string Semester)> GetActivePeriodeAsync()
        {
            try
            {
                await using var conn = new SqlConnection(_conn);
                await using var cmd = new SqlCommand("sia_getActivePeriodePenilaianByTanggal", conn) { CommandType = CommandType.StoredProcedure };
                await conn.OpenAsync();
                await using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return (reader[0]?.ToString() ?? "", reader[1]?.ToString() ?? "");
                }
                return ("", "");
            }
            catch (Exception)
            {
                return ("", "");
            }
        }

        public async Task<(string TahunAjaran, string Semester)> GetActivePenilaianPeriodAsync() => await GetActivePeriodeAsync();

        public async Task<(string TahunAjaran, string Semester)> GetActiveAcademicYearAsync(DateTime? tanggalReferensi = null) => await GetActivePeriodeAsync();

        public async Task<bool> CanEditRemedialAsync(string tahunAjaran, string semester, string mataKuliahId, string kelas, string username, string userRole)
        {
            try
            {
                var (IdMatakuliah, _) = ParseMataKuliahId(mataKuliahId);
                if (!IdMatakuliah.HasValue) return false;

                await using var connPeriode = new SqlConnection(_conn);
                await using var cmdPeriode = new SqlCommand("sia_checkPeriodePenilaianAktif", connPeriode) { CommandType = CommandType.StoredProcedure };
                cmdPeriode.Parameters.AddWithValue(ParamTahunAjaran, tahunAjaran);
                cmdPeriode.Parameters.AddWithValue(ParamSemester, semester);
                await connPeriode.OpenAsync();
                await using var readerPeriode = await cmdPeriode.ExecuteReaderAsync();
                if (!await readerPeriode.ReadAsync() || readerPeriode[2]?.ToString() != "OK") return false;

                await using var connStatus = new SqlConnection(_conn);
                await using var cmdStatus = new SqlCommand("sia_checkStatusPenilaian", connStatus) { CommandType = CommandType.StoredProcedure };
                
                cmdStatus.Parameters.AddWithValue(ParamMataKuliahId, IdMatakuliah.GetValueOrDefault().ToString());
                
                cmdStatus.Parameters.AddWithValue(ParamKelas, kelas);
                await connStatus.OpenAsync();
                var statusResult = await cmdStatus.ExecuteScalarAsync();
                var status = statusResult?.ToString()?.Trim();
                return status == "Final" || status == "Dalam Perbaikan Nilai";
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UpdateNilaiRemedialAsync(UpdateRemedialRequest request, string username, string userRole)
        {
            try
            {
                if (request.NilaiRemedial.HasValue && (request.NilaiRemedial.Value < 0 || request.NilaiRemedial.Value > 60))
                    throw new ArgumentException("Nilai remedial harus antara 0-60");

                string nilaiRemedialParam = request.NilaiRemedial.HasValue
                    ? request.NilaiRemedial.Value.ToString("F2", CultureInfo.InvariantCulture)
                    : "NULL";

                await using var conn = new SqlConnection(_conn);
                await using var cmd = new SqlCommand("sia_setNilaiRemedial", conn) { CommandType = CommandType.StoredProcedure };

                cmd.Parameters.AddWithValue("@NmaId", request.NilaiMahasiswaId?.ToString() ?? "");
                cmd.Parameters.AddWithValue("@NilaiRemedial", nilaiRemedialParam);
                cmd.Parameters.AddWithValue("@ModifyBy", username);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error updating nilai remedial: {ex.Message}", ex);
            }
        }

        private static (int? IdMatakuliah, int? sectionId) ParseMataKuliahId(string mataKuliahId)
        {
            if (string.IsNullOrEmpty(mataKuliahId)) return (null, null);
            var parts = mataKuliahId.Split('!');
            if (parts.Length >= 2 && int.TryParse(parts[0], out int IdMatakuliah) && int.TryParse(parts[1], out int sectionId))
                return (IdMatakuliah, sectionId);
            if (int.TryParse(mataKuliahId, out int singleId)) return (singleId, null);
            return (null, null);
        }
    }
}