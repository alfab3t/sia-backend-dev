using astratech_apps_backend.DTOs.RiwayatPembukuan;
using astratech_apps_backend.Models;
using astratech_apps_backend.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace astratech_apps_backend.Repositories.Implementations
{
    public class RiwayatPembukuanRepository(IConfiguration config) : IRiwayatPembukuanRepository
    {
        private readonly string _conn = PolmanAstraLibrary.PolmanAstraLibrary.Decrypt(config.GetConnectionString("DefaultConnection")!, Environment.GetEnvironmentVariable("DECRYPT_KEY_CONNECTION_STRING"));

        public async Task<(IEnumerable<RiwayatPembukuan>, int TotalData)> GetAllAsync(GetAllRiwayatPembukuanRequest dto)
        {
            var list = new List<RiwayatPembukuan>();
            int totalData = 0;

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getDataRiwayatPembukuan", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add(new SqlParameter("@Keyword", SqlDbType.VarChar, -1) { Value = dto.Keyword ?? "" });
            cmd.Parameters.Add(new SqlParameter("@Prodi", SqlDbType.VarChar, 50) { Value = dto.ProgramStudi ?? "" });
            cmd.Parameters.Add(new SqlParameter("@TahunAjaran", SqlDbType.VarChar, 20) { Value = dto.TahunAjaran ?? "" });
            cmd.Parameters.Add(new SqlParameter("@TanggalDari", SqlDbType.Date) { Value = ParseTanggalMulai(dto.TanggalMulai) });
            cmd.Parameters.Add(new SqlParameter("@TanggalSampai", SqlDbType.Date) { Value = ParseTanggalSampai(dto.TanggalSampai) });
            cmd.Parameters.Add(new SqlParameter("@Urut", SqlDbType.VarChar, 100) { Value = dto.Urut ?? "[Waktu Penerimaan] desc" });
            cmd.Parameters.Add(new SqlParameter("@Halaman", SqlDbType.Int) { Value = dto.PageNumber });
            cmd.Parameters.Add(new SqlParameter("@Limit", SqlDbType.Int) { Value = dto.PageSize });

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                totalData = reader.GetInt32(reader.GetOrdinal("Count"));
                do
                {
                    list.Add(MapRiwayatPembukuan(reader));
                } while (await reader.ReadAsync());
            }

            return (list, totalData);
        }

        public async Task<IEnumerable<RiwayatPembukuanExportDto>> GetForExportAsync(GetAllRiwayatPembukuanRequest dto)
        {
            var list = new List<RiwayatPembukuanExportDto>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getDataRiwayatPembukuanExport", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add(new SqlParameter("@Keyword", SqlDbType.VarChar, -1) { Value = dto.Keyword ?? "" });
            cmd.Parameters.Add(new SqlParameter("@Urut", SqlDbType.VarChar, 100) { Value = dto.Urut ?? "[Waktu Penerimaan] desc" });
            cmd.Parameters.Add(new SqlParameter("@TanggalMulai", SqlDbType.VarChar, 20) { Value = ParseTanggalForExport(dto.TanggalMulai) });
            cmd.Parameters.Add(new SqlParameter("@TanggalSampai", SqlDbType.VarChar, 20) { Value = ParseTanggalForExport(dto.TanggalSampai) });
            cmd.Parameters.Add(new SqlParameter("@ProgramStudi", SqlDbType.VarChar, 50) { Value = dto.ProgramStudi ?? "" });
            cmd.Parameters.Add(new SqlParameter("@TahunAjaran", SqlDbType.VarChar, 20) { Value = dto.TahunAjaran ?? "" });

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(MapRiwayatPembukuanExportDto(reader));
            }

            return list;
        }

        public async Task<IEnumerable<(string Id, string Nama)>> GetListKonsentrasiAsync()
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
                var konsentrasiId = reader["kon_id"].ToString() ?? string.Empty;
                var konsentrasiNama = reader["kon_nama"].ToString() ?? string.Empty;
                list.Add((konsentrasiId, konsentrasiNama));
            }

            return list;
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
                var tahunAjaran = reader["kak_tahun_ajaran"].ToString() ?? string.Empty;
                if (!string.IsNullOrEmpty(tahunAjaran))
                {
                    list.Add(tahunAjaran);
                }
            }

            return list;
        }

        public async Task<string?> GetActiveTahunAjaranByTanggalAsync()
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getActiveTahunAkademikByTanggal", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();
            return result?.ToString();
        }

        private static DateTime ParseTanggalMulai(string? tanggal)
        {
            return string.IsNullOrWhiteSpace(tanggal) || 
                   !DateTime.TryParse(tanggal, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime result)
                ? new DateTime(1753, 1, 1, 0, 0, 0, DateTimeKind.Unspecified)
                : result.Date;
        }

        private static DateTime ParseTanggalSampai(string? tanggal)
        {
            return string.IsNullOrWhiteSpace(tanggal) || 
                   !DateTime.TryParse(tanggal, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime result)
                ? new DateTime(2079, 6, 6, 0, 0, 0, DateTimeKind.Unspecified)
                : result.Date;
        }

        private static string ParseTanggalForExport(string? tanggal)
        {
            if (string.IsNullOrWhiteSpace(tanggal))
                return string.Empty;

            return DateTime.TryParse(tanggal, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime result)
                ? result.ToString("yyyy-MM-dd")
                : string.Empty;
        }

        private static RiwayatPembukuan MapRiwayatPembukuan(SqlDataReader reader)
        {
            return new RiwayatPembukuan
            {
                Id = reader.GetInt32(reader.GetOrdinal("trk_id")),
                RowNumber = reader.IsDBNull(reader.GetOrdinal("rownum")) ? null : reader.GetInt64(reader.GetOrdinal("rownum")),
                TransactionDate = reader.IsDBNull(reader.GetOrdinal("trk_tanggal")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("trk_tanggal")),
                Description = reader.IsDBNull(reader.GetOrdinal("trk_keterangan")) ? string.Empty : reader.GetString(reader.GetOrdinal("trk_keterangan")).Replace("%", ""),
                Amount = reader.IsDBNull(reader.GetOrdinal("trk_jumlah")) ? 0 : Convert.ToDecimal(reader["trk_jumlah"]),
                Program = reader.IsDBNull(reader.GetOrdinal("kon_singkatan")) ? string.Empty : reader.GetString(reader.GetOrdinal("kon_singkatan")),
                AcademicYear = reader.IsDBNull(reader.GetOrdinal("trk_tahun_ajaran")) ? string.Empty : reader.GetString(reader.GetOrdinal("trk_tahun_ajaran")),
                StudentId = reader.IsDBNull(reader.GetOrdinal("mhs_id")) ? string.Empty : reader.GetString(reader.GetOrdinal("mhs_id")),
                StudentName = reader.IsDBNull(reader.GetOrdinal("mhs_nama")) ? string.Empty : reader.GetString(reader.GetOrdinal("mhs_nama")),
                VirtualAccount = reader.IsDBNull(reader.GetOrdinal("vac_number")) ? string.Empty : reader.GetString(reader.GetOrdinal("vac_number")),
                ModifiedBy = reader.IsDBNull(reader.GetOrdinal("trk_modif_by")) ? null : reader.GetString(reader.GetOrdinal("trk_modif_by"))
            };
        }

        private static RiwayatPembukuanExportDto MapRiwayatPembukuanExportDto(SqlDataReader reader)
        {
            return new RiwayatPembukuanExportDto
            {
                No = reader.IsDBNull(reader.GetOrdinal("No")) ? 0 : Convert.ToInt32(reader["No"]),
                VirtualAccount = reader.IsDBNull(reader.GetOrdinal("Virtual Account")) ? string.Empty : reader.GetString(reader.GetOrdinal("Virtual Account")),
                NIMOrNoDaftar = reader.IsDBNull(reader.GetOrdinal("NIM/No. Daftar")) ? string.Empty : reader.GetString(reader.GetOrdinal("NIM/No. Daftar")),
                Prodi = reader.IsDBNull(reader.GetOrdinal("Prodi")) ? string.Empty : reader.GetString(reader.GetOrdinal("Prodi")),
                Nama = reader.IsDBNull(reader.GetOrdinal("Nama")) ? string.Empty : reader.GetString(reader.GetOrdinal("Nama")),
                TahunAjaran = reader.IsDBNull(reader.GetOrdinal("Tahun Ajaran")) ? string.Empty : reader.GetString(reader.GetOrdinal("Tahun Ajaran")),
                WaktuPembukuan = reader.IsDBNull(reader.GetOrdinal("Waktu/Tanggal Pembukuan")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("Waktu/Tanggal Pembukuan")),
                Jumlah = reader.IsDBNull(reader.GetOrdinal("Jumlah")) ? 0 : Convert.ToDecimal(reader["Jumlah"]),
                Keterangan = reader.IsDBNull(reader.GetOrdinal("Keterangan")) ? string.Empty : reader.GetString(reader.GetOrdinal("Keterangan")).Replace("%", "")
            };
        }
    }
}