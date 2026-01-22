using astratech_apps_backend.DTOs.TatapMuka;
using astratech_apps_backend.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
 using System.Globalization;
using System.Data;

namespace astratech_apps_backend.Repositories.Implementations
{
    public class TatapMukaRepository : ITatapMukaRepository
    {
        private readonly string _conn;

        private const string ParamIdRuangan = "@IdRuangan";
        private const string ParamIdGrup = "@IdGrup";
        private const string ParamIdMataKuliah = "@IdMataKuliah";
        private const string ParamIdSection = "@IdSection";
        private const string ParamTahunAkademik = "@TahunAkademik";
        private const string ParamTanggal = "@Tanggal";
        private const string ParamIdTatapMuka = "@IdTatapMuka";
        private const string ParamKaryawanUsername = "@KaryawanUsername";
        private const string ColumnMkuNama = "mku_nama";
        private const string ColumnTanggal = "tanggal";
        private const string ColumnKonNama = "kon_nama";

        public TatapMukaRepository(IConfiguration config)
        {
            _conn = PolmanAstraLibrary.PolmanAstraLibrary.Decrypt(
                config.GetConnectionString("DefaultConnection")!,
                Environment.GetEnvironmentVariable("DECRYPT_KEY_CONNECTION_STRING")
            );
        }

        private static string FormatJadwalTatapMuka(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return "";

            var parts = raw.Split('#');
            if (parts.Length != 3)
                return raw;

            if (!DateTime.TryParseExact(
                    parts[0],
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var date))
                return raw;

            var culture = new CultureInfo("id-ID");
            var tanggal = date.ToString("dddd, dd MMMM yyyy", culture);

            return $"{tanggal}\n{parts[1]} - {parts[2]}";
        }

        public async Task CreateTatapMukaAsync(CreateTatapMukaDto dto)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_createTatapMuka", conn) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.AddWithValue(ParamIdRuangan, dto.RuangId);
            cmd.Parameters.AddWithValue(ParamIdGrup, dto.GrupId.ToString());
            cmd.Parameters.AddWithValue(ParamIdMataKuliah, dto.MataKuliahId);
            cmd.Parameters.AddWithValue(ParamIdSection, dto.SectionId);
            cmd.Parameters.AddWithValue("@IdDosen", dto.DosenId);
            cmd.Parameters.AddWithValue(ParamTahunAkademik, dto.TahunAjaran);
            cmd.Parameters.AddWithValue(ParamTanggal, dto.Tanggal);
            cmd.Parameters.AddWithValue("@WaktuAwal", dto.WaktuAwal);
            cmd.Parameters.AddWithValue("@WaktuAkhir", dto.WaktuAkhir);
            cmd.Parameters.AddWithValue("@Alasan", dto.Alasan);
            cmd.Parameters.AddWithValue("@CreatedBy", dto.CreatedBy);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<IEnumerable<TatapMukaListItemDto>> GetDataTatapMukaAsync(GetDataTatapMukaParams parameters)
        {
            var results = new List<TatapMukaListItemDto>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getDataTatapMuka", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Mode", parameters.Mode);
            cmd.Parameters.AddWithValue("@Search", parameters.SearchText);
            cmd.Parameters.AddWithValue("@OrderBy", parameters.OrderBy);
            cmd.Parameters.AddWithValue("@IdKonsentrasi", parameters.KonsentrasiId);
            cmd.Parameters.AddWithValue(ParamTahunAkademik, parameters.TahunAjaran);
            cmd.Parameters.AddWithValue("@Semester", parameters.Semester);
            cmd.Parameters.AddWithValue("@Role", parameters.Role);
            cmd.Parameters.AddWithValue("@Status", parameters.Status);
            cmd.Parameters.AddWithValue("@Username", parameters.Username);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                results.Add(new TatapMukaListItemDto
                {
                    TatapMukaId = reader["tmu_id"]?.ToString() ?? "",
                    KonsentrasiSingkatan = reader["kon_singkatan"]?.ToString() ?? "",
                    TahunAjaran = reader["tmu_tahun_ajaran"]?.ToString() ?? "",
                    Semester = reader["mku_semester"]?.ToString() ?? "",
                    MataKuliah = reader[ColumnMkuNama]?.ToString() ?? "",
                    DosenPengampu = reader["dos_nama"]?.ToString() ?? "",
                    JadwalTatapMuka = FormatJadwalTatapMuka(reader["tmu_jadwal"]?.ToString()),
                    Ruangan = reader["rua_nama"]?.ToString() ?? "",
                    Status = reader["tmu_status"]?.ToString() ?? "",
                    Tanggal = reader[ColumnTanggal]?.ToString() ?? "",
                    KonsentrasiId = reader["kon_id"]?.ToString() ?? "",
                    Kelas = reader["kel_nama"]?.ToString() ?? "",
                    RuanganId = reader["rua_id"]?.ToString() ?? ""
                });
            }

