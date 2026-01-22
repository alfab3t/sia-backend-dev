using astratech_apps_backend.DTOs.JenisSurat;
using astratech_apps_backend.Models;
using astratech_apps_backend.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace astratech_apps_backend.Repositories.Implementations
{
    public class JenisSuratRepository(IConfiguration configuration) : IJenisSuratRepository
    {
        private readonly string _conn = PolmanAstraLibrary.PolmanAstraLibrary.Decrypt(
                configuration.GetConnectionString("DefaultConnection")!,
                Environment.GetEnvironmentVariable("DECRYPT_KEY_CONNECTION_STRING") ?? "PoliteknikAstra_ConfigurationKey"
            );

        public async Task<(IEnumerable<JenisSurat>, int totalData)> GetAllAsync(GetAllJenisSuratRequest dto)
        {
            var list = new List<JenisSurat>();
            int totalData = 0;

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getDataJenisSurat", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Keyword", dto.SearchKeyword);
            cmd.Parameters.AddWithValue("@Status", dto.Status);
            cmd.Parameters.AddWithValue("@Urut", dto.Urut);
            cmd.Parameters.AddWithValue("@AllowMahasiswa", dto.AllowMahasiswa);
            cmd.Parameters.AddWithValue("@Halaman", dto.PageNumber);
            cmd.Parameters.AddWithValue("@Limit", dto.PageSize);
            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                totalData = reader.GetInt32(reader.GetOrdinal("Count"));

                do
                {
                    list.Add(new JenisSurat
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("jsu_id")),
                        NamaSurat = reader.GetString("jsu_nama_surat"),
                        FormatNoSurat = reader.GetString("jsu_format_no"),
                        FormatSurat = reader.GetString("jsu_format_surat"),
                        AllowMahasiswa = reader.GetInt32("jsu_allow_mahasiswa"),
                        Status = reader.GetString("jsu_status"),
                    });
                } while (await reader.ReadAsync());
            }

            return (list, totalData);
        }

        public async Task<JenisSurat?> GetByIdAsync(int id)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_detailJenisSurat", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@p1", id);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new JenisSurat
                {
                    Id = reader.GetInt32(reader.GetOrdinal("jsu_id")),
                    NamaSurat = reader.GetString(reader.GetOrdinal("jsu_nama_surat")),
                    FormatNoSurat = reader.GetString(reader.GetOrdinal("jsu_format_no")),
                    FormatSurat = reader.GetString(reader.GetOrdinal("jsu_format_surat")),
                    AllowMahasiswa = reader.GetInt32("jsu_allow_mahasiswa"),
                };
            }
            return null;
        }

        public async Task<bool> SetStatusAsync(int id, string updatedBy)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_setStatusJenisSurat", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@UpdatedBy", updatedBy);

            await conn.OpenAsync();

            var rowsAffected = await cmd.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> UpdateAsync(UpdateJenisSuratRequest dto, string updatedBy)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_editJenisSurat", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Id", dto.Id);
            cmd.Parameters.AddWithValue("@NamaSurat", dto.NamaSurat); 
            cmd.Parameters.AddWithValue("@FormatNoSurat", dto.FormatNoSurat);
            cmd.Parameters.AddWithValue("@AllowMahasiswa", dto.AllowMahasiswa);
            cmd.Parameters.AddWithValue("@UpdatedBy", updatedBy);
            await conn.OpenAsync();

            var rowsAffected = await cmd.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }
    }
}