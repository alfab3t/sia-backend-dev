using astratech_apps_backend.DTOs.Program_Studi;
using astratech_apps_backend.Models;
using astratech_apps_backend.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace astratech_apps_backend.Repositories.Implementations
{
    public class ProdiRepository(IConfiguration config) : IProdiRepository
    {
        private readonly string _conn = PolmanAstraLibrary.PolmanAstraLibrary.Decrypt(config.GetConnectionString("DefaultConnection")!, Environment.GetEnvironmentVariable("DECRYPT_KEY_CONNECTION_STRING"));

        private static object ToSqlDate(DateTime? value)
        {
            return value.HasValue ? value.Value : DBNull.Value;
        }

        static string GetStringSafe(SqlDataReader r, string col) =>
                r[col] as string ?? string.Empty;

        static DateTime? GetDateTimeSafe(SqlDataReader r, string col)
        {
            var value = r[col] as string;
            if (string.IsNullOrWhiteSpace(value))
                return default;

            return DateTime.TryParse(value, out var date) ? date : default;
        }

        static short GetShortSafe(SqlDataReader r, string col) =>
            r[col] is short s ? s : Convert.ToInt16(r[col]);

        static short GetInt32ToShortSafe(SqlDataReader r, string col) =>
            r[col] is int i ? (short)i : Convert.ToInt16(r[col]);

        public async Task<int> CreateAsync(CreateProdiRequest dto, string createdBy)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_createProdi", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Jurusan", dto.JurusanId);
            cmd.Parameters.AddWithValue("@NamaProdi", dto.NamaProdi);
            cmd.Parameters.AddWithValue("@Singkatan", dto.Singkatan);
            cmd.Parameters.AddWithValue("@TanggalBerdiri", dto.TanggalBerdiri);
            cmd.Parameters.AddWithValue("@NamaKaprodi", dto.NamaKaprodi);
            cmd.Parameters.AddWithValue("@Npkkaprodi", dto.NpkKaprodi);
            cmd.Parameters.AddWithValue("@NamaSekprodi", dto.NamaSekprodi);
            cmd.Parameters.AddWithValue("@NpkSekprodi", dto.NpkSekprodi);
            cmd.Parameters.AddWithValue("@NomorProdi", dto.NomorProdi);
            cmd.Parameters.AddWithValue("@SingkatanKodeMk", dto.SingkatanKodeMatkul);
            cmd.Parameters.AddWithValue("@JenjangPendidikan", dto.Jenjang);
            cmd.Parameters.AddWithValue("@MasaStudi", dto.MasaStudi);
            cmd.Parameters.AddWithValue("@NomorSkDikti", dto.NomorSkDikti);
            cmd.Parameters.Add("@TanggalMulaiSkDikti", SqlDbType.Date).Value = ToSqlDate(dto.TglSkDiktiMulai);
            cmd.Parameters.Add("@TanggalSelesaiSkDikti", SqlDbType.Date).Value = ToSqlDate(dto.TglSkDiktiSelesai);
            cmd.Parameters.AddWithValue("@NomorAkreditasi", dto.NomorAkreditasi);
            cmd.Parameters.Add("@TanggalMulaiAkreditasi", SqlDbType.DateTime).Value = ToSqlDate(dto.TglAkreditasiMulai);
            cmd.Parameters.Add("@TanggalSelesaiAkreditasi", SqlDbType.DateTime).Value = ToSqlDate(dto.TglAkreditasiSelesai);
            cmd.Parameters.AddWithValue("@Akreditasi", dto.Akreditasi);

            cmd.Parameters.AddWithValue("@ProdiVisi", dto.Visi);
            cmd.Parameters.AddWithValue("@ProdiMisi", dto.Misi);
            cmd.Parameters.AddWithValue("@ProdiDeskripsi", dto.Deskripsi);
            cmd.Parameters.AddWithValue("@ProdiProfilLulusan", dto.ProfilLulusan);

            cmd.Parameters.AddWithValue("@CreatedBy", createdBy);

            await conn.OpenAsync();
            var result = await cmd.ExecuteNonQueryAsync();
            return result;

        }

        public async Task<(IEnumerable<Prodi>, int totalData)> GetAllAsync(GetAllProdiRequest dto)
        {
            var list = new List<Prodi>();
            int totalData = 0;

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getDataProdi", conn)
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
                    list.Add(new Prodi
                    {
                        Id = reader.GetInt16(reader.GetOrdinal("pro_id")),
                        RowNumber = reader.GetInt64(reader.GetOrdinal("rownum")),
                        NamaProdi = reader.GetString(reader.GetOrdinal("pro_nama")),
                        NamaKaprodi = reader.GetString(reader.GetOrdinal("pro_kaprodi")),
                        Status = reader.GetString(reader.GetOrdinal("pro_status")),
                        Singkatan = reader.GetString(reader.GetOrdinal("pro_singkatan")),
                        Jenjang = reader.GetString(reader.GetOrdinal("pro_jenjang")),
                        MasaStudi = reader.GetInt16(reader.GetOrdinal("pro_masa_studi")),
                    });
                } while (await reader.ReadAsync());
            }
            return (list, totalData);
        }

        public async Task<Prodi?> GetByIdAsync(short id)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_detailProdi", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@ProdiId", id.ToString());
            await conn.OpenAsync();

            await using var reader = await cmd.ExecuteReaderAsync();
            Prodi? prodi = null;

            while (await reader.ReadAsync())
            {
                if (prodi == null)
                {
                    prodi = new Prodi
                    {
                        Id = GetShortSafe(reader, "pro_id"),
                        Jurusan = GetStringSafe(reader, "jur_nama"),
                        JurusanId = GetInt32ToShortSafe(reader, "jur_id"),
                        NamaProdi = GetStringSafe(reader, "pro_nama"),
                        Singkatan = GetStringSafe(reader, "pro_singkatan"),
                        TanggalBerdiri = GetDateTimeSafe(reader, "pro_tgl_berdiri"),
                        NamaKaprodi = GetStringSafe(reader, "pro_kaprodi"),
                        NpkKaprodi = GetStringSafe(reader, "pro_npk"),
                        NomorProdi = GetStringSafe(reader, "pro_nomor"),
                        Jenjang = GetStringSafe(reader, "pro_jenjang"),
                        MasaStudi = GetShortSafe(reader, "pro_masa_studi"),
                        Status = GetStringSafe(reader, "pro_status"),
                        NoSkDikti = GetStringSafe(reader, "pro_no_skdikti"),
                        SkDiktiFrom = GetDateTimeSafe(reader, "pro_tgl_skdikti_from"),
                        SkDiktiUntil = GetDateTimeSafe(reader, "pro_tgl_skdikti_until"),
                        NoBanpt = GetStringSafe(reader, "pro_no_banpt"),
                        BanptFrom = GetDateTimeSafe(reader, "pro_tgl_banpt_from"),
                        BanptUntil = GetDateTimeSafe(reader, "pro_tgl_banpt_until"),
                        Akreditasi = GetStringSafe(reader, "pro_akreditasi"),
                        Deskripsi = GetStringSafe(reader, "pro_deskripsi"),
                        Visi = GetStringSafe(reader, "pro_visi"),
                        Misi = GetStringSafe(reader, "pro_misi"),
                        ProfilLulusan = GetStringSafe(reader, "pro_profil_lulusan"),
                    };
                }

                prodi.SekprodiList.Add(GetStringSafe(reader, "kon_sekprodi"));
                prodi.NpkSekprodiList.Add(GetStringSafe(reader, "kon_npk"));
                prodi.SingkatanMkList.Add(GetStringSafe(reader, "kon_singkatan_mk"));
            }

            return prodi;
        }


        public async Task<bool> SetStatusAsync(short id, string updatedBy)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_setStatusProdi", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@ProdiId", id);
            cmd.Parameters.AddWithValue("@UpdatedBy", updatedBy);

            var returnParameter = cmd.Parameters.Add("@returnVal", SqlDbType.Int);
            returnParameter.Direction = ParameterDirection.ReturnValue;

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();

            int result = (int)returnParameter.Value;
            return result > 0;
        }

        public async Task<bool> UpdateAsync(UpdateProdiRequest dto, string updatedBy)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_editProdi", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@ProdiId", dto.Id);
            cmd.Parameters.AddWithValue("@JurusanId", dto.JurusanId);
            cmd.Parameters.AddWithValue("@NamaProdi", dto.NamaProdi);
            cmd.Parameters.AddWithValue("@SingkatanProdi", dto.Singkatan);
            cmd.Parameters.Add("@TanggalBerdiri", SqlDbType.Date).Value = ToSqlDate(dto.TanggalBerdiri);

            cmd.Parameters.AddWithValue("@NamaKaprodi", dto.NamaKaprodi);
            cmd.Parameters.AddWithValue("@NpkKaprodi", dto.NpkKaprodi);
            cmd.Parameters.AddWithValue("@NamaSekprodi", dto.NamaSekprodi);
            cmd.Parameters.AddWithValue("@NpkSekprodi", dto.NpkSekprodi);

            cmd.Parameters.AddWithValue("@NomorProdi", dto.NomorProdi);
            cmd.Parameters.AddWithValue("@SingkatanMk",
                !string.IsNullOrEmpty(dto.SingkatanKodeMatkul)
                    ? dto.SingkatanKodeMatkul
                    : dto.SingkatanMk);

            cmd.Parameters.AddWithValue("@Jenjang", dto.Jenjang);
            cmd.Parameters.AddWithValue("@MasaStudi", dto.MasaStudi);

            cmd.Parameters.AddWithValue("@NoSkDikti", dto.NomorSkDikti);
            cmd.Parameters.Add("@TglSkDiktiMulai", SqlDbType.Date)
                .Value = ToSqlDate(dto.TglSkDiktiMulai);
            cmd.Parameters.Add("@TglSkDiktiSelesai", SqlDbType.Date)
                .Value = ToSqlDate(dto.TglSkDiktiSelesai);

            cmd.Parameters.AddWithValue("@NomorAkreditasi", dto.NomorAkreditasi);
            cmd.Parameters.Add("@TglAkreditasiMulai", SqlDbType.Date)
                .Value = ToSqlDate(dto.TglAkreditasiMulai);
            cmd.Parameters.Add("@TglAkreditasiSelesai", SqlDbType.Date)
                .Value = ToSqlDate(dto.TglAkreditasiSelesai);
            cmd.Parameters.AddWithValue("@Akreditasi", dto.Akreditasi);

            cmd.Parameters.AddWithValue("@Visi", dto.Visi);
            cmd.Parameters.AddWithValue("@Misi", dto.Misi);
            cmd.Parameters.AddWithValue("@Deskripsi", dto.Deskripsi);
            cmd.Parameters.AddWithValue("@ProfilLulusan", dto.ProfilLulusan);

            cmd.Parameters.AddWithValue("@UpdatedBy", updatedBy);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();

            return true;
        }

        public async Task<bool> CheckNPKAsync(string npk)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("ess_checkNPK", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@npk", npk);

            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();

            return result != null && !string.IsNullOrEmpty(result.ToString());
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

        public async Task<ProdiDto> CheckNomerProdiAsync(string nomerProdi)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_checkNomorProdi", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@NomorProdi", nomerProdi);

            await conn.OpenAsync();

            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new ProdiDto
                {
                    ProNomor = reader["pro_nomor"]?.ToString() ?? string.Empty,
                    ProNama = reader["pro_nama"]?.ToString() ?? string.Empty
                };
            }

            return null;
        }
    }
}
