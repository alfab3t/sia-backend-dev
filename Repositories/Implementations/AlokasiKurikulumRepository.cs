using astratech_apps_backend.DTOs.AlokasiKurikulum;
using astratech_apps_backend.Models;
using astratech_apps_backend.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace astratech_apps_backend.Repositories.Implementations
{
    public class AlokasiKurikulumRepository(IConfiguration config) : IAlokasiKurikulumRepository
    {
        private readonly string _conn = PolmanAstraLibrary.PolmanAstraLibrary.Decrypt(config.GetConnectionString("DefaultConnection")!, Environment.GetEnvironmentVariable("DECRYPT_KEY_CONNECTION_STRING"));
        private const string KolomNamaKurikulum = "kur_nama";

        public async Task<(IEnumerable<AlokasiKurikulumMahasiswa>, int totalData)> GetMahasiswaKurikulumListAsync(GetAllKurikulumMahasiswaRequest request)
        {
            var list = new List<AlokasiKurikulumMahasiswa>();
            int totalData = 0;

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getDataKurikulumMahasiswa", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Keyword", request.SearchKeyword ?? "");
            cmd.Parameters.AddWithValue("@Urut", string.IsNullOrEmpty(request.Urut) ? "mhs_id asc" : request.Urut);
            cmd.Parameters.AddWithValue("@KonId", request.ProgramStudiId ?? "");
            cmd.Parameters.AddWithValue("@Angkatan", request.Angkatan ?? "");
            cmd.Parameters.AddWithValue("@Halaman", request.PageNumber);
            cmd.Parameters.AddWithValue("@Limit", request.PageSize);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                totalData = Convert.ToInt32(reader["Count"]);

                var mhs = new AlokasiKurikulumMahasiswa
                {
                    RowNum = Convert.ToInt64(reader["rownum"]),
                    Id = reader.GetString(reader.GetOrdinal("mhs_id")),
                    Nama = reader.GetString(reader.GetOrdinal("mhs_nama")),
                    ProgramStudi = reader.GetString(reader.GetOrdinal("kon_singkatan")),
                    Angkatan = reader.GetString(reader.GetOrdinal("mhs_angkatan")),
                    Status = reader.GetString(reader.GetOrdinal("mhs_status_kuliah")),
                    IdProgramStudi = reader.GetInt16(reader.GetOrdinal("kon_id")),
                    Kurikulum = string.IsNullOrEmpty(reader.GetString(reader.GetOrdinal(KolomNamaKurikulum)))  ? "-" : reader.GetString(reader.GetOrdinal(KolomNamaKurikulum))
                };

                list.Add(mhs);
            }

            return (list, totalData);
        }

        public async Task<bool> UpdateAlokasiKurikulumAsync(UpdateAlokasiKurikulumRequest dto)
        {
            await using var conn = new SqlConnection(_conn);
            await conn.OpenAsync();

            try
            {
                foreach(var Id in dto.ListMahasiswaId)
                {
                    await using var cmd = new SqlCommand("sia_setKurikulumMahasiswa", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@Nim", Id); 
                    cmd.Parameters.AddWithValue("@Kurikulum", dto.KurikulumId); 

                    await cmd.ExecuteNonQueryAsync();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<IEnumerable<AlokasiKurikulumMahasiswa>> GetProdiListAsync()
        {
            var list = new List<AlokasiKurikulumMahasiswa>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getListKonsentrasi", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new AlokasiKurikulumMahasiswa
                {
                    IdProgramStudi = Convert.ToInt16(reader["kon_id"]),
                    ProgramStudi = reader["kon_nama"] != DBNull.Value && reader["kon_nama"] is not null ? reader["kon_nama"]!.ToString()! : string.Empty
                });
            }

            return list;
        }

        public async Task<IEnumerable<AlokasiKurikulumMahasiswa>> GetAngkatanListAsync()
        {
            var list = new List<AlokasiKurikulumMahasiswa>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getListAngkatan", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            { 
                list.Add(new AlokasiKurikulumMahasiswa
                {
                    Angkatan = reader.GetString(reader.GetOrdinal("dul_angkatan"))
                });
            }
            return list;
        }
    
        public async Task<IEnumerable<AlokasiKurikulumMahasiswa>> GetKurikulumListAsync(string konid)
        {
            var list = new List<AlokasiKurikulumMahasiswa>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getListKurikulum", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@IdProdi", konid ?? "");
            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();    

            while(await reader.ReadAsync())
            {
                list.Add(new AlokasiKurikulumMahasiswa
                {
                    IdKurikulum = reader["kur_id"] != DBNull.Value && reader["kur_id"] is not null ? reader["kur_id"]!.ToString()! : string.Empty,
                    Kurikulum = reader[KolomNamaKurikulum] != DBNull.Value && reader[KolomNamaKurikulum] is not null ? reader[KolomNamaKurikulum]!.ToString()! : string.Empty
                });
            }
            return list;
        }
    }
}
