namespace astratech_apps_backend.Repositories.Implementations;

using astratech_apps_backend.DTOs.Jurusan;
using astratech_apps_backend.Models;
using astratech_apps_backend.Repositories.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;
using System.Data;

public class JurusanRepository(IConfiguration config) : IJurusanRepository
{
    private readonly string _conn = PolmanAstraLibrary.PolmanAstraLibrary.Decrypt(config.GetConnectionString("DefaultConnection")!, Environment.GetEnvironmentVariable("DECRYPT_KEY_CONNECTION_STRING"));

    public async Task<int> CreateAsync(CreateJurusanRequest dto, string createdBy)
    {
        await using var conn = new SqlConnection(_conn);
        await using var cmd = new SqlCommand("sia_createJurusan", conn)
        {
            CommandType = CommandType.StoredProcedure
        };
        cmd.Parameters.AddWithValue("@NamaJurusan", dto.NamaJurusan);
        cmd.Parameters.AddWithValue("@KepalaJurusan", dto.KepalaJurusan);
        cmd.Parameters.AddWithValue("@NoNPK", dto.NoNPK);
        cmd.Parameters.AddWithValue("@Deskripsi", dto.JurusanDeskripsi);
        cmd.Parameters.AddWithValue("@CreatedBy", createdBy);

        await conn.OpenAsync();
        var newId = await cmd.ExecuteScalarAsync();
        return Convert.ToInt32(newId);
    }

    public async Task<(IEnumerable<Jurusan>, int totalData)> GetAllAsync(GetAllJurusanRequest dto)
    {
        var list = new List<Jurusan>();
        int totalData = 0;

        await using var conn = new SqlConnection(_conn);
        await using var cmd = new SqlCommand("sia_getDataJurusan", conn)
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
                list.Add(new Jurusan
                {
                    Id = (short)reader.GetInt32(reader.GetOrdinal("jur_id")),
                    RowNumber = reader.GetInt64(reader.GetOrdinal("rownum")),
                    NamaJurusan = reader.GetString(reader.GetOrdinal("jur_nama")),
                    NamaKepalaJurusan = reader.GetString(reader.GetOrdinal("jur_kajur")),
                    NoNPK = reader.GetString(reader.GetOrdinal("jur_npk")),
                    Deskripsi = reader.GetString(reader.GetOrdinal("jur_deskripsi")),
                    JumlahProdi = reader.GetInt32(reader.GetOrdinal("jumlahProdi")),
                    Status = reader.GetString(reader.GetOrdinal("jur_status")),
                });
            } while (await reader.ReadAsync());
        }
        return (list, totalData);
    }

    public async Task<Jurusan?> GetByIdAsync(short id)
    {
        var jurusan = new Jurusan();

        await using var conn = new SqlConnection(_conn);
        await conn.OpenAsync();

        await using (var cmd = new SqlCommand("sia_detailJurusan", conn)
        {
            CommandType = CommandType.StoredProcedure
        })
        {
            cmd.Parameters.AddWithValue("@Id", id);

            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                jurusan.Id = (short)reader.GetInt32(reader.GetOrdinal("jur_id"));
                jurusan.NamaJurusan = reader.GetString(reader.GetOrdinal("jur_nama"));
                jurusan.NamaKepalaJurusan = reader.GetString(reader.GetOrdinal("jur_kajur"));
                jurusan.NoNPK = reader.GetString(reader.GetOrdinal("jur_npk"));
                jurusan.Deskripsi = reader.GetString(reader.GetOrdinal("jur_deskripsi"));
                jurusan.Status = reader.GetString(reader.GetOrdinal("jur_status"));
            }
            else
            {
                return null;
            }
        }

        await using (var cmd = new SqlCommand("sia_getDataJurusanProdi", conn)
        {
            CommandType = CommandType.StoredProcedure
        })
        {
            cmd.Parameters.AddWithValue("@JurusanId", id.ToString());

            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                jurusan.ListProdi.Add(new ListJurusanProdi
                {
                    RowNumber = reader.GetInt64(reader.GetOrdinal("rownum")),
                    NamaProdi = reader.GetString(reader.GetOrdinal("pro_nama")),
                });
            }
        }

        return jurusan;
    }

    public async Task<IEnumerable<Jurusan>> GetListJurusanAsync()
    {
        var list = new List<Jurusan>();

        await using var conn = new SqlConnection(_conn);
        await conn.OpenAsync();

        await using var cmd = new SqlCommand("sia_getListJurusan", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        await using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            list.Add(new Jurusan
            {
                Id = (short)reader.GetInt32(reader.GetOrdinal("jur_id")),
                NamaJurusan = reader.GetString(reader.GetOrdinal("jur_nama"))
            });
        }

        return list;
    }

    public async Task<bool> SetStatusAsync(short id, string updatedBy)
    {
        await using var conn = new SqlConnection(_conn);
        await using var cmd = new SqlCommand("sia_setStatusJurusan", conn)
        {
            CommandType = CommandType.StoredProcedure
        };
        cmd.Parameters.AddWithValue("@Id", id);
        cmd.Parameters.AddWithValue("@UpdatedBy", updatedBy);

        await conn.OpenAsync();
        var rowsAffected = await cmd.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public async Task<bool> UpdateAsync(UpdateJurusanRequest dto, string updatedBy)
    {
        await using var conn = new SqlConnection(_conn);
        await using var cmd = new SqlCommand("sia_editJurusan", conn)
        {
            CommandType = CommandType.StoredProcedure   
        };
        cmd.Parameters.AddWithValue("@Id", dto.Id);
        cmd.Parameters.AddWithValue("@NamaJurusan", dto.NamaJurusan);
        cmd.Parameters.AddWithValue("@KepalaJurusan", dto.KepalaJurusan);
        cmd.Parameters.AddWithValue("@NoNPK", dto.NoNPK);
        cmd.Parameters.AddWithValue("@Deskripsi", dto.JurusanDeskripsi);
        cmd.Parameters.AddWithValue("@UpdatedBy", updatedBy);

        await conn.OpenAsync();
        var rowsAffected = await cmd.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public async Task<List<KaryawanDto>> GetListKaryawanAsync()
    {
        var result = new List<KaryawanDto>();

        await using var conn = new SqlConnection(_conn);
        await using var cmd = new SqlCommand("sia_getListKaryawan", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        await conn.OpenAsync();
        await using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            result.Add(new KaryawanDto
            {
                KaryawanId = reader["kry_id"].ToString() ?? string.Empty,
                KaryawanNama = reader["kry_nama"].ToString() ?? string.Empty,
                Nama = reader["nama"].ToString() ?? string.Empty
            });
        }

        return result;
    }
}