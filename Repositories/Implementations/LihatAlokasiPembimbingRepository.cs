    using astratech_apps_backend.DTOs.LihatAlokasiPembimbing;
    using astratech_apps_backend.Models.LihatAlokasiPembimbing;
    using astratech_apps_backend.Repositories.Interfaces;
    using Microsoft.Data.SqlClient;
    using System.Data;

    namespace astratech_apps_backend.Repositories.Implementations
    {
        public class LihatAlokasiPembimbingRepository : ILihatAlokasiPembimbingRepository
        {
            private readonly string _conn;

            public LihatAlokasiPembimbingRepository (IConfiguration config)
            {
                _conn = PolmanAstraLibrary.PolmanAstraLibrary.Decrypt(
                    config.GetConnectionString("DefaultConnection")!,
                    Environment.GetEnvironmentVariable("DECRYPT_KEY_CONNECTION_STRING")
                );
            }

        public async Task<(IEnumerable<LihatAlokasiPembimbing>, int totalData)> GetAllAsync(GetAllLihatAlokasiPembimbingRequest dto)
        {
            var list = new List<LihatAlokasiPembimbing>();
            int totalData = 0;

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getDataPengajuanProposalTA", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Username", dto.Username ?? "");
            cmd.Parameters.AddWithValue("@Keyword", dto.SearchKeyword ?? "");
            cmd.Parameters.AddWithValue("@OrderBy", dto.Urut);
            cmd.Parameters.AddWithValue("@TahunAkademik", dto.TahunAkademik);
            cmd.Parameters.AddWithValue("@KonsentrasiId", dto.Konsentrasi ?? "");
            cmd.Parameters.AddWithValue("@StatusBimbingan", dto.Status ?? "");
            cmd.Parameters.AddWithValue("@RoleId", dto.Role ?? "");
            cmd.Parameters.AddWithValue("@SekProdi", dto.SekretarisProdi ?? "");
            cmd.Parameters.AddWithValue("@Halaman", dto.PageNumber);
            cmd.Parameters.AddWithValue("@Limit", dto.PageSize);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                totalData = reader.GetInt32(reader.GetOrdinal("Count"));
                do
                {
                    list.Add(new LihatAlokasiPembimbing
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("id")),
                        RowNumber = reader.GetInt64(reader.GetOrdinal("rownum")),
                        Tanggal = reader.GetString(reader.GetOrdinal("tanggal")),
                        Konsentrasi = reader.GetString(reader.GetOrdinal("konsentrasi")),
                        Industri = reader.GetString(reader.GetOrdinal("industri")),
                        NamaKelompok = reader.GetString(reader.GetOrdinal("kelompok")),
                        AnggotaKelompok = reader.GetString(reader.GetOrdinal("anggota")),
                        Status = reader.GetString(reader.GetOrdinal("status")),
                        JudulProposalAktual = reader.GetString(reader.GetOrdinal("judulaktual")),
                        NamaDosenPembimbing = reader.GetString(reader.GetOrdinal("namados")),
                    });
                } while (await reader.ReadAsync());
            }
            return (list, totalData);
        }

        public async Task<DataTable> ExportTA(string tahunAkademik, string konsentrasiId)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_exportKelompokTA2", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@TahunAkademik", (object)tahunAkademik ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@KonsentrasiId", (object)konsentrasiId ?? DBNull.Value);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

            var dataTable = new DataTable();
            dataTable.Load(reader);

            return dataTable;
        }

        public async Task<IEnumerable<Konsentrasi>> GetListKonsentrasiAsync(string username, string roleId)
        {
            var list = new List<Konsentrasi>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getListKonsentrasi", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@username", (object)username ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@roleId", (object)roleId ?? DBNull.Value);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

            while (await reader.ReadAsync())
            {
                var item = new Konsentrasi();

                try
                {
                    item.KonsentrasiId = Convert.ToInt32(reader["kon_id"]);
                    item.NamaKonsentrasi = reader.GetString(reader.GetOrdinal("kon_nama"));
                }
                catch 
                {
                    continue;
                }

                list.Add(item);
            }

            return list;
        }

        public async Task<IEnumerable<KonsentrasiByNPK>> GetListKonsentrasiByNPKAsync(GetListKonsentrasiByNPKRequest request)
        {
            var list = new List<KonsentrasiByNPK>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getListKonsentrasiByNPK", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@UserRole", (object)request.Role ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Username", (object)request.Username ?? DBNull.Value);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

            while (await reader.ReadAsync())
            {
                list.Add(new KonsentrasiByNPK
                {
                    KonsentrasiId = Convert.ToInt32(reader["kon_id"]),
                    NamaKonsentrasi = reader["kon_nama"]?.ToString() ?? string.Empty
                });
            }
            return list;
        }

        public async Task<IEnumerable<KonsentrasiByDosen>> GetListKonsentrasiByDosenAsync(string username)
        {
            var list = new List<KonsentrasiByDosen>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getListKonsentrasiByDosen", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Username", (object)username ?? DBNull.Value);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

            while (await reader.ReadAsync())
            {
                list.Add(new KonsentrasiByDosen
                {
                    KonsentrasiId = Convert.ToInt32(reader["kon_id"]),
                    NamaKonsentrasi = reader["kon_nama"]?.ToString() ?? string.Empty
                });
            }

            return list;
        }

        public async Task<TahunAkademikAktif?> GetActiveTahunAkademikAsync()
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getActiveTahunAkademikByTanggal", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@DateToCheck", DBNull.Value);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                int idxTahun = reader.GetOrdinal("kak_tahun_ajaran");
                int idxSemester = reader.GetOrdinal("kak_ganjil_genap");
                int idxTglFrom = reader.GetOrdinal("kak_tgl_from");

                return new TahunAkademikAktif
                {
                    TahunAjaran = await reader.IsDBNullAsync(idxTahun) ? "" : reader.GetString(idxTahun),
                    Semester = await reader.IsDBNullAsync(idxSemester) ? "" : reader.GetString(idxSemester),
                    TanggalMulai = await reader.IsDBNullAsync(idxTglFrom)
                                   ? DateTime.MinValue
                                   : reader.GetDateTime(idxTglFrom)
                };
            }

            return null;
        }
    }
}
