using astratech_apps_backend.DTOs.HasilStudiKriteria;
using astratech_apps_backend.Models.HasilStudiKriteria;
using astratech_apps_backend.Repositories.Interfaces;
using astratech_apps_backend.Services.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace astratech_apps_backend.Repositories.Implementations
{
    public class KriteriaRepository(IConfiguration config, ILdapService ldapService) : IKriteriaRepository
    {
        private readonly string _conn = PolmanAstraLibrary.PolmanAstraLibrary.Decrypt(
            config.GetConnectionString("DefaultConnection")!,
            Environment.GetEnvironmentVariable("DECRYPT_KEY_CONNECTION_STRING")!
        );
        private readonly ILdapService _ldapService = ldapService;

        private const string ParamUsername = "@Username";
        private const string ParamTahunAjaran = "@TahunAjaran";
        private const string ParamSemester = "@Semester";
        private const string ParamKriteriaId = "@KriteriaId";
        private const string ColCreatedDate = "kni_created_date";
        private const string ColModifDate = "kni_modif_date";

        public async Task<(IEnumerable<Kriteria>, int totalData)> GetAllAsync(
            GetAllKriteriaRequest dto, string userId, string userRole)
        {
            var list = new List<Kriteria>();
            int totalData = 0;

            var displayName = await _ldapService.GetDisplayNameAsync(userId) ?? userId;

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getDataKriteria", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Keyword", dto.SearchKeyword ?? "");
            cmd.Parameters.AddWithValue("@KonsentrasiId", dto.KonsentrasiId ?? "");
            cmd.Parameters.AddWithValue(ParamTahunAjaran, dto.TahunAjaran ?? "");
            cmd.Parameters.AddWithValue(ParamSemester, dto.Semester ?? "");
            cmd.Parameters.AddWithValue("@UserRole", userRole ?? "");
            cmd.Parameters.AddWithValue(ParamUsername, userId ?? "");
            cmd.Parameters.AddWithValue("@Status", dto.Status ?? "");
            cmd.Parameters.AddWithValue("@DisplayName", displayName ?? "");
            cmd.Parameters.AddWithValue("@Urut", dto.Urut ?? "desc");
            cmd.Parameters.AddWithValue("@Halaman", dto.PageNumber);
            cmd.Parameters.AddWithValue("@Limit", dto.PageSize);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                totalData = Convert.ToInt32(reader["total_data"] ?? 0);
                await ReadKriteriaListAsync(reader, list);
            }

            var detailTasks = list.Select(k => GetDetailsByKriteriaIdAsync(k.Id)).ToArray();
            var detailResults = await Task.WhenAll(detailTasks);
            for (int i = 0; i < list.Count; i++)
            {
                list[i].Details = detailResults[i].ToList();
            }

            return (list, totalData);
        }

        private static async Task ReadKriteriaListAsync(SqlDataReader reader, List<Kriteria> list)
        {
            do
            {
                var kriteria = new Kriteria
                {
                    Id = reader["kni_id"]?.ToString() ?? "",
                    RowNumber = Convert.ToInt64(reader["rownum"] ?? 0),
                    TahunAjaran = reader["kni_tahun_ajaran"]?.ToString() ?? "",
                    Semester = reader["mku_semester"]?.ToString() ?? "",
                    MataKuliah = reader["mku_nama"]?.ToString() ?? "",
                    MataKuliahId = reader["mku_id"]?.ToString() ?? "",
                    Dosen = reader["dos_nama"]?.ToString() ?? "",
                    DosenId = reader["dos_id_create"]?.ToString() ?? "",
                    Status = reader["kni_status"]?.ToString() ?? "",
                    AlasanTolak = reader["kni_alasan_tolak"]?.ToString() ?? "",
                    TipeKriteria = reader["kni_jenis"]?.ToString() ?? "",
                    Konsentrasi = reader["kon_singkatan"]?.ToString() ?? "",
                    KonsentrasiId = reader["kon_id"]?.ToString() ?? "",
                    CreatedBy = reader["kni_created_by"]?.ToString() ?? "",
                    CreatedDate = reader[ColCreatedDate] != DBNull.Value
                        ? Convert.ToDateTime(reader[ColCreatedDate])
                        : DateTime.MinValue,
                    UpdatedBy = reader["kni_modif_by"]?.ToString() ?? "",
                    UpdatedDate = reader[ColModifDate] != DBNull.Value
                        ? Convert.ToDateTime(reader[ColModifDate])
                        : DateTime.MinValue
                };
                list.Add(kriteria);
            } while (await reader.ReadAsync());
        }

        public async Task<Kriteria?> GetByIdAsync(string id)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_detailKriteria", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue(ParamKriteriaId, id);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                return null;
            }

            var kriteria = new Kriteria
            {
                Id = id,
                TahunAjaran = reader["kni_tahun_ajaran"]?.ToString() ?? "",
                Semester = reader["kni_semester"]?.ToString() ?? "",
                MataKuliah = reader["mku_nama"]?.ToString() ?? "",
                MataKuliahId = reader["mku_id"]?.ToString() ?? "",
                Dosen = reader["dos_nama"]?.ToString() ?? "",
                DosenId = reader["dos_id"]?.ToString() ?? "",
                Status = reader["kni_status"]?.ToString() ?? "",
                AlasanTolak = reader["kni_alasan_tolak"]?.ToString() ?? "",
                TipeKriteria = reader["kni_jenis"]?.ToString() ?? "",
                CreatedBy = reader["kni_created_by"]?.ToString() ?? "",
                CreatedDate = reader[ColCreatedDate] != DBNull.Value
                    ? Convert.ToDateTime(reader[ColCreatedDate])
                    : DateTime.MinValue,
                UpdatedBy = reader["kni_modif_by"]?.ToString() ?? "",
                UpdatedDate = reader[ColModifDate] != DBNull.Value
                    ? Convert.ToDateTime(reader[ColModifDate])
                    : DateTime.MinValue,
                Konsentrasi = reader["kon_singkatan"]?.ToString() ?? "",
                KonsentrasiId = reader["kon_id"]?.ToString() ?? ""
            };

            await reader.CloseAsync();
            kriteria.Details = (await GetDetailsByKriteriaIdAsync(id)).ToList();

            return kriteria;
        }

        private async Task<IEnumerable<KriteriaDetail>> GetDetailsByKriteriaIdAsync(string kriteriaId)
        {
            var details = new List<KriteriaDetail>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getDataKriteriaDetail", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue(ParamKriteriaId, kriteriaId);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                details.Add(new KriteriaDetail
                {
                    Id = reader["knd_id"]?.ToString() ?? "",
                    KriteriaId = kriteriaId,
                    Kriteria = reader["knd_kriteria"]?.ToString() ?? "",
                    Persentase = Convert.ToInt32(reader["knd_prosentase"] ?? 0),
                    CreatedBy = reader["knd_created_by"]?.ToString() ?? "",
                    CreatedDate = reader["knd_created_date"] != DBNull.Value
                        ? Convert.ToDateTime(reader["knd_created_date"])
                        : DateTime.MinValue
                });
            }

            return details;
        }

        public async Task<string> CreateAsync(CreateKriteriaRequest dto, string userId, string userRole)
        {
            await using var conn = new SqlConnection(_conn);
            await conn.OpenAsync();
            await using var transaction = (SqlTransaction)await conn.BeginTransactionAsync();

            try
            {
                await using var cmd = new SqlCommand("sia_createKriteria", conn, transaction)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@MataKuliahId", dto.MataKuliahId);
                cmd.Parameters.AddWithValue(ParamTahunAjaran, dto.TahunAjaran);
                cmd.Parameters.AddWithValue(ParamSemester, dto.Semester);
                cmd.Parameters.AddWithValue(ParamUsername, userId);
                cmd.Parameters.AddWithValue("@Jenis", dto.TipeKriteria);
                cmd.Parameters.AddWithValue("@UserRole", userRole);

                var newId = await cmd.ExecuteScalarAsync();
                var kriteriaId = newId?.ToString() ?? string.Empty;

                if (string.IsNullOrEmpty(kriteriaId))
                    throw new InvalidOperationException("Gagal membuat kriteria");

                foreach (var detail in dto.KriteriaDetails)
                {
                    await using var cmdDetail = new SqlCommand("sia_createKriteriaDetail", conn, transaction)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmdDetail.Parameters.AddWithValue(ParamKriteriaId, kriteriaId);
                    cmdDetail.Parameters.AddWithValue("@Kriteria", detail.Kriteria);
                    cmdDetail.Parameters.AddWithValue("@Persentase", detail.Persentase);
                    cmdDetail.Parameters.AddWithValue(ParamUsername, userId);

                    await cmdDetail.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();
                return kriteriaId;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> UpdateAsync(UpdateKriteriaRequest dto, string userId)
        {
            await using var conn = new SqlConnection(_conn);
            await conn.OpenAsync();
            await using var transaction = (SqlTransaction)await conn.BeginTransactionAsync();

            try
            {
                await using var cmdClear = new SqlCommand("sia_clearKriteriaDetail", conn, transaction)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmdClear.Parameters.AddWithValue(ParamKriteriaId, dto.KriteriaId);
                await cmdClear.ExecuteNonQueryAsync();

                foreach (var detail in dto.KriteriaDetails)
                {
                    await using var cmdDetail = new SqlCommand("sia_createKriteriaDetail", conn, transaction)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmdDetail.Parameters.AddWithValue(ParamKriteriaId, dto.KriteriaId);
                    cmdDetail.Parameters.AddWithValue("@Kriteria", detail.Kriteria);
                    cmdDetail.Parameters.AddWithValue("@Persentase", detail.Persentase);
                    cmdDetail.Parameters.AddWithValue(ParamUsername, userId);

                    await cmdDetail.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<bool> DeleteAsync(string id, string userId)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_deleteKriteria", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@UserId", userId);

            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> KirimAsync(string id, string userId)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_sentKriteria", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue(ParamKriteriaId, id);
            cmd.Parameters.AddWithValue(ParamUsername, userId);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
            return true;
        }

        public async Task<bool> ApproveAsync(string id, string userId)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_approveKriteria", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue(ParamKriteriaId, id);
            cmd.Parameters.AddWithValue(ParamUsername, userId);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
            return true;
        }

        public async Task<bool> RejectAsync(ApproveRejectRequest dto, string userId)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_rejectKriteria", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue(ParamKriteriaId, dto.KriteriaId);
            cmd.Parameters.AddWithValue(ParamUsername, userId);
            cmd.Parameters.AddWithValue("@Alasan", dto.Alasan);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
            return true;
        }

        public async Task<List<LookupDto>> GetListKonsentrasiAsync()
        {
            var list = new List<LookupDto>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getListKonsentrasi", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new LookupDto
                {
                    Id = reader["kon_id"]?.ToString() ?? "",
                    Nama = reader["kon_nama"]?.ToString() ?? ""
                });
            }

            return list;
        }

        public async Task<List<LookupDto>> GetListTahunAjaranAsync()
        {
            var list = new List<LookupDto>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getListTahunAjaran", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new LookupDto
                {
                    Id = reader["kak_tahun_ajaran"]?.ToString() ?? "",
                    Nama = reader["kak_tahun_ajaran"]?.ToString() ?? ""
                });
            }

            return list;
        }

        public async Task<List<LookupDto>> GetListMataKuliahAsync(
            string userId, string userRole, string tahunAjaran, string semester)
        {
            var list = new List<LookupDto>();

            var displayName = await _ldapService.GetDisplayNameAsync(userId) ?? userId;

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getListMataKuliahByKriteria", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue(ParamUsername, userId);
            cmd.Parameters.AddWithValue(ParamTahunAjaran, tahunAjaran);
            cmd.Parameters.AddWithValue(ParamSemester, semester);
            cmd.Parameters.AddWithValue("@DisplayName", displayName);
            cmd.Parameters.AddWithValue("@UserRole", userRole);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new LookupDto
                {
                    Id = reader["mku_id"]?.ToString() ?? "",
                    Nama = reader["mku_nama"]?.ToString() ?? ""
                });
            }

            return list;
        }

        public async Task<bool> CheckTotalPersentaseAsync(string kriteriaId)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_checkKriteria", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue(ParamKriteriaId, kriteriaId);

            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();
            return result?.ToString() == "100";
        }

        public async Task<string?> GetActivePeriodeByTanggalAsync()
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getActivePeriodePenilaianByTanggal", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                var tahunAjaran = reader[0]?.ToString() ?? "";
                var semester = reader[1]?.ToString() ?? "";
                return $"{tahunAjaran}|{semester}";
            }

            return null;
        }

        public async Task<string?> GetActiveTahunAkademikByTanggalAsync()
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getActiveTahunAkademikByTanggal", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                var tahunAjaran = reader[0]?.ToString() ?? "";
                var semester = reader[1]?.ToString() ?? "";
                return $"{tahunAjaran}|{semester}";
            }

            return null;
        }

        public async Task<TemplateKriteriaResponse?> GetTemplateKriteriaAsync(TemplateKriteriaRequest dto, string userRole, string username)
        {
            var response = new TemplateKriteriaResponse
            {
                Tipe = dto.Tipe,
                KriteriaDetails = new List<KriteriaDetailDto>()
            };

            var tipeUpper = dto.Tipe?.ToUpper() ?? "";

            if (tipeUpper == "UUT")
            {
                response.KriteriaDetails = new List<KriteriaDetailDto>
                {
                    new KriteriaDetailDto { Kriteria = "Tugas", Persentase = 30 },
                    new KriteriaDetailDto { Kriteria = "UTS", Persentase = 30 },
                    new KriteriaDetailDto { Kriteria = "UAS", Persentase = 40 }
                };
                return response;
            }

            if (tipeUpper == "KUSTOM")
            {
                response.KriteriaDetails = new List<KriteriaDetailDto>
                {
                    new KriteriaDetailDto { Kriteria = "", Persentase = 0 }
                };
                return response;
            }

            if (tipeUpper == "TAHUNLALU" || tipeUpper == "LAST")
            {
                return await LoadTemplateTahunLaluAsync(dto, response);
            }

            if (tipeUpper == "RPS")
            {
                return null;
            }

            return null;
        }

        private async Task<TemplateKriteriaResponse?> LoadTemplateTahunLaluAsync(TemplateKriteriaRequest dto, TemplateKriteriaResponse response)
        {
            if (string.IsNullOrEmpty(dto.MataKuliahId) || string.IsNullOrEmpty(dto.TahunAjaran))
            {
                return null;
            }


            var tahunAjaranParts = dto.TahunAjaran.Split('/');
            if (tahunAjaranParts.Length != 2 ||
                !int.TryParse(tahunAjaranParts[0], out int startYear) ||
                !int.TryParse(tahunAjaranParts[1], out int endYear))
            {
                return null;
            }


            string prevTahunAjaran = $"{startYear - 1}/{endYear - 1}";


            var details = await LoadDetailsFromSP(dto.MataKuliahId, prevTahunAjaran, dto.Semester ?? "");

            if (details.Count > 0)
            {
                response.KriteriaDetails = details;
                return response;
            }


            return null;
        }

        private async Task<List<KriteriaDetailDto>> LoadDetailsFromSP(string mataKuliahId, string tahunAjaran, string semester)
        {
            var details = new List<KriteriaDetailDto>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getDataKriteriaDetail2", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@MataKuliahId", mataKuliahId);
            cmd.Parameters.AddWithValue("@TahunAjaran", tahunAjaran);
            cmd.Parameters.AddWithValue("@Semester", semester);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                details.Add(new KriteriaDetailDto
                {
                    Id = reader["knd_id"]?.ToString() ?? Guid.NewGuid().ToString(),
                    Kriteria = reader["knd_kriteria"]?.ToString() ?? "",
                    Persentase = reader["knd_prosentase"] != DBNull.Value
                        ? Convert.ToInt32(reader["knd_prosentase"])
                        : 0
                });
            }

            return details;
        }
    }
}