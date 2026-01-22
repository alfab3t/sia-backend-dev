using astratech_apps_backend.DTOs.TagihanCalonMahasiswa;
using astratech_apps_backend.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace astratech_apps_backend.Repositories.Implementations
{
    public class TagihanCalonMahasiswaRepository(IConfiguration config) : ITagihanCalonMahasiswaRepository
    {
        private readonly string _conn = PolmanAstraLibrary.PolmanAstraLibrary.Decrypt(
            config.GetConnectionString("DefaultConnection")!,
            Environment.GetEnvironmentVariable("DECRYPT_KEY_CONNECTION_STRING")
        );

        public async Task<GetAllTagihanCalonMahasiswaResponse> GetAllAsync(GetAllTagihanCalonMahasiswaRequest dto)
        {
            var response = new GetAllTagihanCalonMahasiswaResponse();
            var dataList = new List<TagihanCalonMahasiswaDto>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getDataMahasiswa4", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add(new SqlParameter("@SearchKeyword", SqlDbType.VarChar, -1) { Value = dto.Keyword ?? "" });
            cmd.Parameters.Add(new SqlParameter("@Nim", SqlDbType.VarChar, 50) { Value = dto.NIM ?? "" });
            cmd.Parameters.Add(new SqlParameter("@Prodi", SqlDbType.VarChar, 50) { Value = dto.Prodi ?? "" });
            cmd.Parameters.Add(new SqlParameter("@Angkatan", SqlDbType.VarChar, 10) { Value = dto.Angkatan ?? "" });
            cmd.Parameters.Add(new SqlParameter("@Status", SqlDbType.VarChar, 50) { Value = dto.Status ?? "" });
            cmd.Parameters.Add(new SqlParameter("@Urut", SqlDbType.VarChar, 100) { Value = dto.Urut ?? "[No. Daftar] DESC" });
            cmd.Parameters.Add(new SqlParameter("@PageNumber", SqlDbType.Int) { Value = dto.PageNumber });
            cmd.Parameters.Add(new SqlParameter("@PageSize", SqlDbType.Int) { Value = dto.PageSize });

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            int totalData = 0;

            while (await reader.ReadAsync())
            {
                if (totalData == 0)
                    totalData = SafeGetInt32(reader, "TotalCount");

                dataList.Add(MapTagihanCalonMahasiswa(reader));
            }

            response.Data = dataList;
            response.TotalData = totalData;
            response.TotalHalaman = dto.PageSize > 0
                ? (int)Math.Ceiling(totalData / (double)dto.PageSize)
                : 1;

            return response;
        }

        public async Task<IEnumerable<ExportTagihanCalonMahasiswaResponse>> GetForExportAsync(GetAllTagihanCalonMahasiswaRequest dto)
        {
            var exportRequest = new GetAllTagihanCalonMahasiswaRequest
            {
                Keyword = dto.Keyword,
                NIM = dto.NIM,
                Prodi = dto.Prodi,
                Angkatan = dto.Angkatan,
                Status = dto.Status,
                Urut = dto.Urut ?? "[Angkatan] desc",
                PageNumber = 1,
                PageSize = int.MaxValue
            };

            var result = await GetAllAsync(exportRequest);

            var exportList = new List<ExportTagihanCalonMahasiswaResponse>();
            int index = 1;

            foreach (var item in result.Data)
            {
                exportList.Add(new ExportTagihanCalonMahasiswaResponse
                {
                    RowNumber = index++,
                    NIM = item.NIM,
                    Nama = item.Nama,
                    JalurMasuk = item.JalurMasuk,
                    Prodi = item.Prodi,
                    Angkatan = item.Angkatan,
                    TotalTagihan = item.TotalTagihan,
                    TotalPembayaran = item.TotalPembayaran,
                    SisaTagihan = item.SisaTagihan
                });
            }

            return exportList;
        }

        public async Task<IEnumerable<(string Id, string Nama)>> GetListProdiAsync()
        {
            var list = new List<(string, string)>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getListKonsentrasi", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var konsentrasiId = reader["kon_id"]?.ToString() ?? string.Empty;
                var konsentrasiNama = reader["kon_nama"]?.ToString() ?? string.Empty;
                list.Add((konsentrasiId, konsentrasiNama));
            }

            return list;
        }

        public async Task<IEnumerable<string>> GetListAngkatanAsync()
        {
            var list = new List<string>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getListAngkatanAktif", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var angkatan = reader["mhs_angkatan"]?.ToString() ?? string.Empty;
                if (!string.IsNullOrEmpty(angkatan))
                {
                    list.Add(angkatan);
                }
            }

            return list;
        }

        private static TagihanCalonMahasiswaDto MapTagihanCalonMahasiswa(SqlDataReader reader)
        {
            var totalTagihan = SafeGetDecimal(reader, "TotalTagihan");
            var totalPembayaran = SafeGetDecimal(reader, "TotalPembayaran");

            return new TagihanCalonMahasiswaDto
            {
                RowNumber = SafeGetInt64(reader, "RowNumber"),
                NIM = SafeGetString(reader, "NIM"),
                Nama = SafeGetString(reader, "Nama"),
                JalurMasuk = SafeGetString(reader, "Jalur"),
                Prodi = SafeGetString(reader, "Prodi"),
                Angkatan = SafeGetString(reader, "Angkatan"),
                TotalTagihan = totalTagihan,
                TotalPembayaran = totalPembayaran,
                SisaTagihan = totalTagihan - totalPembayaran
            };
        }

        private static string SafeGetString(SqlDataReader reader, string columnName)
        {
            try
            {
                var ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? string.Empty : reader.GetString(ordinal);
            }
            catch
            {
                return string.Empty;
            }
        }

        private static long SafeGetInt64(SqlDataReader reader, string columnName)
        {
            try
            {
                var ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? 0 : reader.GetInt64(ordinal);
            }
            catch
            {
                return 0;
            }
        }

        private static int SafeGetInt32(SqlDataReader reader, string columnName)
        {
            try
            {
                var ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? 0 : reader.GetInt32(ordinal);
            }
            catch
            {
                return 0;
            }
        }

        private static decimal SafeGetDecimal(SqlDataReader reader, string columnName)
        {
            try
            {
                var ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? 0 : Convert.ToDecimal(reader[ordinal]);
            }
            catch
            {
                return 0;
            }
        }
    }
}