            return results;
        }

        public async Task<IEnumerable<TatapMukaListItemDto>> GetDataTatapMukaAsync(
          string mode, string searchText, string orderBy, string konsentrasiId,
          string tahunAjaran, string semester, string role, string status, string username)
        {
            var parameters = new GetDataTatapMukaParams
            {
                Mode = mode,
                SearchText = searchText,
                OrderBy = orderBy,
                KonsentrasiId = konsentrasiId,
                TahunAjaran = tahunAjaran,
                Semester = semester,
                Role = role,
                Status = status,
                Username = username
            };
            return await GetDataTatapMukaAsync(parameters);
        }

        public async Task<TatapMukaDetailDto?> GetDetailTatapMukaAsync(string id)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_detailTatapMuka", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue(ParamIdTatapMuka, id);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new TatapMukaDetailDto
                {
                    RuanganId = reader["rua_id"]?.ToString() ?? "",
                    Ruangan = reader["rua_nama"]?.ToString() ?? "",
                    GrupId = reader["gru_id"]?.ToString() ?? "",
                    KelasId = reader["kel_id"]?.ToString() ?? "",
                    Kelas = reader["kel_nama"]?.ToString() ?? "",
                    MataKuliahId = reader["mku_id"]?.ToString() ?? "",
                    SectionId = reader["sec_id"]?.ToString() ?? "",
                    MataKuliah = reader[ColumnMkuNama]?.ToString() ?? "",
                    DosenId = reader["dos_id"]?.ToString() ?? "",
                    DosenPengampu = reader["dos_nama"]?.ToString() ?? "",
                    Konsentrasi = reader[ColumnKonNama]?.ToString() ?? "",
                    TahunAjaran = reader["tmu_tahun_ajaran"]?.ToString() ?? "",
                    Semester = reader["mku_semester"]?.ToString() ?? "",
                    Tanggal = reader["tmu_tanggal"]?.ToString() ?? "",
                    WaktuAwal = reader["tmu_waktu_awal"]?.ToString() ?? "",
                    WaktuAkhir = reader["tmu_waktu_akhir"]?.ToString() ?? "",
                    AlasanTolak = reader["tmu_alasan_tolak"]?.ToString() ?? "",
                    Status = reader["tmu_status"]?.ToString() ?? "",
                    KonsentrasiId = reader["kon_id"]?.ToString() ?? "",
                    Alasan = reader["tmu_alasan"]?.ToString() ?? ""
                };
            }
            return null;
        }

        public async Task EditTatapMukaAsync(EditTatapMukaDto dto)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_editTatapMuka", conn) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.AddWithValue("@IdTatapmuka", dto.TatapMukaId);
            cmd.Parameters.AddWithValue(ParamIdRuangan, dto.RuanganId);
            cmd.Parameters.AddWithValue(ParamIdGrup, dto.GrupId.ToString());
            cmd.Parameters.AddWithValue(ParamIdMataKuliah, dto.MataKuliahId);
            cmd.Parameters.AddWithValue(ParamIdSection, dto.SectionId);
            cmd.Parameters.AddWithValue("@IdDosen", dto.DosenId);
            cmd.Parameters.AddWithValue(ParamTahunAkademik, dto.TahunAjaran);
            cmd.Parameters.AddWithValue(ParamTanggal, dto.Tanggal);
            cmd.Parameters.AddWithValue("@WaktuAwal", dto.WaktuAwal);
            cmd.Parameters.AddWithValue("@WaktuAkhir", dto.WaktuAkhir);
            cmd.Parameters.AddWithValue("@ModifBy", dto.ModifBy);
            cmd.Parameters.AddWithValue("@Alasan", dto.Alasan ?? "");

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteTatapMukaAsync(string tatapMukaId)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_deleteTatapMuka", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue(ParamIdTatapMuka, tatapMukaId);
            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task ApproveTatapMukaAsync(ApproveTatapMukaRequestDto dto)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_approveTatapMuka", conn) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.AddWithValue("@UserRole", dto.UserRole);
            cmd.Parameters.AddWithValue(ParamIdTatapMuka, dto.TatapMukaId);
            cmd.Parameters.AddWithValue("@ApproveBy", dto.ApprovedBy);
            cmd.Parameters.AddWithValue(ParamIdRuangan, dto.RuangIdGA?? "");

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task CancelTatapMukaAsync(CancelTatapMukaRequestDto dto)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_cancelTatapMuka", conn) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.AddWithValue(ParamIdTatapMuka, dto.TatapMukaId);
            cmd.Parameters.AddWithValue("@ModifBy", dto.ModifBy);
            cmd.Parameters.AddWithValue("@IdRole", dto.IdRole);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task RejectTatapMukaAsync(RejectTatapMukaRequestDto dto)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_rejectTatapMuka", conn) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.AddWithValue(ParamIdTatapMuka, dto.TatapMukaId);
            cmd.Parameters.AddWithValue("@tmu_modif_by", dto.ModifBy);
            cmd.Parameters.AddWithValue("@AlasanTolak", dto.AlasanTolak ?? "");

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task SentTatapMukaAsync(SentTatapMukaRequestDto dto)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_sentTatapMuka", conn) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.AddWithValue(ParamIdTatapMuka, dto.TatapMukaId);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }
        public async Task<List<KonsentrasiDto>> GetListKonsentrasiByDosenAsync(string username)
        {
            var result = new List<KonsentrasiDto>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getListKonsentrasiByDosen2", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue(ParamKaryawanUsername, username);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                result.Add(new KonsentrasiDto
                {
                    KonsentrasiId = Convert.ToInt32(reader.GetInt16(0)),
                    Konsentrasi = reader.GetString(1)
                });
            }

            return result;
        }

        public async Task<List<MataKuliahResponseDto>> GetListMatkulByDosenAsync(MataKuliahRequestDto dto, string username)
        {
            var result = new List<MataKuliahResponseDto>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getListMatKulSecByDosen", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add(new SqlParameter("@Idkonsentrasi", dto.KonsentrasiId));
            cmd.Parameters.Add(new SqlParameter("@TahunAkademik", dto.TahunAjaran));
            cmd.Parameters.Add(new SqlParameter("@MataKuliahSemester", dto.Semester));
            cmd.Parameters.Add(new SqlParameter(ParamKaryawanUsername, username));

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                result.Add(new MataKuliahResponseDto
                {
                    MataKuliahId = reader.GetString(0),
                    MataKuliah = reader.GetString(1)
                });
            }

            return result;
        }

        public async Task<TahunAjaranActiveDto?> GetTahunAjaranAktifAsync()
        {
            TahunAjaranActiveDto? result = null;

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getActiveTahunAkademikByTanggal", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                result = new TahunAjaranActiveDto
                {
                    TahunAjaran = reader["kak_tahun_ajaran"].ToString()!,
                    GanjilGenap = reader["kak_ganjil_genap"].ToString()!,
                    TanggalMulai = Convert.ToDateTime(reader["kak_tgl_from"])
                };
            }
            return result;
        }

        public async Task<IEnumerable<ListTahunAkademikActiveDto>> GetListTahunAkademikAsync()
        {
            var list = new List<ListTahunAkademikActiveDto>();

            using (SqlConnection conn = new SqlConnection(_conn))
            {
                await conn.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("sia_getListTahunAjaran", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            list.Add(new ListTahunAkademikActiveDto
                            {
                                TahunAkademik = reader["kak_tahun_ajaran"].ToString()!,
                            });
                        }
                    }
                }
            }
            return list;
        }

        public async Task<IEnumerable<KelasResponeDto>> GetKelasByMatkulSecAsync(KelasRequestDto dto)
        {
            var result = new List<KelasResponeDto>();
            using (SqlConnection conn = new SqlConnection(_conn))
            {
                await conn.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("sia_getListKelasByMatkulSec", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter(ParamIdMataKuliah, dto.MataKuliahId));
                    cmd.Parameters.Add(new SqlParameter(ParamIdSection, dto.SectionId));
                    cmd.Parameters.Add(new SqlParameter("@IdRole", dto.Role));
                    cmd.Parameters.Add(new SqlParameter(ParamKaryawanUsername, dto.Username));

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            result.Add(new KelasResponeDto
                            {
                                KelasId = reader["kel_id"]?.ToString() ?? ""
                            });
                        }
                    }
                }
            }
            return result;
        }

        public async Task<IEnumerable<GetListRuanganDto>> GetListRuanganMahasiswaAsync()
        {
            var results = new List<GetListRuanganDto>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getListRuanganMahasiswa", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                results.Add(new GetListRuanganDto
                {
                    RuanganId = reader["rua_id"].ToString()!,
                    NamaRuangan = reader["rua_nama"].ToString()!
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
                    KonsentrasiId = reader["kon_id"].ToString()!,
                    KonsentrasiNama = reader["kon_nama"].ToString()!
                });
            }

            return results;
        }

        public async Task<IEnumerable<GetProdiByUser>> GetProdiByUserAsync(string username)
        {
            var results = new List<GetProdiByUser>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getKonsentrasiByUser", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Username", username);

            await conn.OpenAsync();

            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                results.Add(new GetProdiByUser
                {
                    KonsentrasiId = reader["kon_id"].ToString()!
                });
            }

            return results;
        }

        public async Task<IEnumerable<GetProdiByNIM>> GetProdiByNIMAsync(string username)
        {
            var results = new List<GetProdiByNIM>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getKonsentrasiByNIM", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@IdMahasiswa", username);

            await conn.OpenAsync();

            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                results.Add(new GetProdiByNIM
                {
                    KonsentrasiId = reader["kon_id"].ToString()!
                });
            }

            return results;
        }

        public async Task<IEnumerable<IdDosenDto>> GetIdDosenByUserAsync(string username)
        {
            var result = new List<IdDosenDto>();
            using (SqlConnection conn = new SqlConnection(_conn))
            {
                await conn.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("sia_getIdDosenByUser", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter(ParamKaryawanUsername, username));

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            result.Add(new IdDosenDto
                            {
                                DosenId = reader["dos_id"]?.ToString() ?? ""
                            });
                        }
                    }
                }
            }
            return result;
        }

        public async Task<IEnumerable<NamaDosenDto>> GetDosenByJadwalMatkulAsync(NamaDosenRequestDto dto)
        {
            var result = new List<NamaDosenDto>();

            using (SqlConnection conn = new SqlConnection(_conn))
            {
                await conn.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("sia_getDosenByJadwalMatkul", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter(ParamIdMataKuliah, dto.MataKuliahId));
                    cmd.Parameters.Add(new SqlParameter(ParamIdSection, dto.SectionId));
                    cmd.Parameters.Add(new SqlParameter(ParamTahunAkademik, dto.TahunAjaran));
                    cmd.Parameters.Add(new SqlParameter("@IdKelas", dto.KelasId));

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            result.Add(new NamaDosenDto
                            {
                                Dosen1 = reader["dos_1"]?.ToString() ?? "",
                                Dosen2 = reader["dos_2"]?.ToString() ?? "",
                                Dosen3 = reader["dos_3"]?.ToString() ?? ""
                            });
                        }
                    }
                }
            }
            return result;
        }

        public async Task<IEnumerable<CheckBentrokResponseDto>> CheckTatapMukaAsync(CheckBentrokRequestDto dto)
        {
            var result = new List<CheckBentrokResponseDto>();

            using (SqlConnection conn = new SqlConnection(_conn))
            {
                await conn.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("sia_checkTatapMuka", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter(ParamIdRuangan, dto.RuanganId));
                    cmd.Parameters.Add(new SqlParameter(ParamIdGrup, dto.GrupId));
                    cmd.Parameters.Add(new SqlParameter(ParamIdMataKuliah, dto.MataKuliahId));
                    cmd.Parameters.Add(new SqlParameter(ParamIdSection, dto.SectionId));
                    cmd.Parameters.Add(new SqlParameter(ParamTahunAkademik, dto.TahunAjaran));
                    cmd.Parameters.Add(new SqlParameter(ParamTanggal, dto.Tanggal));

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            result.Add(new CheckBentrokResponseDto
                            {
                                TatapMukaId = reader["tmu_id"]?.ToString() ?? "",
                            });
                        }
                    }
                }
            }
            return result;
        }

        public async Task<IEnumerable<GrupDto>> GetListGrupAsync(GrupRequestDto dto)
        {
            var list = new List<GrupDto>();

            using (SqlConnection conn = new SqlConnection(_conn))
            {
                await conn.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("sia_getListGrupByKelasMatkulSec", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue(ParamIdMataKuliah, dto.MataKuliahId);
                    cmd.Parameters.AddWithValue(ParamIdSection, dto.SectionId);
                    cmd.Parameters.AddWithValue("@Idkelas", dto.KelasId);

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            list.Add(new GrupDto
                            {
                                GrupId = reader["gru_id"]?.ToString() ?? "",
                                Grup = reader["gru_nama"]?.ToString() ?? ""
                            });
                        }
                    }
                }
            }
            return list;
        }

        public async Task<IEnumerable<BentrokJadwalTatapMukaResponseDto>> CheckJadwalBentrokTatapMukaAsync(BentrokJadwalTatapMukaRequestDto dto)
        {
            var list = new List<BentrokJadwalTatapMukaResponseDto>();

            using (SqlConnection conn = new SqlConnection(_conn))
            {
                await conn.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("sia_checkJadwalBentrokTatapMuka", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@mode", dto.Mode);
                    cmd.Parameters.AddWithValue(ParamTanggal, dto.Tanggal);
                    cmd.Parameters.AddWithValue("@WaktuAwal", dto.WaktuAwal);
                    cmd.Parameters.AddWithValue("@WaktuAkhir", dto.WaktuAkhir);
                    cmd.Parameters.AddWithValue("@IdDosen", dto.DosenId);
                    cmd.Parameters.AddWithValue(ParamTahunAkademik, dto.TahunAjaran);
                    cmd.Parameters.AddWithValue(ParamIdRuangan, dto.RuanganId);
                    cmd.Parameters.AddWithValue(ParamIdGrup, dto.GrupId);
                    cmd.Parameters.AddWithValue(ParamIdSection, dto.SectionId);
                    cmd.Parameters.AddWithValue(ParamIdTatapMuka, dto.TatapMukaId);

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var responseDto = CreateResponseDto(reader);
                            if (responseDto != null)
                            {
                                list.Add(responseDto);
                            }
                        }
                    }
                }
            }
            return list;
        }
        private BentrokJadwalTatapMukaResponseDto? CreateResponseDto(SqlDataReader reader)
        {
            try
            {
                return new BentrokJadwalTatapMukaResponseDto
                {
                    Subjek = reader["subjek"]?.ToString() ?? "",
                    MataKuliah = reader[ColumnMkuNama]?.ToString() ?? "",
                    Tanggal = reader[ColumnTanggal]?.ToString() ?? "",
                    WaktuAwal = reader["waktu_awal"]?.ToString() ?? "",
                    WaktuAkhir = reader["waktu_akhir"]?.ToString() ?? "",
                    Konsentrasi = reader[ColumnKonNama]?.ToString() ?? ""
                };
            }
            catch
            {
                return null;
            }
        }
    }
}