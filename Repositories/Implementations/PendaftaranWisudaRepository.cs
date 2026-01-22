using astratech_apps_backend.DTOs.PendaftaranWisuda;
using astratech_apps_backend.Models;
using astratech_apps_backend.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace astratech_apps_backend.Repositories.Implementations
{
    public class PendaftaranWisudaRepository : IPendaftaranWisudaRepository
    {
        private readonly string _conn;

        private const string ParamKeyword = "@Keyword";
        private const string ParamRole = "@Role";
        private const string ParamMahasiswaId = "@MahasiswaId";
        private const string ColWisTanggalDaftar = "wis_tanggal_daftar";
        private const string RoleDefault = "ROL21";

        public PendaftaranWisudaRepository(IConfiguration config)
        {
            _conn = PolmanAstraLibrary.PolmanAstraLibrary.Decrypt(
                config.GetConnectionString("DefaultConnection")!,
                Environment.GetEnvironmentVariable("DECRYPT_KEY_CONNECTION_STRING")
            );
        }

        public async Task<string> CreatePendaftaranWisudaAsync(CreatePendaftaranWisudaRequest dto, string createdBy)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_createWisudaHead", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@TahunAkademik", dto.TahunAkademik);
            cmd.Parameters.AddWithValue("@TanggalMulaiPendaftaran", dto.TanggalMulaiPendaftaran.Date);
            cmd.Parameters.AddWithValue("@TanggalAkhirPendaftaran", dto.TanggalAkhirPendaftaran.Date);
            cmd.Parameters.AddWithValue("@KuotaUndanganTambahan", dto.KuotaUndanganTambahan);
            cmd.Parameters.AddWithValue("@CreatedBy", createdBy);

            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();

            return result?.ToString() ?? "FAILED";
        }

        public async Task<List<string>> GetListTahunAkademikAsync()
        {
            var result = new List<string>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getListTahunAjaran", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                result.Add(reader["kak_tahun_ajaran"]?.ToString() ?? "");
            }

            return result;
        }

        public async Task<(IEnumerable<PendaftaranWisuda>, int totalData)> GetAllAsync(GetAllPendaftaranWisudaRequest dto)
        {
            var list = new List<PendaftaranWisuda>();
            int totalData = 0;

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getDataWisudaHead", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue(ParamKeyword, dto.SearchKeyword ?? "");
            cmd.Parameters.AddWithValue("@Urut", dto.Urut);
            cmd.Parameters.AddWithValue("@Halaman", dto.PageNumber);
            cmd.Parameters.AddWithValue("@Limit", dto.PageSize);

            await conn.OpenAsync();
    
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                if (totalData == 0)
                {
                    totalData = Convert.ToInt32(reader["Count"]);
                }

                list.Add(new PendaftaranWisuda
                {
                    Id = GetInt(reader, "id"),
                    TahunAkademik = GetString(reader, "tahunAkademik"),
                    TanggalMulaiPendaftaran = GetString(reader, "tanggalMulaiPendaftaran"),
                    TanggalAkhirPendaftaran = GetString(reader, "tanggalAkhirPendaftaran"),
                    KuotaUndanganTambahan = GetInt(reader, "kuotaUndanganTambahan")
                });
            }

            return (list, totalData);
        }

        public async Task<(IEnumerable<PendaftaranWisuda?>, int totalData)> GetByIdAsync(int id)
        {
            var list = new List<PendaftaranWisuda?>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getDataWisudaHead", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue(ParamKeyword, "");
            cmd.Parameters.AddWithValue("@Urut", "whe_id");
            cmd.Parameters.AddWithValue("@Halaman", 1);
            cmd.Parameters.AddWithValue("@Limit", 1);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            int totalData = 0;

            while (await reader.ReadAsync())
            {
                list.Add(new PendaftaranWisuda
                {
                    Id = GetInt(reader, "id"),
                    TahunAkademik = GetString(reader, "tahunAkademik"),
                    TanggalMulaiPendaftaran = GetString(reader, "tanggalMulaiPendaftaran"),
                    TanggalAkhirPendaftaran = GetString(reader, "tanggalAkhirPendaftaran"),
                    KuotaUndanganTambahan = GetInt(reader, "kuotaUndanganTambahan")
                });

                totalData++;
            }

            return (list, totalData);
        }

        public async Task<bool> UpdateAsync(UpdatePendaftaranWisudaRequest dto, string updatedBy)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_editWisudaHead", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Id", dto.Id);
            cmd.Parameters.AddWithValue("@TanggalMulai", dto.TanggalMulai);
            cmd.Parameters.AddWithValue("@TanggalSelesai", dto.TanggalAkhir);
            cmd.Parameters.AddWithValue("@KuotaTambahan", dto.KuotaTambahan);
            cmd.Parameters.AddWithValue("@ModifiedBy", updatedBy);

            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();

            return result?.ToString() == "SUCCESS";
        }

        public async Task<(List<DetailDataWisuda>, int)> GetDetailPesertaAsync(
            int wisudaHeadId,
            GetDetailPesertaWisudaRequest req)
        {
            var list = new List<DetailDataWisuda>();
            int totalData = 0;

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getDataWisuda", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue(ParamKeyword, req.SearchKeyword ?? "");
            cmd.Parameters.AddWithValue("@WisudaStatus", "");
            cmd.Parameters.AddWithValue("@OrderBy", req.Urut ?? ColWisTanggalDaftar);
            cmd.Parameters.AddWithValue("@KonsentrasiId", "");
            cmd.Parameters.AddWithValue("@TahunAjaran", "");
            cmd.Parameters.AddWithValue(ParamRole, RoleDefault);
            cmd.Parameters.AddWithValue(ParamMahasiswaId, "");
            cmd.Parameters.AddWithValue("@Sekretarisprodi", "");
            cmd.Parameters.AddWithValue("@StatusBerkas", "");
            cmd.Parameters.AddWithValue("@WisudaHeadId", wisudaHeadId);
            cmd.Parameters.AddWithValue("@PageNumber", req.PageNumber);
            cmd.Parameters.AddWithValue("@PageSize", req.PageSize);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                totalData = reader.IsDBNull(0)
                    ? 0
                    : reader.GetInt32(0);
            }

            if (await reader.NextResultAsync())
            {
                while (await reader.ReadAsync())
                {
                    list.Add(new DetailDataWisuda
                    {
                        NomorPendaftaran = GetString(reader, "wis_id"),
                        TanggalDaftar = GetDateTime(reader, ColWisTanggalDaftar),
                        Prodi = GetString(reader, "prodi"),
                        NIM = GetString(reader, "mhs_id"),
                        Nama = GetString(reader, "mhs_nama"),
                        KelengkapanBerkas = GetString(reader, "wis_status_berkas"),
                        StatusPembayaran = GetString(reader, "wis_status"),
                        TanggalBayar = GetDateTime(reader, "wis_tanggal_bayar"),
                        UndanganTambahan = GetInt(reader, "wis_undangan")
                    });
                }
            }

            return (list, totalData);
        }

        public async Task<WisudaDetailDto?> GetWisudaDetailAsync(string wisId)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_detailWisuda", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("@WisudaId", SqlDbType.VarChar, 200).Value = wisId;

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            return new WisudaDetailDto
            {
                NIM = GetString(reader, "nim"),
                Nama = GetString(reader, "nama"),
                TempatLahir = GetString(reader, "tempatLahir"),
                TanggalLahir = GetDateTime(reader, "tanggalLahir"),
                HP = GetString(reader, "hp"),
                Email = GetString(reader, "email"),
                Foto = GetString(reader, "foto"),
                Nominal = (long)GetDecimal(reader, "nominal"),
                Undangan = GetNullableInt(reader, "undangan"),
                StatusPembayaran = GetString(reader, "statusPembayaran"),
                VirtualAccount = GetString(reader, "virtualAccount"),
                TanggalBayar = GetDateTime(reader, "tanggalBayar"),
                TanggalDaftar = GetDateTime(reader, "tanggalDaftar"),
                StatusBerkas = GetString(reader, "statusBerkas"),
                NIK = GetString(reader, "nik"),
                Alamat = GetString(reader, "alamat")
            };
        }

        public async Task<List<ExportPesertaWisudaDto>> ExportPesertaWisudaAsync(int wisudaHeadId)
        {
            var list = new List<ExportPesertaWisudaDto>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getDataWisuda", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue(ParamKeyword, "");
            cmd.Parameters.AddWithValue("@WisudaStatus", "");
            cmd.Parameters.AddWithValue("@OrderBy", ColWisTanggalDaftar);
            cmd.Parameters.AddWithValue("@KonsentrasiId", "");
            cmd.Parameters.AddWithValue("@TahunAjaran", "");
            cmd.Parameters.AddWithValue(ParamRole, RoleDefault);
            cmd.Parameters.AddWithValue(ParamMahasiswaId, "");
            cmd.Parameters.AddWithValue("@SekretarisProdi", "");
            cmd.Parameters.AddWithValue("@StatusBerkas", "");
            cmd.Parameters.AddWithValue("@WisudaHeadId", wisudaHeadId);
            cmd.Parameters.AddWithValue("@PageNumber", 1);
            cmd.Parameters.AddWithValue("@PageSize", int.MaxValue);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();


            if (await reader.NextResultAsync())
            {
                while (await reader.ReadAsync())
                {
                    list.Add(new ExportPesertaWisudaDto
                    {
                        NomorPendaftaran = GetString(reader, "wis_id"),
                        TanggalDaftar = GetDateTime(reader, ColWisTanggalDaftar),
                        Prodi = GetString(reader, "prodi"),
                        NIM = GetString(reader, "mhs_id"),
                        NamaMahasiswa = GetString(reader, "mhs_nama"),
                        KelengkapanBerkas = GetString(reader, "wis_status_berkas"),
                        StatusPembayaran = GetString(reader, "wis_status"),
                        TanggalBayar = GetDateTime(reader, "wis_tanggal_bayar"),
                        UndanganTambahan = GetInt(reader, "wis_undangan")
                    });
                }
            }

            return list;
        }

        public async Task<int> CreateWisudaMahasiswaAsync(CreateWisudaRequest dto, string createdBy)
        {
            using var conn = new SqlConnection(_conn);
            using var cmd = new SqlCommand("sia_createWisuda", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@MahasiswasId", dto.NIM);
            cmd.Parameters.AddWithValue("@Nama", dto.NamaMahasiswa);
            cmd.Parameters.AddWithValue("@NomorHandphone", dto.NomorHandphone);
            cmd.Parameters.AddWithValue("@Email", dto.Email);
            cmd.Parameters.AddWithValue("@Undangan", dto.JumlahUndangan);
            cmd.Parameters.AddWithValue("@UserInput", createdBy);

            await conn.OpenAsync();
            return Convert.ToInt32(await cmd.ExecuteScalarAsync());
        }

        public async Task<bool> CheckKuotaTambahanAsync(
            string tahunAjaran,
            short jumlahUndangan,
            string mhsId)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_checkKuotaTambahan", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Tahun_ajaran", tahunAjaran);
            cmd.Parameters.AddWithValue("@Jumlah_undangan", jumlahUndangan.ToString());
            cmd.Parameters.AddWithValue("@Mahasiswa_id", mhsId);

            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();

            return result?.ToString() == "OK";
        }

        public async Task<string> GetKonsentrasiByNimAsync(string mhsId)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getKonsentrasiByNIM", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Mahasiswa_id", mhsId);

            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();

            return result?.ToString() ?? string.Empty;
        }

        public async Task<DetailDataWisuda?> GetByMahasiswaAsync(string mhsId)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getDataWisuda", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue(ParamKeyword, "");
            cmd.Parameters.AddWithValue("@WisudaStatus", "");
            cmd.Parameters.AddWithValue("@OrderBy", ColWisTanggalDaftar);
            cmd.Parameters.AddWithValue("@KonsentrasiId", "");
            cmd.Parameters.AddWithValue("@TahunAjaran", "");
            cmd.Parameters.AddWithValue(ParamRole, "ROL23");
            cmd.Parameters.AddWithValue(ParamMahasiswaId, mhsId);
            cmd.Parameters.AddWithValue("@SekretarisProdi", "");
            cmd.Parameters.AddWithValue("@StatusBerkas", "");
            cmd.Parameters.AddWithValue("@WisudaHeadId", 0);
            cmd.Parameters.AddWithValue("@PageNumber", 1);
            cmd.Parameters.AddWithValue("@PageSize", 10);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            if (!await reader.NextResultAsync())
                return null;

            if (!await reader.ReadAsync())
                return null;

            return new DetailDataWisuda
            {
                NomorPendaftaran = GetString(reader, "wis_id"),
                TanggalDaftar = GetDateTime(reader, ColWisTanggalDaftar),
                Prodi = GetString(reader, "prodi"),
                NIM = GetString(reader, "mhs_id"),
                Nama = GetString(reader, "mhs_nama"),
                KelengkapanBerkas = GetString(reader, "wis_status_berkas"),
                StatusPembayaran = GetString(reader, "wis_status"),
                TanggalBayar = GetDateTime(reader, "wis_tanggal_bayar"),
                UndanganTambahan = GetInt(reader, "wis_undangan")
            };
        }

        public async Task<GetWisudaDetailResponse?> GetDetailWisudaAsync(string wisId)
        {
            using var conn = new SqlConnection(_conn);
            using var cmd = new SqlCommand("sia_detailWisuda", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@WisudaId", wisId);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            return new GetWisudaDetailResponse
            {
                Nim = GetString(reader, "nim"),
                Nama = GetString(reader, "nama"),
                Nik = GetString(reader, "nik"),
                TempatLahir = GetString(reader, "tempatLahir"),
                TanggalLahir = GetDateTime(reader, "tanggalLahir"),
                Hp = GetString(reader, "hp"),
                Email = GetString(reader, "email"),
                VirtualAccount = GetString(reader, "virtualAccount"),
                TanggalDaftar = GetDateTime(reader, "tanggalDaftar"),
                TanggalBayar = GetDateTime(reader, "tanggalBayar"),
                StatusPembayaran = GetString(reader, "statusPembayaran"),
                StatusBerkas = GetString(reader, "statusBerkas"),
                Undangan = GetInt(reader, "undangan"),
                Nominal = GetDecimal(reader, "nominal"),
                Foto = GetString(reader, "foto"),
                Alamat = GetString(reader, "alamat")
            };
        }

        public async Task ApproveAsync(string wisudaId, string userLogin, string role, DateTime? tanggalBayar)
        {
            using var conn = new SqlConnection(_conn);
            using var cmd = new SqlCommand("sia_approveWisuda", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@WisudaId", wisudaId);
            cmd.Parameters.AddWithValue("@UserLogin", userLogin);
            cmd.Parameters.AddWithValue(ParamRole, role);
            cmd.Parameters.AddWithValue("@TanggalBayar", (object?)tanggalBayar ?? DBNull.Value);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<(string status, string fileName)> GetQrInfoAsync(string mhsId, string guid)
        {
            using var conn = new SqlConnection(_conn);
            using var cmd = new SqlCommand("sia_getQRWisuda", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue(ParamMahasiswaId, mhsId);
            cmd.Parameters.AddWithValue("@Guid", guid);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return ("ERROR", "");

            var hasil = reader["hasil"].ToString()!;

            if (hasil.StartsWith("NOT"))
                return ("NOT_ELIGIBLE", "");

            var parts = hasil.Split('#');
            return ("VALID", parts[2]);
        }

        private static string GetString(SqlDataReader reader, string column)
        {
            int ordinal = reader.GetOrdinal(column);
            return reader.IsDBNull(ordinal)
                ? string.Empty
                : reader.GetString(ordinal);
        }

        private static DateTime? GetDateTime(SqlDataReader reader, string column)
        {
            int ordinal = reader.GetOrdinal(column);
            return reader.IsDBNull(ordinal)
                ? (DateTime?)null
                : reader.GetDateTime(ordinal);
        }

        private static decimal GetDecimal(SqlDataReader reader, string column)
        {
            int ordinal = reader.GetOrdinal(column);

            if (reader.IsDBNull(ordinal))
                return 0m;

            object value = reader.GetValue(ordinal);

            return value switch
            {
                decimal d => d,
                long l => l,
                int i => i,
                short s => s,
                double db => (decimal)db,
                float f => (decimal)f,
                _ => Convert.ToDecimal(value)
            };
        }

        private static int GetInt(SqlDataReader reader, string column)
        {
            int ordinal = reader.GetOrdinal(column);

            if (reader.IsDBNull(ordinal))
                return 0;

            object value = reader.GetValue(ordinal);

            return value switch
            {
                short s => s,
                int i => i,
                long l => (int)l,
                decimal d => (int)d,
                _ => Convert.ToInt32(value)
            };
        }

        private static int? GetNullableInt(SqlDataReader reader, string column)
        {
            int ordinal = reader.GetOrdinal(column);

            if (reader.IsDBNull(ordinal))
                return null;

            object value = reader.GetValue(ordinal);

            return value switch
            {
                short s => s,
                int i => i,
                long l => (int)l,
                decimal d => (int)d,
                _ => Convert.ToInt32(value)
            };
        }
    }
}