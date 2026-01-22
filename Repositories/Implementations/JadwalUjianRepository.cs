using astratech_apps_backend.DTOs.JadwalUjian;
using astratech_apps_backend.Models;
using astratech_apps_backend.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Globalization;

namespace astratech_apps_backend.Repositories.Implementations
{
    public class JadwalUjianRepository(IConfiguration config): IJadwalUjianRepository
    {
        private readonly string _conn = PolmanAstraLibrary.PolmanAstraLibrary.Decrypt(config.GetConnectionString("DefaultConnection")!, Environment.GetEnvironmentVariable("DECRYPT_KEY_CONNECTION_STRING"));
        
        
        private const string ParameterSemester = "@semester";
        private const string GrupIdParameter = "@GrupId";
        private const string TahunAkademikParameter = "@TahunAkademik";
        private const string JenisParameter = "@Jenis";
        private const string IdParameter = "@Id";
        private const string COL_KON_ID = "kon_id";
        private const string COL_KON_NAMA = "kon_nama";
        private const string COL_KELAS_ID = "kel_id";

   
        public async Task<int> CreateAsyncUjian(CreateJadwalUjianDto dto, string createdBy)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_createJadwalUjian", conn) 
            { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.AddWithValue("@IdGrup", dto.GrupId);
            cmd.Parameters.AddWithValue(TahunAkademikParameter, dto.TahunAkademik);
            cmd.Parameters.AddWithValue(ParameterSemester, dto.Semester);
            cmd.Parameters.AddWithValue("@Jenis", dto.JenisUjian);
            cmd.Parameters.AddWithValue("@createdBy", createdBy);
            
            await conn.OpenAsync();
            var newId = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(newId);
        }

        public async Task<int> CreateAsyncDetail(CreateJadwalUjianDetailDto dto, string createdBy)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_createJadwalUjianDetail", conn) 
            { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.AddWithValue(IdParameter, dto.IdJadwalUjian);
            cmd.Parameters.AddWithValue("@MataKuliah", dto.IdMataKuliah);
            cmd.Parameters.AddWithValue("@Section", dto.IdSection);
            cmd.Parameters.AddWithValue("@Ruangan", dto.IdRuangan);
            cmd.Parameters.AddWithValue("@Pengawas1", dto.IdDosen1);
            cmd.Parameters.AddWithValue("@Pengawas2", dto.IdDosen2 ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Tanggal", dto.Tanggal);
            cmd.Parameters.AddWithValue("@WaktuMulai", dto.WaktuMulai);
            cmd.Parameters.AddWithValue("@WaktuSelesai", dto.WaktuSelesai);
            cmd.Parameters.AddWithValue("@CreatedBy", createdBy);

            await conn.OpenAsync();
            var newId = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(newId);
        }


        public async Task<(IEnumerable<JadwalUjian>, int totalData)> GetAllAsync(GetAllJadwalUjianRequest dto)
        {
            var results = new List<JadwalUjian>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getDataJadwalUjian", conn) 
            { 
                CommandType = CommandType.StoredProcedure 
            };

            cmd.Parameters.AddWithValue("@SearchTerm", dto.SearchTerm ?? string.Empty);
            cmd.Parameters.AddWithValue("@SortColumn", dto.SortColumn ?? "id");
            cmd.Parameters.AddWithValue("@TahunAkademik", dto.TahunAkademik);
            cmd.Parameters.AddWithValue(ParameterSemester, dto.Semester);
            cmd.Parameters.AddWithValue("@KonsentrasiId", dto.KonsentrasiId ?? string.Empty);
            cmd.Parameters.AddWithValue("@KelasId", dto.KelasId ?? string.Empty);
            cmd.Parameters.AddWithValue("@GrupId", dto.GrupId ?? string.Empty);
            cmd.Parameters.AddWithValue("@JenisUjian", dto.JenisUjian ?? string.Empty);
            cmd.Parameters.AddWithValue("@UserRole", dto.UserRole ?? string.Empty);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var jadwal = new JadwalUjian
                {
                    IdJadwalUjian = Convert.ToInt32(reader["id"]),
                    TahunAkademik = reader["tahunakademik"]?.ToString() ?? string.Empty,
                    Semester = reader["semester"]?.ToString() ?? string.Empty,
                    Prodi = reader["kon_nama"]?.ToString() ?? string.Empty,
                    Kelas = reader["kelasgrup"]?.ToString() ?? string.Empty,
                    JenisUjian = reader["jenisujian"]?.ToString() ?? string.Empty,
                    Status = reader["status"]?.ToString() ?? string.Empty
                };
                results.Add(jadwal);
            }

            return (results, results.Count);
        }
        
