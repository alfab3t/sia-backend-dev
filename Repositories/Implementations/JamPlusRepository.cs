using astratech_apps_backend.DTOs.JamPlus;
using astratech_apps_backend.DTOs.JamPlusController;
using astratech_apps_backend.Models;
using astratech_apps_backend.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using System;
using System.Data;


namespace astratech_apps_backend.Repositories.Implementations
{
    public class JamPlusRepository(IConfiguration config) : IJamPlusRepository
    {
        private readonly string _conn = PolmanAstraLibrary.PolmanAstraLibrary.Decrypt(config.GetConnectionString("DefaultConnection")!, Environment.GetEnvironmentVariable("DECRYPT_KEY_CONNECTION_STRING"));
        public async Task<int> CreateAsync(CreateJamplus dto, string createdBy)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_createJamPlus", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@MahasiswaId", dto.MahasiswaId);
            cmd.Parameters.AddWithValue("@Jumlah", dto.Jumlah);
            cmd.Parameters.AddWithValue("@Jenis", dto.Jenis);
            cmd.Parameters.AddWithValue("@Deskripsi", dto.Deskripsi);
            cmd.Parameters.AddWithValue("@Tanggal", dto.Tanggal.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@Tahun", dto.TahunAkademik);
            cmd.Parameters.AddWithValue("@Semester", dto.Semester);
            cmd.Parameters.AddWithValue("@CreatedBy", createdBy);

            await conn.OpenAsync();
            var newId = await cmd.ExecuteNonQueryAsync();
            return Convert.ToInt32(newId);
        }


