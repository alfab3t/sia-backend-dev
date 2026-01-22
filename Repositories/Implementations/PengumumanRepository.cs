using astratech_apps_backend.DTOs.Pengumuman;
using astratech_apps_backend.Models;
using astratech_apps_backend.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace astratech_apps_backend.Repositories.Implementations
{
    public class PengumumanRepository(IConfiguration config) : IPengumumanRepository
    {
        private readonly string _conn = PolmanAstraLibrary.PolmanAstraLibrary.Decrypt(config.GetConnectionString("DefaultConnection")!, Environment.GetEnvironmentVariable("DECRYPT_KEY_CONNECTION_STRING"));
        private const string idAplikasi = "@IdAplikasi";
        private const string idPengumuman = "@IdPengumuman";
        private const string diubahOleh = "@DiubahOleh";

        public async Task<int> CreatePengumumanRepo(CreatePengumumanRequest dto, string IdRole, string DibuatOleh)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("all_createPengumuman", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@IdRolePengguna", IdRole);
            cmd.Parameters.AddWithValue(idAplikasi, dto.IdAplikasi);
            cmd.Parameters.AddWithValue("@KpdPengumuman", dto.KpdPengumuman);
            cmd.Parameters.AddWithValue("@SubyekPengumuman", dto.SubyekPengumuman);
            cmd.Parameters.AddWithValue("@IsiPengumuman", dto.IsiPengumuman);
            cmd.Parameters.AddWithValue("@StatusBaca", dto.StatusBaca);
            cmd.Parameters.AddWithValue("@TanggalMulaiPengumuman", dto.TanggalMulaiPengumuman);
            cmd.Parameters.AddWithValue("@TanggalSelesaiPengumuman", dto.TanggalSelesaiPengumuman);
            cmd.Parameters.AddWithValue("@DibuatOleh", DibuatOleh);

            await conn.OpenAsync();
            var newId = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(newId);
        }
        public async Task<(IEnumerable<Pengumuman>, int totalData)> GetAllDataPengumumanRepo(GetDataPengumumanRequest dto)
        {
            var list = new List<Pengumuman>();
            int totalData = 0;

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("all_getDataPengumuman", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Filter", dto.Filter);
            cmd.Parameters.AddWithValue("@InputCari", dto.InputCari);
            cmd.Parameters.AddWithValue("@Halaman", dto.Halaman);
            cmd.Parameters.AddWithValue("@Limit", dto.Limit);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                do
                {
                    totalData = reader.GetInt32(reader.GetOrdinal("Count"));
                    list.Add(new Pengumuman
                    {
                        IdPengumuman = reader.GetInt32(reader.GetOrdinal("id")),
                        RowNumber = reader.GetInt64(reader.GetOrdinal("rownum")),
                        IdAplikasi = reader.GetString(reader.GetOrdinal("app")),
                        TanggalPengumumanF = reader.GetString(reader.GetOrdinal("tanggal")),
                        SubyekPengumuman = reader.GetString(reader.GetOrdinal("subyek")),
                        StatusPengumuman = reader.GetString(reader.GetOrdinal("status")),
                        TanggalMulaiPengumuman = reader.GetDateTime(reader.GetOrdinal("date")),
                    });
                } while (await reader.ReadAsync());
            }
            return (list, totalData);
        }
        public async Task<bool> UpdatePegumumanRepo(UpdatePengumumanRequest dto, string DiubahOleh)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("all_editPengumuman", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue(idPengumuman, dto.IdPengumuman);
            cmd.Parameters.AddWithValue(idAplikasi, dto.IdAplikasi);
            cmd.Parameters.AddWithValue("@KpdPengumuman", dto.KpdPengumuman);
            cmd.Parameters.AddWithValue("@SubyekPengumuman", dto.SubyekPengumuman);
            cmd.Parameters.AddWithValue("@IsiPengumuman", dto.IsiPengumuman);
            cmd.Parameters.AddWithValue("@StatusBaca", dto.StatusBaca);
            cmd.Parameters.AddWithValue("@TanggalMulaiPengumuman", dto.TanggalMulaiPengumuman);
            cmd.Parameters.AddWithValue("@TanggalSelesaiPengumuman", dto.TanggalSelesaiPengumuman);
            cmd.Parameters.AddWithValue(diubahOleh, DiubahOleh);

            await conn.OpenAsync();
            var rowsAffected = await cmd.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<bool> SetStatusHapusPengumuman(int IdPengumuman, string DiubahOleh)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("all_deletePengumuman", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue(idPengumuman, IdPengumuman);
            cmd.Parameters.AddWithValue(diubahOleh, DiubahOleh);

            await conn.OpenAsync();
            var rowsAffected = await cmd.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<(IEnumerable<Pengumuman>, int totalData)> GetDataPengumumanRepo(GetDataPengumumanRequest dto, string DibuatOleh, string IdAplikasi)
        {
            var list = new List<Pengumuman>();
            var totalData = 0;

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("all_getDataPengumumanByUser", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Filter", dto.Filter);
            cmd.Parameters.AddWithValue(idAplikasi, dto.IdAplikasi);
            cmd.Parameters.AddWithValue("@DibuatOleh", DibuatOleh);
            cmd.Parameters.AddWithValue("@InputCari", dto.InputCari);
            cmd.Parameters.AddWithValue("@Halaman", dto.Halaman);
            cmd.Parameters.AddWithValue("@Limit", dto.Limit);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                do
                {
                    totalData = reader.GetInt32(reader.GetOrdinal("Count"));
                    list.Add(new Pengumuman
                    {
                        IdPengumuman = reader.GetInt32(reader.GetOrdinal("id")),
                        RowNumber = reader.GetInt64(reader.GetOrdinal("rownum")),
                        IdAplikasi = reader.GetString(reader.GetOrdinal("app")),
                        TanggalPengumumanF = reader.GetString(reader.GetOrdinal("tanggal")),
                        SubyekPengumuman = reader.GetString(reader.GetOrdinal("subyek")),
                        StatusPengumuman = reader.GetString(reader.GetOrdinal("status")),
                        TanggalMulaiPengumuman = reader.GetDateTime(reader.GetOrdinal("date")),
                    });
                } while (await reader.ReadAsync());
            }
            return (list, totalData);
        }
        public async Task<GetDetailPengumumanResponse?> GetDetailPengumumanRepo(int IdPengumuman)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("all_detailPengumuman", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue(idPengumuman, IdPengumuman);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new GetDetailPengumumanResponse
                {
                    IdPengumuman = reader.GetInt32(reader.GetOrdinal("id")),
                    IdAplikasi = reader.GetString(reader.GetOrdinal("appid")),
                    NamaAplikasi = reader.GetString(reader.GetOrdinal("appnama")),
                    SubyekPengumuman = reader.GetString(reader.GetOrdinal("subyek")),
                    KpdPengumuman = reader.GetString(reader.GetOrdinal("untuk")),
                    TanggalMulaiPengumuman = reader.GetString(reader.GetOrdinal("dari")),
                    TanggalSelesaiPengumuman = reader.GetString(reader.GetOrdinal("sampai")),
                    TanggalMulaiPengumumanIndo = reader.GetString(reader.GetOrdinal("darif")),
                    TanggalSelesaiPengumumanIndo = reader.GetString(reader.GetOrdinal("sampaif")),
                    StatusBaca = reader.GetByte(reader.GetOrdinal("wajib")),
                    IsiPengumuman = reader.GetString(reader.GetOrdinal("pengumuman"))
                };
            }
            return null;
        }

        public async Task<bool> HidePengumumanRepo(int IdPengumuman, string DiubahOleh)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("all_hidePengumuman", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue(idPengumuman, IdPengumuman);
            cmd.Parameters.AddWithValue(diubahOleh, DiubahOleh);

            await conn.OpenAsync();
            var rowsAffected = await cmd.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<bool> ShowPengumumanRepo(int IdPengumuman, string DiubahOleh)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("all_showPengumuman", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue(idPengumuman, IdPengumuman);
            cmd.Parameters.AddWithValue(diubahOleh, DiubahOleh);

            await conn.OpenAsync();
            var rowsAffected = await cmd.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<IEnumerable<string>> GetListRoleRepo(string IdPengguna, string IdRole)
        {
            var listRole = new List<string>();
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("all_getListRoleAllow", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@IdPengguna", IdPengguna);
            cmd.Parameters.AddWithValue("@IdRole", IdRole);
            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                string data = reader.GetString(reader.GetOrdinal("upe_batasan"));
                listRole.Add(data);
            }
            return listRole;
        }

        public async Task<IEnumerable<Aplikasi>> GetListAplikasiRepo()
        {
            var listAplikasi = new List<Aplikasi>();
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("all_getListAplikasi", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                listAplikasi.Add(new Aplikasi
                {
                    AppId = reader.GetString(reader.GetOrdinal("app_id")),
                    NamaAplikasi = reader.GetString(reader.GetOrdinal("app_deskripsi"))
                });
            }
            return listAplikasi;
        }

        public async Task<IEnumerable<Aplikasi>> GetListRoleByAplikasiRepo(string IdAplikasi)
        {
            var listRole = new List<Aplikasi>();
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("all_getListRoleByAplikasi", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue(idAplikasi, IdAplikasi);
            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();


            if (await reader.ReadAsync())
            {
                do
                {
                    listRole.Add(new Aplikasi
                    {
                        RoleId = reader.GetString(reader.GetOrdinal("rol_id")),
                        NamaAplikasi = reader.GetString(reader.GetOrdinal("rol_deskripsi")),
                        AppId = reader.GetString(reader.GetOrdinal("app_id"))
                    });
                } while (await reader.ReadAsync());
            }
            return listRole;
        }
    }
}
