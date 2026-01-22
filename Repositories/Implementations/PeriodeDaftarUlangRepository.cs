using astratech_apps_backend.DTOs.PeriodeDaftarUlang;
using astratech_apps_backend.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;
using astratech_apps_backend.Models;

namespace astratech_apps_backend.Repositories.Implementations
{
    public class PeriodeDaftarUlangRepository(IConfiguration config) : IPeriodeDaftarUlangRepository
    {
        private readonly string _conn = PolmanAstraLibrary.PolmanAstraLibrary.Decrypt(config.GetConnectionString("DefaultConnection")!, Environment.GetEnvironmentVariable("DECRYPT_KEY_CONNECTION_STRING"));

        public async Task<(IEnumerable<PeriodeDaftarUlang>, int totalData)> GetAllAsync(GetAllPeriodeDaftarUlangRequest dto)
        {
            var list = new List<PeriodeDaftarUlang>();
            int totalData = 0;

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getDataPeriodeDaftarUlang", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Keyword", dto.SearchKeyword);
            cmd.Parameters.AddWithValue("@Urut", dto.Urut);
            cmd.Parameters.AddWithValue("@Halaman", dto.PageNumber);
            cmd.Parameters.AddWithValue("@Limit", dto.PageSize);
            
            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                totalData = reader.GetInt32(reader.GetOrdinal("Count"));
                do
                {
                    list.Add(new PeriodeDaftarUlang
                    {
                        RowNumber = reader.GetInt64(reader.GetOrdinal("rownum")),
                        Id = reader.GetInt32(reader.GetOrdinal("pdu_id")),
                        TahunAjaran = reader.GetString(reader.GetOrdinal("pdu_tahun_ajaran")),
                        TanggalMulai = reader.GetDateTime(reader.GetOrdinal("pdu_tgl_from")),
                        TanggalAkhir = reader.GetDateTime(reader.GetOrdinal("pdu_tgl_until"))
                    });
                } while (await reader.ReadAsync());
            }

            return (list, totalData);
        }

        public async Task<PeriodeDaftarUlang?> GetByIdAsync(int id)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getPeriodeDaftarUlangById", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            
            cmd.Parameters.AddWithValue("@IdPDU", id);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new PeriodeDaftarUlang
                {
                    Id = reader.GetInt32(reader.GetOrdinal("pdu_id")),
                    TahunAjaran = reader.GetString(reader.GetOrdinal("pdu_tahun_ajaran")),
                    TanggalMulai = reader.GetDateTime(reader.GetOrdinal("pdu_tgl_from")),
                    TanggalAkhir = reader.GetDateTime(reader.GetOrdinal("pdu_tgl_until"))
                };
            }

            return null;
        }

        public async Task<bool> CreateAsync(CreatePeriodeDaftarUlangRequest dto, string createdBy)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_createPeriodeDaftarUlang", conn) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.AddWithValue("@TahunAjaran", dto.TahunAjaran);
            cmd.Parameters.AddWithValue("@TanggalMulai", dto.TanggalMulai);
            cmd.Parameters.AddWithValue("@TanggalAkhir", dto.TanggalAkhir);
            cmd.Parameters.AddWithValue("@CreatedBy", createdBy);

            await conn.OpenAsync();
            int rowsAffected = await cmd.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<bool> CheckPeriodeDaftarUlangAsync(string tahunAjaran)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_checkPeriodeDaftarUlang", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@TahunAjaran", tahunAjaran);

            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();
            return result?.ToString() == "OK";
        }

        public async Task<IEnumerable<TahunAjaranDto>> GetListTahunAjaranAsync()
        {
            var list = new List<TahunAjaranDto>();
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getListTahunAjaran", conn) { CommandType = CommandType.StoredProcedure };

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new TahunAjaranDto
                {
                    TahunAjaran = reader.GetString(reader.GetOrdinal("kak_tahun_ajaran"))
                });
            }
            return list;
        }

        public async Task<bool> UpdateAsync(UpdatePeriodeDaftarUlangRequest dto, string updatedBy)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_editPeriodeDaftarUlang", conn) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.AddWithValue("@Id", dto.Id);
            cmd.Parameters.AddWithValue("@TanggalMulai", dto.TanggalMulai);
            cmd.Parameters.AddWithValue("@TanggalAkhir", dto.TanggalAkhir);
            cmd.Parameters.AddWithValue("@UpdatedBy", updatedBy);

            await conn.OpenAsync();
            int rowsAffected = await cmd.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
    }
}