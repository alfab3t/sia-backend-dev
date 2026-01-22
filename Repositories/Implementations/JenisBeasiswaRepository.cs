using astratech_apps_backend.DTOs.JenisBeasiswa;
using astratech_apps_backend.Models;
using astratech_apps_backend.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace astratech_apps_backend.Repositories.Implementations
{
    public class JenisBeasiswaRepository : IJenisBeasiswaRepository
    {
        private readonly string _conn;

        public JenisBeasiswaRepository(IConfiguration config)
        {
            _conn = PolmanAstraLibrary.PolmanAstraLibrary.Decrypt(
                config.GetConnectionString("DefaultConnection")!,
                Environment.GetEnvironmentVariable("DECRYPT_KEY_CONNECTION_STRING")
            );
        }

        private const string PARAM_MASA_SEMESTER = "@MasaSemester";


        public async Task<int> CreateAsync(CreateJenisBeasiswaRequest dto, string createdBy)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_createJenisBeasiswa", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@IbeId", dto.InstitusiId);
            cmd.Parameters.AddWithValue("@NamaJenisBeasiswa", dto.NamaJenisBeasiswa);
            cmd.Parameters.AddWithValue("@MasaSemester", dto.masaSemester);
            cmd.Parameters.AddWithValue("@CreatedBy", createdBy);

            await conn.OpenAsync();
            var newId = await cmd.ExecuteScalarAsync();

            return Convert.ToInt32(newId);
        }



        public async Task<(IEnumerable<JenisBeasiswa>, int totalData)> GetAllAsync(GetAllJenisBeasiswaRequest dto)
        {
            var list = new List<JenisBeasiswa>();
            int totalData = 0;

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getDataJenisBeasiswa", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Keyword", dto.SearchKeyword ?? "");
            cmd.Parameters.AddWithValue("@Status", dto.Status ?? "");
            cmd.Parameters.AddWithValue("@Urut", dto.Urut ?? "JenisBeasiswaId asc");
            cmd.Parameters.AddWithValue("@Halaman", dto.PageNumber);
            cmd.Parameters.AddWithValue("@Limit", dto.PageSize);

            cmd.Parameters.AddWithValue("@MasaSemester",
            dto.MasaSemester.HasValue ? (object)dto.MasaSemester.Value : DBNull.Value);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                totalData = reader.GetInt32(reader.GetOrdinal("Count"));

                do
                {
                    list.Add(new JenisBeasiswa
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("JenisBeasiswaId")),
                        RowNumber = reader.GetInt64(reader.GetOrdinal("rownum")),
                        NamaJenisBeasiswa = reader.GetString(reader.GetOrdinal("Nama Jenis")),
                        NamaInstitusi = reader.GetString(reader.GetOrdinal("Nama Institusi")),
                        masaSemester = reader.GetInt32(reader.GetOrdinal("Masa Semester")),
                        Status = reader.GetString(reader.GetOrdinal("JenisBeasiswaStatus"))
                    });
                }
                while (await reader.ReadAsync());
            }

            return (list, totalData);
        }

        public async Task<(IEnumerable<JenisBeasiswa>, int totalData)> GetAllDropdownAsync(GetAllJenisBeasiswaDropdownRequest dto)
        {
            var list = new List<JenisBeasiswa>();
            int totalData = 0;

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getDataJenisBeasiswa", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Keyword", dto.SearchKeyword ?? "");
            cmd.Parameters.AddWithValue("@Status", dto.Status ?? "");
            cmd.Parameters.AddWithValue("@Urut", dto.Urut ?? "JenisBeasiswaId asc");
            cmd.Parameters.AddWithValue("@Halaman", dto.PageNumber);
            cmd.Parameters.AddWithValue("@Limit", dto.PageSize);
            cmd.Parameters.AddWithValue(PARAM_MASA_SEMESTER,
            dto.MasaSemester.HasValue ? (object)dto.MasaSemester.Value : DBNull.Value);


            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                totalData = reader.GetInt32(reader.GetOrdinal("Count"));

                do
                {
                    list.Add(new JenisBeasiswa
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("JenisBeasiswaId")),
                        RowNumber = reader.GetInt64(reader.GetOrdinal("rownum")),
                        NamaJenisBeasiswa = reader.GetString(reader.GetOrdinal("Nama Jenis")),
                        NamaInstitusi = reader.GetString(reader.GetOrdinal("Nama Institusi")),
                        masaSemester = reader.GetInt32(reader.GetOrdinal("Masa Semester")),
                        Status = reader.GetString(reader.GetOrdinal("JenisBeasiswaStatus"))
                    });
                }
                while (await reader.ReadAsync());
            }

            return (list, totalData);
        }




        public async Task<JenisBeasiswa?> GetByIdAsync(short id)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_detailJenisBeasiswa", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@IdJenis", id);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new JenisBeasiswa
                {
                    Id = reader.GetInt32(reader.GetOrdinal("jenisId")),
                    NamaJenisBeasiswa = reader.GetString(reader.GetOrdinal("jenisNama")),
                    NamaInstitusi = reader.GetString(reader.GetOrdinal("institusiNama")),
                    masaSemester = reader.GetInt32(reader.GetOrdinal("jenisMasaSemester")),
                    institusiId = reader.GetInt32(reader.GetOrdinal("institusiId")),
                    Status = reader.GetString(reader.GetOrdinal("jenisStatus")),
                };
            }

            return null;
        }



        public async Task<bool> SetStatusAsync(short id, string updatedBy)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_setStatusJenisBeasiswa", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@UpdatedBy", updatedBy);

            await conn.OpenAsync();
            var rowsAffected = await cmd.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }




        public async Task<bool> UpdateAsync(UpdateJenisBeasiswaRequest dto, string updatedBy)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_editJenisBeasiswa", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Id", dto.Id);
            cmd.Parameters.AddWithValue("@InstitusiId", dto.InstitusiId);
            cmd.Parameters.AddWithValue("@NamaJenisBeasiswa", dto.NamaJenisBeasiswa);
            cmd.Parameters.AddWithValue(PARAM_MASA_SEMESTER,
            dto.masaSemester.HasValue ? (object)dto.masaSemester.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@UpdatedBy", updatedBy);

            await conn.OpenAsync();
            var rowsAffected = await cmd.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }
    }
}
