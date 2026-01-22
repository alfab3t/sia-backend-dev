using astratech_apps_backend.DTOs.SkalaPenilaian;
using astratech_apps_backend.Models;
using astratech_apps_backend.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace astratech_apps_backend.Repositories.Implementations
{
    public class SkalaPenilaianRepository(IConfiguration config) : ISkalaPenilaianRepository
    {
        private readonly string _conn = PolmanAstraLibrary.PolmanAstraLibrary.Decrypt(config.GetConnectionString("DefaultConnection")!, Environment.GetEnvironmentVariable("DECRYPT_KEY_CONNECTION_STRING"));

        public async Task<(IEnumerable<SkalaPenilaian>, int totalData)> GetAllAsync(GetAllSkalaPenilaianRequest dto)
        {
            var list = new List<SkalaPenilaian>();
            int totalData = 0;

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getDataSkalaPenilaian", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@SearchKeyword", dto.SearchKeyword);
            cmd.Parameters.AddWithValue("@Status", dto.Status);
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
                    list.Add(new SkalaPenilaian
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("spe_id")),
                        RowNumber = reader.GetInt64(reader.GetOrdinal("rownum")),
                        Skala = reader.GetInt32(reader.GetOrdinal("spe_skala")),
                        Definisi = reader.GetString(reader.GetOrdinal("spe_deskripsi")),
                        Status = reader.GetString(reader.GetOrdinal("spe_status"))
                    });
                } while (await reader.ReadAsync());
            }
            return (list, totalData);
        }

        public async Task<SkalaPenilaian?> GetByIdAsync(int id)
        {
            await using var conn = new SqlConnection(_conn);

            await using var cmd = new SqlCommand("sia_detailSkalaPenilaian", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Id", id);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new SkalaPenilaian
                {
                    Id = id,
                    Skala = reader.GetInt32(reader.GetOrdinal("spe_skala")),
                    Definisi = reader.GetString(reader.GetOrdinal("spe_deskripsi")),
                };
            }
            return null;
        }
        public async Task<int> CreateAsync(CreateSkalaPenilaianRequest dto, string createdBy)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_createSkalaPenilaian", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Skala", dto.Skala);
            cmd.Parameters.AddWithValue("@Definisi", dto.Definisi);
            cmd.Parameters.AddWithValue("@CreatedBy", createdBy);

            await conn.OpenAsync();
            var newId = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(newId);
        }

        public async Task<bool> UpdateAsync(UpdateSkalaPenilaianRequest dto, string updatedBy)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_editSkalaPenilaian", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Id", dto.Id);
            cmd.Parameters.AddWithValue("@Definisi", dto.Definisi);
            cmd.Parameters.AddWithValue("@UpdatedBy", updatedBy);

            await conn.OpenAsync();
            var rowsAffected = await cmd.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<bool> SetStatusAsync(int id, string newStatus, string updatedBy)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_setStatusSkalaPenilaian", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Status", newStatus);
            cmd.Parameters.AddWithValue("@Id", id);

            await conn.OpenAsync();
            var rowsAffected = await cmd.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }
    }
}