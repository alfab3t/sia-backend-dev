using astratech_apps_backend.DTOs.Jam_Minus;
using astratech_apps_backend.DTOs.JamMinusController;
using astratech_apps_backend.Models;
using astratech_apps_backend.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace astratech_apps_backend.Repositories.Implementations
{
    public class JamMinusRepository : IJamMinusRepository
    {
        private readonly string _conn;

        public JamMinusRepository(IConfiguration config)
        {
            _conn = PolmanAstraLibrary.PolmanAstraLibrary.Decrypt(
                config.GetConnectionString("DefaultConnection")!,
                Environment.GetEnvironmentVariable("DECRYPT_KEY_CONNECTION_STRING")
            );
        }

        public async Task<int> CreateAsync(CreateJamMinus dto, string createdBy)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_createJamMinus", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@MahasiswaId", dto.Nim);
            cmd.Parameters.AddWithValue("@IdJumlah", dto.Jumlah);
            cmd.Parameters.AddWithValue("@Jenis", dto.Jenis);
            cmd.Parameters.AddWithValue("@Deskripsi", dto.Deskripsi);
            cmd.Parameters.AddWithValue("@Tanggal", dto.Tanggal.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@Tahun", dto.TahunAkademik);
            cmd.Parameters.AddWithValue("@Semester", dto.Semester);
            cmd.Parameters.AddWithValue("@CreatedBy", createdBy);

            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync();
        }

        public async Task<(IEnumerable<JamMinus>, int totalData)> GetAllAsync(GetAllJamMinusRequest dto)
        {
            var list = new List<JamMinus>();
            int totalData = 0;

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getDataJamMinus", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@PageNumber", dto.Page);
            cmd.Parameters.AddWithValue("@PageSize", dto.PageSize);
            cmd.Parameters.AddWithValue("@Username", dto.Username ?? "");
            cmd.Parameters.AddWithValue("@Keyword", dto.Keyword ?? "");
            cmd.Parameters.AddWithValue("@Sortingby", dto.Sort ?? "MhsId asc");
            cmd.Parameters.AddWithValue("@FilterKonID", dto.Prodi ?? "");
            cmd.Parameters.AddWithValue("@TahunAjaran", dto.TahunAkademik);
            cmd.Parameters.AddWithValue("@Semester", dto.Semester);
            cmd.Parameters.AddWithValue("@FilterKelas", dto.Kelas ?? "");
            cmd.Parameters.AddWithValue("@RoleID", dto.RoleID ?? "");

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                if (totalData == 0 && reader["Count"] != DBNull.Value)
                    totalData = Convert.ToInt32(reader["Count"]);

                var item = new JamMinus();

                item.mahasiswaID = reader.GetString(reader.GetOrdinal("MhsId"));
                item.Nama = reader.GetString(reader.GetOrdinal("Nama"));
                item.Prodi = reader.GetString(reader.GetOrdinal("Prodi"));
                item.Kelas = reader.GetString(reader.GetOrdinal("Kelas"));
                item.TahunAkademik = reader.GetString(reader.GetOrdinal("TahunAkademik"));
                item.Semester = reader.GetString(reader.GetOrdinal("Semester"));
                item.TotalJamMinus = Convert.ToDecimal(reader["TotalJamMinus"]);

                list.Add(item);
            }

            return (list, totalData);
        }

        public async Task<IEnumerable<JamMinus>> GetAllAsync(string id, string tahunAjaran, string semester)
        {
            var list = new List<JamMinus>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getListJamMinus", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@TahunAjaran", tahunAjaran);
            cmd.Parameters.AddWithValue("@Semester", semester);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new JamMinus
                {
                    IdJamMinus = reader.GetInt32(reader.GetOrdinal("JumlahId")),
                    Deskripsi = reader.GetString(reader.GetOrdinal("Deskripsi")),
                    TotalJamMinus = reader.GetInt32(reader.GetOrdinal("Jumlah")),
                    Tanggal = reader.GetString(reader.GetOrdinal("Tanggal")),
                    Jenis = reader.GetString(reader.GetOrdinal("Jenis")),
                });
            }

            return list;
        }

        public async Task<JamMinus?> GetByIdAsync(string id, string tahunAjaran, string semester)
        {
            JamMinus? item = null;

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getListJamMinus", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            
            cmd.Parameters.AddWithValue("Id", id);
            cmd.Parameters.AddWithValue("TahunAjaran", tahunAjaran);
            cmd.Parameters.AddWithValue("Semester", semester);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                item = new JamMinus
                {
                   
                    IdJamMinus = reader.GetInt32(reader.GetOrdinal("JumlahId")),
                    Deskripsi = reader.GetString(reader.GetOrdinal("Deskripsi")),
                    TotalJamMinus = decimal.Parse(reader.GetString(reader.GetOrdinal("Jumlah"))),
                };
            }

            return item;
        }


        public async Task<bool> UpdateAsync(UpdateJamMinusRequest dto, string updatedBy)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_editJamMinus", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Id", dto.Id);
            cmd.Parameters.AddWithValue("@Jumlah", dto.JumlahJam.ToString());
            cmd.Parameters.AddWithValue("@Jenis", dto.Jenis);
            cmd.Parameters.AddWithValue("@Deskripsi", dto.Deskripsi);
            cmd.Parameters.AddWithValue("@ModifyBy", updatedBy);

            await conn.OpenAsync();
            var rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(DeleteJamMinusRequest dto, string modifBy)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_deleteJamMinus", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Id", dto.Id);
            cmd.Parameters.AddWithValue("@ModifBy", modifBy);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
            return true;
        }

        public async Task<List<GetListKonsentrasiDto>> GetListKonsentrasiAsync(string username, string roleId)
        {
            var list = new List<GetListKonsentrasiDto>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getListKonsentrasi", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Username", username);
            cmd.Parameters.AddWithValue("@RoleId", roleId);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new GetListKonsentrasiDto
                {
                    Username = Convert.ToString(reader["kon_id"]) ?? "",
                    Rol = reader.GetString(reader.GetOrdinal("kon_nama")) ?? ""
                });
            }

            return list;
        }

        public async Task<List<GetListKelasDto>> GetListKelasAsync(string idProdi, string tahunAjaran)
        {
            var kelas = new List<GetListKelasDto>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getListKelasByKonsentrasi", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@IdProdi", idProdi);
            cmd.Parameters.AddWithValue("@TahunAkademik", tahunAjaran);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                kelas.Add(new GetListKelasDto
                {
                    KelasId = reader.GetString(reader.GetOrdinal("kel_id")),
                    KelasNama = reader.GetString(reader.GetOrdinal("kel_nama"))
                });
            }

            return kelas;
        }

        public async Task<List<MahasiswaKelasDto>> GetListMahasiswaAsync(string idKelas)
        {
            var mahasiswa = new List<MahasiswaKelasDto>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getListMahasiswaByKelas", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Id", idKelas);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                mahasiswa.Add(new MahasiswaKelasDto
                {
                    mhs_id = reader.GetString(reader.GetOrdinal("mhs_id")),
                    mhs_nama = reader.GetString(reader.GetOrdinal("mhs_nama"))
                });
            }

            return mahasiswa;
        }

        public async Task<TahunAkademikDto> GetActiveTahunAkademikAsync(DateTime dateToCheck)
        {
            TahunAkademikDto result = new TahunAkademikDto();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getActiveTahunAkademikByTanggal", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@DateToCheck", dateToCheck);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                result = new TahunAkademikDto
                {
                    kak_tahun_ajaran = reader.GetString(reader.GetOrdinal("kak_tahun_ajaran")),
                    kak_ganjil_genap = reader.GetString(reader.GetOrdinal("kak_ganjil_genap"))
                };
            }

            return result;
        }
    }
}
