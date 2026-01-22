using astratech_apps_backend.DTOs.InstitusiBeasiswa;
using astratech_apps_backend.Models;
using astratech_apps_backend.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace astratech_apps_backend.Repositories.Implementations
{
    public class InstitusiBeasiswaRepository(IConfiguration config) : IInstitusiBeasiswaRepository
    {
        private readonly string _conn = PolmanAstraLibrary.PolmanAstraLibrary.Decrypt(config.GetConnectionString("DefaultConnection")!, Environment.GetEnvironmentVariable("DECRYPT_KEY_CONNECTION_STRING"));

        public async Task<int> CreateAsync(CreateInstitusiBeasiswaRequest dto, string createdBy)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_createInstitusiBeasiswa", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@NamaInstitusiBeasiswa", dto.NamaInstitusiBeasiswa);
            cmd.Parameters.AddWithValue("@Alamat", dto.Alamat);
            cmd.Parameters.AddWithValue("@Telepon", dto.Telepon);
            cmd.Parameters.AddWithValue("@Email", dto.Email);
            cmd.Parameters.AddWithValue("@CreatedBy", createdBy);

            await conn.OpenAsync();
            var newId = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(newId);
        }



        public async Task<(IEnumerable<InstitusiBeasiswa>, int totalData)> GetAllAsync(GetAllInstitusiBeasiswaRequest dto)
        {
            var list = new List<InstitusiBeasiswa>();
            int totalData = 0;

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getDataInstitusiBeasiswa", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Keyword", dto.SearchKeyword);
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
                    list.Add(new InstitusiBeasiswa
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("institusiId")),
                        RowNumber = reader.GetInt64(reader.GetOrdinal("rownum")),
                        NamaInstitusiBeasiswa = reader.GetString(reader.GetOrdinal("institusiNama")),
                        Alamat = reader.GetString(reader.GetOrdinal("institusiAlamat")),
                        Telepon = reader.GetString(reader.GetOrdinal("institusiTelp")),
                        Email = reader.GetString(reader.GetOrdinal("institusiEmail")),
                        Status = reader.GetString(reader.GetOrdinal("institusiStatus"))
                    });
                }
                while (await reader.ReadAsync());
            }

            return (list, totalData);
        }

        public async Task<(IEnumerable<InstitusiBeasiswa>, int totalData)> GetAllDropDownAsync(GetAllInstitusiBeasiswaDropdownRequest dto)
        {
            var list = new List<InstitusiBeasiswa>();
            int totalData = 0;

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getDataInstitusiBeasiswa", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Keyword", dto.SearchKeyword);
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
                    list.Add(new InstitusiBeasiswa
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("institusiId")),
                        RowNumber = reader.GetInt64(reader.GetOrdinal("rownum")),
                        NamaInstitusiBeasiswa = reader.GetString(reader.GetOrdinal("institusiNama")),
                        Alamat = reader.GetString(reader.GetOrdinal("institusiAlamat")),
                        Telepon = reader.GetString(reader.GetOrdinal("institusiTelp")),
                        Email = reader.GetString(reader.GetOrdinal("institusiEmail")),
                        Status = reader.GetString(reader.GetOrdinal("institusiStatus"))
                    });
                }
                while (await reader.ReadAsync());
            }

            return (list, totalData);
        }

        public async Task<InstitusiBeasiswa?> GetByIdAsync(short id)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_detailInstitusiBeasiswa", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@IdInstitusi", id);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new InstitusiBeasiswa
                {
                    Id = reader.GetInt32(reader.GetOrdinal("institusiId")),
                    NamaInstitusiBeasiswa = reader.GetString(reader.GetOrdinal("institusiNama")),
                    Alamat = reader.GetString(reader.GetOrdinal("institusiAlamat")),
                    Telepon = reader.GetString(reader.GetOrdinal("institusiTelp")),
                    Email = reader.GetString(reader.GetOrdinal("institusiEmail")),
                    Status = reader.GetString(reader.GetOrdinal("institusiStatus")),
                };
            }

            return null;
        }


        public async Task<bool> SetStatusAsync(short id, string updatedBy)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_setStatusInstitusiBeasiswa", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@IdInstitusi", id);
            cmd.Parameters.AddWithValue("@UpdatedBy", updatedBy);

            await conn.OpenAsync();
            var rowsAffected = await cmd.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }


        public async Task<bool> UpdateAsync(UpdateInstitusiBeasiswaRequest dto, string updatedBy)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_editInstitusiBeasiswa", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Id", dto.Id);
            cmd.Parameters.AddWithValue("@NamaInstitusi", dto.NamaInstitusiBeasiswa);
            cmd.Parameters.AddWithValue("@Alamat", dto.Alamat);
            cmd.Parameters.AddWithValue("@Telepon", dto.Telepon);
            cmd.Parameters.AddWithValue("@Email", dto.Email);
            cmd.Parameters.AddWithValue("@UpdatedBy", updatedBy);

            await conn.OpenAsync();
            var rowsAffected = await cmd.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }


    }
}