        public async Task<(IEnumerable<JamPlus>, int totalData)> GetAllAsync(GetAllJamPlusRequest dto)
        {
            var list = new List<JamPlus>();
            int totalData = 0;

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getDataJamPlus", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Username", dto.Username);
            cmd.Parameters.AddWithValue("@Keyword", dto.Keyword ?? string.Empty);
            cmd.Parameters.AddWithValue("@Sortingby", dto.Sort);
            cmd.Parameters.AddWithValue("@FilterKonID", dto.Prodi);
            cmd.Parameters.AddWithValue("@TahunAjaran", dto.TahunAkademik);
            cmd.Parameters.AddWithValue("@Semester", dto.Semester);
            cmd.Parameters.AddWithValue("@FilterKelas", dto.Kelas);
            cmd.Parameters.AddWithValue("@RoleID", dto.RoleID);
            cmd.Parameters.AddWithValue("@PageNumber", dto.Page);
            cmd.Parameters.AddWithValue("@PageSize", dto.PageSize);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                if (!reader.IsDBNull(reader.GetOrdinal("Count")))
                {
                    totalData = reader.GetInt32(reader.GetOrdinal("Count"));
                }
                do
                {
                    var item = new JamPlus();

                    item.MahasiswaId = reader.GetString(reader.GetOrdinal("mhs_id"));
                    item.Nama = reader.GetString(reader.GetOrdinal("mhs_nama"));
                    item.Prodi = reader.GetString(reader.GetOrdinal("prodi"));
                    item.Kelas = reader.GetString(reader.GetOrdinal("kelas"));
                    if (!reader.IsDBNull(reader.GetOrdinal("jamplus")))
                    {
                        item.TotalJamPlus = Convert.ToDecimal(reader["jamplus"]);
                    }
                    else
                    {
                        item.TotalJamPlus = 0;
                    }

                    list.Add(item);

                } while (await reader.ReadAsync());
            }

            return (list, totalData);
        }

        public async Task<IEnumerable<JamPlus>> GetAllAsync(string id, string tahunAjaran, string semester)
        {
            var list = new List<JamPlus>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getListJamPlus", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@TahunAjaran", tahunAjaran);
            cmd.Parameters.AddWithValue("@Semester", semester);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new JamPlus
                {
                    IdJamPlus = reader.GetInt32(reader.GetOrdinal("jpl_id")),
                    Deskripsi = reader.GetString(reader.GetOrdinal("jpl_deskripsi")),
                    TotalJamPlus = decimal.Parse(reader.GetString(reader.GetOrdinal("jpl_jumlah"))),
                    Tanggal = reader.GetString(reader.GetOrdinal("tanggal")),
                    Jenis = reader.GetString(reader.GetOrdinal("jpl_jenis"))
                });
            }
            return list;
        }

        public async Task<JamPlus?> GetByIdAsync(string id, string tahunAjaran, string semester)
        {
            JamPlus? item = null;

            await using var conn = new SqlConnection(_conn);

            await using var cmd = new SqlCommand("sia_getListJamPlus", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@TahunAjaran", tahunAjaran);
            cmd.Parameters.AddWithValue("@Semester", semester);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                item = new JamPlus
                {
                    
                    IdJamPlus = reader.IsDBNull(reader.GetOrdinal("jpl_id")) ? 0 : reader.GetInt32(reader.GetOrdinal("jpl_id")),

                    Deskripsi = reader.IsDBNull(reader.GetOrdinal("jpl_deskripsi")) ? string.Empty : reader.GetString(reader.GetOrdinal("jpl_deskripsi")),
                    TotalJamPlus = decimal.Parse(reader.GetString(reader.GetOrdinal("jpl_jumlah"))),

                };
            }

            return item;
        }
        public async Task<bool> UpdateAsync(UpdateJamPlusRequest dto, string updatedBy)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_editJamPlus", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Id", dto.Id);
            cmd.Parameters.AddWithValue("@Jumlah", dto.JumlahJam.ToString());
            cmd.Parameters.AddWithValue("@Jenis", dto.Jenis);
            cmd.Parameters.AddWithValue("@Deskripsi", dto.Deskripsi);
            cmd.Parameters.AddWithValue("@ModifyBy", updatedBy);
            await conn.OpenAsync();
            var rowsAffected = await cmd.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
        public async Task<List<GetListKonsentrasiDto>> GetListKonsentrasiAsync(string username, string roleId)
        {
            var list = new List<GetListKonsentrasiDto>();

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
            await using var cmd = new SqlCommand("sia_getListKelasByKonsentrasi", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

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

        public async Task<bool> DeleteJamPlusAsync(DeleteJamPlus dto, string modifBy)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_deleteJamPlus", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Id", dto.Id);
            cmd.Parameters.AddWithValue("@ModifBy", modifBy);
            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
            return true;
        }

        public async Task<List<MahasiswaKelasDto>> GetListMahasiswaAsync(string idKelas)
        {
            var mahasiswa = new List<MahasiswaKelasDto>();

            await using var conn = new SqlConnection(_conn);

            await conn.OpenAsync();

            await using var cmd = new SqlCommand("sia_getListMahasiswaByKelas", conn);
            cmd.CommandType = CommandType.StoredProcedure;


            cmd.Parameters.AddWithValue("@id", idKelas);

            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                mahasiswa.Add(new MahasiswaKelasDto
                {
                    IdMahasiswa= reader.GetString(reader.GetOrdinal("mhs_id")),
                    NamaMahasiswa = reader.GetString(reader.GetOrdinal("mhs_nama"))
                    
                });
            }
            return mahasiswa;

        }

        private async Task<string?> GetMahasiswaIdByNim(string nim, string tahunAjaran, string semester)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getListJamPlus", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Id", nim);               
            cmd.Parameters.AddWithValue("@TahunAjaran", tahunAjaran);
            cmd.Parameters.AddWithValue("@Semester", semester);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return reader.GetString(reader.GetOrdinal("mhs_id"));
            }

            return null;
        }


        public async Task<TahunAkademikDto> GetActiveTahunAkademikAsync(DateTime dateToCheck)
        {
            TahunAkademikDto? result = new TahunAkademikDto { };
            await using var conn = new SqlConnection(_conn); 

            await conn.OpenAsync();

            await using var cmd = new SqlCommand("sia_getActiveTahunAkademikByTanggal", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            
            cmd.Parameters.AddWithValue("@DateToCheck", dateToCheck);

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