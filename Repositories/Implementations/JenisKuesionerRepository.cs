using astratech_apps_backend.DTOs.JenisKuesioner;
using astratech_apps_backend.Models;
using astratech_apps_backend.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace astratech_apps_backend.Repositories.Implementations
{
    public class JenisKuesionerRepository(IConfiguration config) : IJenisKuesionerRepository
    {
        private readonly string _conn = PolmanAstraLibrary.PolmanAstraLibrary.Decrypt(config.GetConnectionString("DefaultConnection")!, Environment.GetEnvironmentVariable("DECRYPT_KEY_CONNECTION_STRING"));

        public async Task<(IEnumerable<JenisKuesioner>, int totalData)> GetAllAsync(GetAllJenisKuesionerRequest dto)
        {
            var list = new List<JenisKuesioner>();
            int totalData = 0;

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getDataJenisKuesioner", conn)
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
                    list.Add(new JenisKuesioner
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("jku_id")),
                        RowNumber = reader.GetInt64(reader.GetOrdinal("rownum")),
                        NamaJenis = reader.GetString(reader.GetOrdinal("jku_nama")),
                        Status = reader.GetString(reader.GetOrdinal("jku_status")),
                    });
                } while (await reader.ReadAsync());
            }
            return (list, totalData);
        }

        public async Task<JenisKuesioner?> GetByIdAsync(int id)
        {
            await using var conn = new SqlConnection(_conn);

            await using var cmd = new SqlCommand("sia_detailJenisKuesioner", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Id", id);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new JenisKuesioner
                {
                    Id = id,
                    NamaJenis = reader.GetString(reader.GetOrdinal("jku_nama")),
                };
            }

            return null;
        }
        public async Task<int> CreateAsync(CreateJenisKuesionerRequest dto, string createdBy)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_createJenisKuesioner", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@NamaJenis", dto.NamaJenis);
            cmd.Parameters.AddWithValue("@CreatedBy", createdBy);

            await conn.OpenAsync();
            var newId = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(newId);
        }

        public async Task<bool> SetStatusAsync(int id, string newStatus, string updatedBy)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_setStatusJenisKuesioner", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Status", newStatus);
            cmd.Parameters.AddWithValue("@Id", id);

            await conn.OpenAsync();
            var rowsAffected = await cmd.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> UpdateAsync(UpdateJenisKuesionerRequest dto, string updatedBy)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_editJenisKuesioner", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Id", dto.Id);
            cmd.Parameters.AddWithValue("@NamaJenis", dto.NamaJenis);
            cmd.Parameters.AddWithValue("@UpdatedBy", updatedBy);

            await conn.OpenAsync();
            var rowsAffected = await cmd.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }


    }
}