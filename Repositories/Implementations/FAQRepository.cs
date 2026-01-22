using astratech_apps_backend.DTOs.Bantuan.FAQ;
using astratech_apps_backend.Models;
using astratech_apps_backend.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace astratech_apps_backend.Repositories.Implementations
{
    public class FAQRepository(IConfiguration config) : IFAQRepository
    {
        private readonly string _conn = PolmanAstraLibrary.PolmanAstraLibrary.Decrypt(config.GetConnectionString("DefaultConnection")!, Environment.GetEnvironmentVariable("DECRYPT_KEY_CONNECTION_STRING"));

        public async Task<int> CreateAsync(CreateFAQRequest dto, string createdBy, string creatorRoleId)
        {
            await using var conn = new SqlConnection(_conn);
            await conn.OpenAsync();
            await using var transaction = conn.BeginTransaction();

            try
            {
                var cmdFaq = new SqlCommand("sia_createBantuanFaq", conn, transaction) { CommandType = CommandType.StoredProcedure };

                cmdFaq.Parameters.AddWithValue("@BkaId", dto.KategoriId);
                cmdFaq.Parameters.AddWithValue("@Pertanyaan", dto.Pertanyaan);
                cmdFaq.Parameters.AddWithValue("@Jawaban", dto.Jawaban);
                cmdFaq.Parameters.AddWithValue("@CreatedBy", createdBy);

                var newFaqId = Convert.ToInt32(await cmdFaq.ExecuteScalarAsync());
                if (newFaqId <= 0) throw new Exception("Gagal membuat data FAQ di database.");

                var rolesToInsert = new HashSet<string>(dto.AksesRoleIds ?? []);

                if (!string.IsNullOrEmpty(creatorRoleId))
                {
                    rolesToInsert.Add(creatorRoleId);
                }

                foreach (var roleId in rolesToInsert)
                {
                    var cmdAkses = new SqlCommand("sia_createBantuanAkses", conn, transaction) { CommandType = CommandType.StoredProcedure };
                    cmdAkses.Parameters.AddWithValue("@PanduanId", DBNull.Value);
                    cmdAkses.Parameters.AddWithValue("@FaqId", newFaqId);
                    cmdAkses.Parameters.AddWithValue("@RolId", roleId);
                    cmdAkses.Parameters.AddWithValue("@CreatedBy", createdBy);
                    await cmdAkses.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();
                return newFaqId;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> UpdateAsync(UpdateFAQRequest dto, string updatedBy, string currentRoleId)
        {
            await using var conn = new SqlConnection(_conn);
            await conn.OpenAsync();
            await using var transaction = conn.BeginTransaction();

            try
            {
                var cmdFaq = new SqlCommand("sia_editBantuanFaq", conn, transaction) { CommandType = CommandType.StoredProcedure };
                cmdFaq.Parameters.AddWithValue("@BkaId", dto.KategoriId);
                cmdFaq.Parameters.AddWithValue("@FaqId", dto.FaqId);
                cmdFaq.Parameters.AddWithValue("@Pertanyaan", dto.Pertanyaan);
                cmdFaq.Parameters.AddWithValue("@Jawaban", dto.Jawaban);
                cmdFaq.Parameters.AddWithValue("@Status", dto.Status);
                cmdFaq.Parameters.AddWithValue("@ModifiedBy", updatedBy);
                await cmdFaq.ExecuteNonQueryAsync();

                var cmdDeleteAkses = new SqlCommand("sia_deleteBantuanAksesByFaqId", conn, transaction) { CommandType = CommandType.StoredProcedure };
                cmdDeleteAkses.Parameters.AddWithValue("@FaqId", dto.FaqId);
                cmdDeleteAkses.Parameters.AddWithValue("@DeletedBy", updatedBy);
                await cmdDeleteAkses.ExecuteNonQueryAsync();

                var rolesToInsert = new HashSet<string>(dto.AksesRoleIds ?? []);

                if (!string.IsNullOrEmpty(currentRoleId))
                {
                    rolesToInsert.Add(currentRoleId);
                }

                foreach (var roleId in rolesToInsert)
                {
                    var cmdAkses = new SqlCommand("sia_createBantuanAkses", conn, transaction) { CommandType = CommandType.StoredProcedure };
                    cmdAkses.Parameters.AddWithValue("@PanduanId", DBNull.Value);
                    cmdAkses.Parameters.AddWithValue("@FaqId", dto.FaqId);
                    cmdAkses.Parameters.AddWithValue("@RolId", roleId);
                    cmdAkses.Parameters.AddWithValue("@CreatedBy", updatedBy);
                    await cmdAkses.ExecuteNonQueryAsync();
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

        public async Task<bool> DeleteAsync(int id, string deletedBy)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_deleteBantuanFaq", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@FaqId", id);
            cmd.Parameters.AddWithValue("@DeletedBy", deletedBy);

            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result) > 0;
        }

        public async Task<FAQ?> GetByIdAsync(int id, string userRoleId)
        {
            FAQ? data = null;

            await using var conn = new SqlConnection(_conn);
            await conn.OpenAsync();

            await using (var cmd = new SqlCommand("sia_getDataBantuanFaqById", conn) { CommandType = CommandType.StoredProcedure })
            {
                cmd.Parameters.AddWithValue("@FaqId", id);
                cmd.Parameters.AddWithValue("@RolId", userRoleId);

                await using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    data = new FAQ
                    {
                        FaqId = reader.GetInt32(reader.GetOrdinal("bfa_id")),

                        KategoriId = reader.IsDBNull(reader.GetOrdinal("bka_id"))
                                     ? null
                                     : reader.GetInt32(reader.GetOrdinal("bka_id")),

                        Pertanyaan = reader.GetString(reader.GetOrdinal("bfa_pertanyaan")),
                        Jawaban = reader.GetString(reader.GetOrdinal("bfa_jawaban")),
                        Status = reader.GetString(reader.GetOrdinal("bfa_status")),
                        AksesRoleIds = []
                    };
                }
                if (data == null) return null;
                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        if (!reader.IsDBNull(0))
                        {
                            data.AksesRoleIds.Add(reader.GetString(0));
                        }
                    }
                }
            }

            return data;
        }

        public async Task<(IEnumerable<FAQ>, int totalData)> GetAllAsync(GetAllFAQRequest dto, string userRoleId, string username)
        {
            var list = new List<FAQ>();
            int totalData = 0;

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getDataAllBantuanFaqs", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@SearchKeyword", dto.SearchKeyword ?? "");
            cmd.Parameters.AddWithValue("@Status", dto.Status ?? "");
            cmd.Parameters.AddWithValue("@OrderBy", dto.Urut ?? "urutan_asc");
            cmd.Parameters.AddWithValue("@PageNumber", dto.PageNumber);
            cmd.Parameters.AddWithValue("@PageSize", dto.PageSize);
            cmd.Parameters.AddWithValue("@RolId", userRoleId);
            cmd.Parameters.AddWithValue("@KategoriId", dto.KategoriId);
            cmd.Parameters.AddWithValue("@username", username);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                totalData = reader.GetInt32(reader.GetOrdinal("TotalCount"));

                do
                {
                    list.Add(new FAQ
                    {
                        FaqId = reader.GetInt32(reader.GetOrdinal("bfa_id")),
                        Jawaban = reader.GetString(reader.GetOrdinal("bfa_jawaban")),
                        NamaKategori = reader.IsDBNull(reader.GetOrdinal("bka_nama"))
                                       ? "-"
                                       : reader.GetString(reader.GetOrdinal("bka_nama")),

                        Pertanyaan = reader.GetString(reader.GetOrdinal("bfa_pertanyaan")),
                        Status = reader.GetString(reader.GetOrdinal("bfa_status")),
                    });
                } while (await reader.ReadAsync());
            }

            return (list, totalData);
        }

        public async Task<IEnumerable<RoleOptionFAQ>> GetRolesAsync()
        {
            var spName = "sso_getListRole";
            var roles = new List<RoleOptionFAQ>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand(spName, conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@p1", "");

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                roles.Add(new RoleOptionFAQ
                {
                    Value = reader.GetValue(0).ToString() ?? "",
                    Text = reader.GetValue(1).ToString() ?? ""
                });
            }
            return roles;
        }
    }
}

