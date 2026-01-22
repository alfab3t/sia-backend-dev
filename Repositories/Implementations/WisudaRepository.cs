using astratech_apps_backend.DTOs.Wisuda;
using astratech_apps_backend.Models;
using astratech_apps_backend.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace astratech_apps_backend.Repositories.Implementations
{
    public class WisudaRepository(IConfiguration config) : IWisudaRepository
    {
        private readonly string _conn =
            PolmanAstraLibrary.PolmanAstraLibrary.Decrypt(
                config.GetConnectionString("DefaultConnection")!,
                Environment.GetEnvironmentVariable("DECRYPT_KEY_CONNECTION_STRING")
            );

        
        public async Task<(IEnumerable<Wisuda>, int totalData)> GetAllAsync(GetAllWisudaRequest dto)
        {
            var list = new List<Wisuda>();
            int totalData = 0;

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getDataTahunLulusan", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Halaman", dto.PageNumber);
            cmd.Parameters.AddWithValue("@Limit", dto.PageSize);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                totalData = reader.GetInt32(reader.GetOrdinal("Count"));

                do
                {
                    list.Add(new Wisuda
                    {
                        RowNumber = reader.GetInt64(reader.GetOrdinal("rownum")),
                        TahunLulus = reader.GetInt32(reader.GetOrdinal("tahun_lulus")),
                        JumlahLulusan = reader.GetInt32(reader.GetOrdinal("jumlah_lulusan")),
                        JumlahTerima = reader.GetInt32(reader.GetOrdinal("jumlah_terima")),
                    });
                }
                while (await reader.ReadAsync());
            }

            return (list, totalData);
        }

      
        public async Task<(IEnumerable<Wisuda>, int totalData)> GetAllTandaTerimaAsync(GetAllTandaTerimaRequest dto)
        {
            var list = new List<Wisuda>();
            int totalData = 0;

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getDataTandaTerima", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@TahunLulus", dto.TahunLulus);
            cmd.Parameters.AddWithValue("@Keyword", dto.SearchKeyword ?? string.Empty);
            cmd.Parameters.AddWithValue("@KonID", dto.KonsentrasiId ?? string.Empty);
            cmd.Parameters.AddWithValue("@StatusTerima", dto.StatusTerima ?? string.Empty);
            cmd.Parameters.AddWithValue("@Urut", dto.Urut ?? "mhs_id asc");
            cmd.Parameters.AddWithValue("@Halaman", dto.PageNumber);
            cmd.Parameters.AddWithValue("@Limit", dto.PageSize);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                totalData = reader.GetInt32(reader.GetOrdinal("Count"));

                do
                {
                    list.Add(new Wisuda
                    {
                        RowNumber = reader.GetInt64(reader.GetOrdinal("rownum")),
                        MahasiswaId = reader.GetString(reader.GetOrdinal("mhs_id")),
                        MahasiswaNama = reader.GetString(reader.GetOrdinal("mhs_nama")),
                        ProdiSingkatan = reader.GetString(reader.GetOrdinal("pro_singkatan")),
                        DetailStatus = reader.GetString(reader.GetOrdinal("tti_status")),
                        Status = reader.GetString(reader.GetOrdinal("tti_status2")),
                        KonsentrasiId = reader.GetInt16(reader.GetOrdinal("kon_id")),
                        Foto = reader.GetString(reader.GetOrdinal("tti_foto")),
                    });
                }
                while (await reader.ReadAsync());
            }

            return (list, totalData);
        }

        public async Task<IEnumerable<Wisuda>> GetListLulusanAsync(int tahunLulus)
        {
            var list = new List<Wisuda>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getListLulusan", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@TahunLulus", tahunLulus);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new Wisuda
                {
                    MahasiswaId = reader.GetString(reader.GetOrdinal("mhs_id")),
                    MahasiswaNama = reader.GetString(reader.GetOrdinal("mhs_nama")),
                });
            }

            return list;
        }

       
        public async Task<IEnumerable<Wisuda>> GetListKonsentrasiAsync(string username, string roleId)
        {
            var list = new List<Wisuda>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getListKonsentrasi", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@roleId", roleId);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new Wisuda
                {
                    KonsentrasiId = reader.GetInt16(reader.GetOrdinal("kon_id")),
                    KonsentrasiNama = reader.GetString(reader.GetOrdinal("kon_nama")),
                    KonsentrasiSingkatan = reader.GetString(reader.GetOrdinal("kon_singkatan")),
                    KonsentrasiStatus = reader.GetString(reader.GetOrdinal("kon_status")),
                });
            }

            return list;
        }

        public async Task<bool> CreateTandaTerimaIjazahAsync(string createdBy,CreateWisudaRequest dto)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_createTandaTerimaIjazah", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@nim_mahasiswa", dto.MahasiswaId);
            cmd.Parameters.AddWithValue("@foto", dto.Foto);
            cmd.Parameters.AddWithValue("@created_by", createdBy);

            var returnValue = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
            returnValue.Direction = ParameterDirection.ReturnValue;

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();

            return (int)returnValue.Value == 1;
        }
    }
}
