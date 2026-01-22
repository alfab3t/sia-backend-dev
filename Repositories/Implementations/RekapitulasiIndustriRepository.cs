using astratech_apps_backend.DTOs.RekapitulsiIndustri;
using astratech_apps_backend.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace astratech_apps_backend.Repositories.Implementations
{
    public class RekapitulasiIndustriRepository : IRekapitulasiIndustriRepository
    {
        private readonly string _conn;

        public RekapitulasiIndustriRepository(IConfiguration config)
        {
            _conn = PolmanAstraLibrary.PolmanAstraLibrary.Decrypt(config.GetConnectionString("DefaultConnection")!, Environment.GetEnvironmentVariable("DECRYPT_KEY_CONNECTION_STRING"));
        }
        public async Task<GetAllRekapitulasiIndustriResponse> GetAllAsync(GetAllRekapitulasiIndustriRequest request)
        {
            var list = new List<RekapitulasiIndustriDto>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getRekapitulasiIndustri", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            string orderBy = string.IsNullOrEmpty(request.OrderBy)
                ? "Nama Industri ASC"
                : request.OrderBy;

            cmd.Parameters.AddWithValue("@Keyword", request.Keyword ?? "");
            cmd.Parameters.AddWithValue("@OrderBy", orderBy);
            cmd.Parameters.AddWithValue("@TahunAjaran", request.TahunAjaran);
            cmd.Parameters.AddWithValue("@PageNumber", request.PageNumber);
            cmd.Parameters.AddWithValue("@PageSize", request.PageSize);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new RekapitulasiIndustriDto
                {
                    TahunAjaran = reader["kin_tahun_ajaran"]?.ToString() ?? "",
                    NamaIndustri = reader["ipr_nama"]?.ToString() ?? "",
                    GrupIndustri = reader["ipr_grup"]?.ToString() ?? "",

                    p4 = Convert.ToInt32(reader["p4"]),
                    tpm = Convert.ToInt32(reader["tpm"]),
                    mi = Convert.ToInt32(reader["mi"]),
                    to = Convert.ToInt32(reader["to"]),
                    mk = Convert.ToInt32(reader["mk"]),
                    tab = Convert.ToInt32(reader["tab"]),
                    tphp = Convert.ToInt32(reader["tphp"]),
                    jumlah = Convert.ToInt32(reader["jumlah"])
                });
            }

            return new GetAllRekapitulasiIndustriResponse
            {
                Data = list,
                TotalData = list.Count,
                TotalHalaman = (int)Math.Ceiling(
                    (double)list.Count / request.PageSize
                )
            };
        }


        public async Task<IEnumerable<RekapitulasiIndustriPivotDto>> GetPivotAsync(GetAllRekapitulasiIndustriPivotRequest request)
        {
            var result = new List<RekapitulasiIndustriPivotDto>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getRekapitulasiIndustri2", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            string orderBy = string.IsNullOrEmpty(request.OrderBy)
                ? "NamaGrup ASC"
                : request.OrderBy;

            cmd.Parameters.AddWithValue("@Keyword", request.Keyword ?? "");
            cmd.Parameters.AddWithValue("@OrderBy", orderBy);
            cmd.Parameters.AddWithValue("@TahunAjaran", request.TahunAjaran);
            cmd.Parameters.AddWithValue("@PageNumber", request.PageNumber);
            cmd.Parameters.AddWithValue("@PageSize", request.PageSize);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            var columnNames = (await reader.GetColumnSchemaAsync())
                .Select(c => c.ColumnName)
                .Where(name =>
                    name is not (
                        "Program Studi" or
                        "Jumlah Mahasiswa" or
                        "TotalIndustri" or
                        "Sisa" or
                        "RowNum"))
                .ToList();

            while (await reader.ReadAsync())
            {
                var dto = new RekapitulasiIndustriPivotDto
                {
                    ProgramStudi = reader["Program Studi"]?.ToString() ?? "",
                    JumlahMahasiswa = Convert.ToInt32(reader["Jumlah Mahasiswa"]),
                    Total = Convert.ToInt32(reader["TotalIndustri"]),
                    Sisa = Convert.ToInt32(reader["Sisa"]),
                    JumlahPerIndustri = new Dictionary<string, int>()
                };

                foreach (var name in columnNames)
                {
                    dto.JumlahPerIndustri[name] =
                        reader[name] == DBNull.Value
                            ? 0
                            : Convert.ToInt32(reader[name]);
                }

                result.Add(dto);
            }

            return result;
        }
        public async Task<IEnumerable<RekapitulasiIndustriMahasiswaDto>>GetDetailMahasiswaAsync(string namaIndustri, string tahunAjaran)
        {
            var result = new List<RekapitulasiIndustriMahasiswaDto>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getDetailRekapitulasiIndustri", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@NamaIndustri", namaIndustri);
            cmd.Parameters.AddWithValue("@TahunAjaran", tahunAjaran);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                result.Add(new RekapitulasiIndustriMahasiswaDto
                {
                    TahunAjaran = reader["kin_tahun_ajaran"]?.ToString() ?? "",
                    CabangIndustri = reader["ipr_cabang"]?.ToString() ?? "",
                    NIM = reader["mhs_id"]?.ToString() ?? "",
                    NamaMahasiswa = reader["mhs_nama"]?.ToString() ?? "",
                    Konsentrasi = reader["kon_singkatan"]?.ToString() ?? ""
                });
            }

            return result;
        }

        public async Task<IEnumerable<string>> GetListTahunAjaranAsync()
        {
            var list = new List<string>();
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getListTahunAjaran", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(reader["kak_tahun_ajaran"].ToString() ?? "");
            }
            return list;
        }

        public async Task<string> GetActiveTahunAjaranAsync()
        {
            string activeYear = "";
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getActiveTahunAkademikByTanggal", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();
            activeYear = result?.ToString() ?? "";

            return activeYear;
        }
    }
}