        public async Task<JadwalUjianDetailDto?> GetDetailAsync(int idJadwalUjian)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_detailJadwalUjian", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@IdJadwalUjian", idJadwalUjian);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new JadwalUjianDetailDto
                {
                    IdDetailJadwalUjian = idJadwalUjian, 
                    Prodi = reader.GetString(reader.GetOrdinal("kon_nama")),
                    TahunAkademik = reader.GetString(reader.GetOrdinal("juj_tahun_akademik")),
                    Semester = reader.GetString(reader.GetOrdinal("juj_semester")),
                    Kelas = reader.GetString(reader.GetOrdinal("kel_id")),
                    Grup = reader.GetString(reader.GetOrdinal("gru_nama")),
                    JenisUjian = reader.GetString(reader.GetOrdinal("juj_jenis")),
                    Status = reader.GetString(reader.GetOrdinal("juj_status")) 
                };
            }
            return null;
        }

public async Task<IEnumerable<JadwalUjian>> GetDataAsyncDetail(int idJadwalUjian) 
        {
            var results = new List<JadwalUjian>();
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_detailJadwalUjianDetail", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@IdJadwalUjian", idJadwalUjian);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

                        while (await reader.ReadAsync())
                {
                    var jadwalUjian = new JadwalUjian
                    {
                        IdJadwalUjian = Convert.ToInt32(reader["id"]),
                        IdMataKuliah = Convert.ToInt32(reader["id_MataKuliah"]),
                        TahunAkademik = reader["tahunakademik"].ToString()!,
                        Semester = reader["semester"].ToString()!,
                        Prodi = reader["konsentrasi"].ToString()!,
                        Kelas = reader["kelasgrup"].ToString()!,
                        MataKuliah = reader["matkulsection"].ToString()!,
                        Pengawas1 = reader["pengawas1"].ToString()!,
                        Pengawas2 = reader["pengawas2"].ToString()!,
                        Ruangan = reader["ruangan"].ToString()!,
                        TanggalUjian = reader["tanggalujian"].ToString()!,
                        WaktuUjian = reader["waktuujian"].ToString()!,
                        JenisUjian = reader["jenisujian"].ToString()!,
                        Status = reader["status"].ToString()!
                    };

                    results.Add(jadwalUjian);
                }

            return results;
        }

        public async Task<IEnumerable<GetDataMatkul>> GetDataMatkulAllAsync(GetDataMatkul dto)
        {
            var results = new List<GetDataMatkul>();

            try
            {
                await using var conn = new SqlConnection(_conn);
                await using var cmd = new SqlCommand("sia_getDataMatkulJadwalUjian", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                
                cmd.Parameters.AddWithValue("@IdGrup", string.IsNullOrEmpty(dto.GrupId) ? (object)DBNull.Value : dto.GrupId);
                cmd.Parameters.AddWithValue(TahunAkademikParameter, string.IsNullOrEmpty(dto.TahunAkademik) ? (object)DBNull.Value : dto.TahunAkademik);
                cmd.Parameters.AddWithValue(ParameterSemester, string.IsNullOrEmpty(dto.Semester) ? (object)DBNull.Value : dto.Semester);
                cmd.Parameters.AddWithValue("@JenisUjian", string.IsNullOrEmpty(dto.JenisUjian) ? (object)DBNull.Value : dto.JenisUjian);

                await conn.OpenAsync();
                await using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var dataMatkul = new GetDataMatkul
                    {
                        IdMataKuliah = Convert.ToInt32(reader["mku_id"] ?? 0),
                        
                        MataKuliah = reader.GetString(reader.GetOrdinal("matkul")) ?? string.Empty,
                        GrupId = dto.GrupId,
                        TahunAkademik = dto.TahunAkademik,
                        Semester = dto.Semester,
                        JenisUjian = dto.JenisUjian
                    };
                    
                    results.Add(dataMatkul);
                }
            }
            catch (SqlException sqlEx)
            {
                throw new InvalidOperationException($"Database error: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while retrieving data", ex);
            }

            return results;
        }


        public async Task<bool> UpdateAsync(UpdateJadwalUjianRequest dto, string updatedBy)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_editJadwalUjianDetail", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue(IdParameter, dto.IdJadwalUjian);
            cmd.Parameters.AddWithValue("@MataKuliah", dto.IdMataKuliah);
            cmd.Parameters.AddWithValue("@Section", dto.IdSection);
            cmd.Parameters.AddWithValue("@Ruang", dto.IdRuang);
            cmd.Parameters.AddWithValue("@Pengawas1", dto.IdDosen1);
            cmd.Parameters.AddWithValue("@Pengawas2", dto.IdDosen2 ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Tanggal", dto.Tanggal);
            cmd.Parameters.AddWithValue("@WaktuMulai", dto.WaktuMulai);
            cmd.Parameters.AddWithValue("@WaktuSelesai", dto.WaktuSelesai);
            cmd.Parameters.AddWithValue("@ModifBy", updatedBy);

            await conn.OpenAsync();
            var rowsAffected = await cmd.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<bool> FinalizeAsync(string[] idJadwalUjian, string modifiedBy) 
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_finalJadwalUjian", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            var idString = string.Join("_", idJadwalUjian);

            cmd.Parameters.AddWithValue(IdParameter, idString);
            cmd.Parameters.AddWithValue("@ModifBy", modifiedBy);

            await conn.OpenAsync();
            var rowsAffected = await cmd.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

    
        public async Task<CheckBentrokResultDto> CheckJadwalBentrok(DateTime tanggal, TimeSpan mulai, TimeSpan selesai)
        {
            var results = new List<CheckBentrokResponseDto>();
            
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_checkJadwalUjianBentrok", conn) 
            { 
                CommandType = CommandType.StoredProcedure 
            };
            
            cmd.Parameters.AddWithValue("@Tanggal", tanggal.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@WaktuMulai", mulai.ToString());
            cmd.Parameters.AddWithValue("@WaktuSelesai", selesai.ToString());

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();
            
            while (await reader.ReadAsync())
            {
                var bentrok = new CheckBentrokResponseDto
                {
                    IdJadwalUjian = Convert.ToInt32(reader["jud_id"]),
                    Kelas = reader["kelas"]?.ToString() ?? string.Empty,
                    MataKuliah = reader["matkul"]?.ToString() ?? string.Empty,
                    IdMataKuliah = Convert.ToInt32(reader["mku_id"]),
                    IdSection = reader["sec_id"] == DBNull.Value ? null : Convert.ToInt32(reader["sec_id"]),
                    IdGrup = reader["gru_id"]?.ToString() ?? string.Empty,
                    IdRuang = Convert.ToInt32(reader["rua_id"]),
                    IdDosen1 = Convert.ToInt32(reader["dos_id_1"]),
                    IdDosen2 = reader["dos_id_2"] == DBNull.Value ? null : Convert.ToInt32(reader["dos_id_2"]),
                    Tanggal = Convert.ToDateTime(reader["jud_tanggal"]),
                    WaktuMulai = TimeSpan.Parse(reader["jud_waktu_mulai"].ToString()!, CultureInfo.InvariantCulture), 
                    WaktuSelesai = TimeSpan.Parse(reader["jud_waktu_selesai"].ToString()!, CultureInfo.InvariantCulture), 
                    Konsentrasi = reader["kon_singkatan"]?.ToString() ?? string.Empty
                };
                results.Add(bentrok);
            }

            return new CheckBentrokResultDto
            {
                IsBentrok = results.Any(),
                JadwalBentrok = results
            };
        }

        public async Task<CheckJadwalUjianResultDto> CheckJadwalUjian(string IdGrup, string TahunAkademik, string Semester, string Jenis)
        {
            var results = new List<CheckJadwalUjianResponseDto>();
            
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_checkJadwalUjian", conn) 
            { 
                CommandType = CommandType.StoredProcedure 
            };
            
            cmd.Parameters.AddWithValue(GrupIdParameter, IdGrup);
            cmd.Parameters.AddWithValue(TahunAkademikParameter, TahunAkademik);
            cmd.Parameters.AddWithValue(ParameterSemester, Semester);
            cmd.Parameters.AddWithValue(JenisParameter, Jenis);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();
            
            while (await reader.ReadAsync())
            {
                var jadwal = new CheckJadwalUjianResponseDto
                {
                    StatusCode = Convert.ToInt32(reader[0]),
                    IdJadwalUjian = Convert.ToInt32(reader["juj_id"])
                };
                results.Add(jadwal);
            }

            return new CheckJadwalUjianResultDto
            {
                IsExist = results.Any(),
                JadwalList = results
            };
        }


        public async Task<bool> DeleteJadwalUjianAsync(string idJadwal, string modifiedBy)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_deleteJadwalUjian", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@IdJadwal", idJadwal);
            cmd.Parameters.AddWithValue("@ModifBy", modifiedBy);

            await conn.OpenAsync();
            var rowsAffected = await cmd.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

       public async Task<IEnumerable<GetAllListDosen>> GetAllListDosenAsync()
        {
            var results = new List<GetAllListDosen>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getListDosen", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            await conn.OpenAsync();

            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                results.Add(new GetAllListDosen
                {
                    DosenId = reader["dos_id"].ToString()!,
                    NamaDosen = reader["dos_nama"].ToString()!
                });
            }

            return results;
        }

        public async Task<IEnumerable<GetListGrup>> GetListGrupAsync(string KonsentrasiId, string KelasId, string GrupTahunAjaran, string GrupSemester)
        {
            var results = new List<GetListGrup>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getListGrupByKonsentrasiKelas", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@IdKonsentrasi", KonsentrasiId);
            cmd.Parameters.AddWithValue("@IdKelas", KelasId);
            cmd.Parameters.AddWithValue("@GrupTahunAjaran", GrupTahunAjaran);
            cmd.Parameters.AddWithValue("@GrupSemester", GrupSemester);

            await conn.OpenAsync();

            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                results.Add(new GetListGrup
                {
                    GrupId = reader["gru_id"].ToString()!,
                    GrupNama = reader["gru_nama"].ToString()!,

                });
            }

            return results;
        }

        public async Task<IEnumerable<GetAllListProdi>> GetAllListProdiAsync()
        {
            var results = new List<GetAllListProdi>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getListKonsentrasi", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                results.Add(new GetAllListProdi
                {
                    KonsentrasiId   = reader[COL_KON_ID]?.ToString() ?? "",
                    NamaKonsentrasi = reader[COL_KON_NAMA]?.ToString() ?? ""
                });
            }

            return results;
        }

        public async Task<IEnumerable<GetAllListRuangan>> GetAllListRuanganAsync()
        {
            var results = new List<GetAllListRuangan>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getListRuanganMahasiswa", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            await conn.OpenAsync();

            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                results.Add(new GetAllListRuangan
                {
                    RuanganId = reader["rua_id"].ToString()!,
                    NamaRuangan = reader["rua_nama"].ToString()!
                });
            }

            return results;
        }

        public async Task<IEnumerable<GetListKelas>> GetListKelasAsync(string KonsentrasiId, string TahunAjaran, string jenis)
        {
            var results = new List<GetListKelas>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getListKelas", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@IdKonsentrasi", KonsentrasiId);
            cmd.Parameters.AddWithValue("@TahunAkademik", TahunAjaran);
            cmd.Parameters.AddWithValue("@JenisKelas", jenis);

            await conn.OpenAsync();

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                results.Add(new GetListKelas
                {
                    KelasId = reader[COL_KELAS_ID]?.ToString() ?? string.Empty
                });
            }

            return results;
        }

        public async Task<IEnumerable<SectionDto>> GetSectionByGrupAsync(string KonsentrasiId, string KelasId, string GrupId, string TahunAjaran, string Semester)
        {
            var result = new List<SectionDto>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getListSectionByGrup2", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@IdKonsentrasi", KonsentrasiId);
            cmd.Parameters.AddWithValue("@IdKelas", KelasId);
            cmd.Parameters.AddWithValue("@IdGrup", GrupId);
            cmd.Parameters.AddWithValue("@TahunAjaran", TahunAjaran);
            cmd.Parameters.AddWithValue("@Semester", Semester);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(new SectionDto
                {
                    SectionId = reader["sec_id"].ToString() ?? "",
                    SectionNama = reader["sec_nama"].ToString() ?? ""
                });
            }

            return result;
        }

        public async Task<IEnumerable<GetTahunSemesterActive>> GetTahunSemesterActiveAsync()
        {
            var results = new List<GetTahunSemesterActive>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getActiveTahunAkademikByTanggal", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            await conn.OpenAsync();

            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                results.Add(new GetTahunSemesterActive
                {
                    TahunAjaran = reader["kak_tahun_ajaran"].ToString()!,
                    Semester = reader["kak_ganjil_genap"].ToString()!,
                    TanggalMulai = Convert.ToDateTime(reader["kak_tgl_from"])
                });
            }

            return results;
        }
    }
}