using astratech_apps_backend.DTOs.WaliMahasiswa;
using astratech_apps_backend.Models;
using astratech_apps_backend.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace astratech_apps_backend.Repositories.Implementations
{
    public class WaliMhsRepository : IWaliMhsRepository
    {
        private readonly string _conn;

        private const string ParamDosenIdUpper = "@DosenId";
        private const string ParamProdiIdUpper = "@ProdiId";
        private const string ParamAngkatanUpper = "@Angkatan";
        private const string ParamUpdatedBy = "@UpdatedBy";
        
        private const string ColDosId = "dos_id";
        private const string ColAngkatan = "mhs_angkatan";
        private const string ColDosNama = "dos_nama";
        private const string ColKonSingkatan = "kon_singkatan";
        private const string ColStatus = "dwa_status";
        private const string ColKonId = "kon_id";      
        private const string ColKonNama = "kon_nama";
        

        private const string ColMhsId = "mhs_id";
        private const string ColMhsNama = "mhs_nama";
        private const string ColKelId = "kel_id";

        public WaliMhsRepository(IConfiguration config)
        {
            _conn = PolmanAstraLibrary.PolmanAstraLibrary.Decrypt(
                config.GetConnectionString("DefaultConnection")!,
                Environment.GetEnvironmentVariable("DECRYPT_KEY_CONNECTION_STRING")
            );
        }
         public async Task<int> CreateAsync(CreateWaliMhsRequest dto, string createdBy)
        {
            if (dto.MahasiswaIdsToCreate == null || !dto.MahasiswaIdsToCreate.Any())
                throw new ArgumentException("List mahasiswa tidak boleh kosong.");

            int successCount = 0;

            await using var conn = new SqlConnection(_conn);
            await conn.OpenAsync();

            foreach (var mahasiswa in dto.MahasiswaIdsToCreate)
            {
                await using var cmd = new SqlCommand("sia_createDosenWali", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };


                cmd.Parameters.AddWithValue(ParamDosenIdUpper, dto.DosenId);
                cmd.Parameters.AddWithValue("@MahasiswaId", mahasiswa);
                cmd.Parameters.AddWithValue(ParamProdiIdUpper, dto.Prodi);
                cmd.Parameters.AddWithValue("@CreatedBy", createdBy);

                try
                {
                    await cmd.ExecuteNonQueryAsync();
                    successCount++;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"Gagal assign id mahasiswa {mahasiswa}: {ex.Message}");
                }
            }

            return successCount;
        }


        public async Task<IEnumerable<MahasiswaItem>> GetMahasiswaAvailableAsync(string angkatan, int prodiId)
        {
            await using var conn = new SqlConnection(_conn);
            await conn.OpenAsync();

            var resultList = new List<MahasiswaItem>();

            await using var cmd = new SqlCommand("sia_getDataDosenWaliDetail", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Mode", "Add");
            cmd.Parameters.AddWithValue(ParamAngkatanUpper, angkatan);
            cmd.Parameters.AddWithValue(ParamProdiIdUpper, prodiId);

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                resultList.Add(new MahasiswaItem
                {
                    NIM = reader[ColMhsId].ToString()!,
                    Nama = reader[ColMhsNama].ToString()!,
                    Kelas = reader[ColKelId].ToString()!
                });
            }

            return resultList;
        }


        public async Task<bool> UpdateAsync(int dosenId, string angkatan, int prodiId, string status, UpdateWaliMhsRequest dto, string updatedBy)
        {
            var existingData = await GetDetailAsync(dosenId, angkatan, prodiId, status); 
            if (existingData == null) return false;

            
            string angkatanLama = existingData.Angkatan; 

            await using var conn = new SqlConnection(_conn);
            await conn.OpenAsync();

            
           await using (var cmdDelete = new SqlCommand("sia_deleteDosenWali", conn))
            {
                cmdDelete.CommandType = CommandType.StoredProcedure;
                cmdDelete.Parameters.AddWithValue(ParamDosenIdUpper, existingData.DosenId);
                cmdDelete.Parameters.AddWithValue(ParamProdiIdUpper, existingData.Prodi);
                cmdDelete.Parameters.AddWithValue(ParamAngkatanUpper, angkatanLama);

                await cmdDelete.ExecuteNonQueryAsync();
            }
            
                foreach (var mahasiswa in dto.MahasiswasIds)
            {
                await using var cmdCreate = new SqlCommand("sia_createDosenWali", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmdCreate.Parameters.AddWithValue(ParamDosenIdUpper, dto.DosenId);
                cmdCreate.Parameters.AddWithValue("@MahasiswaId", mahasiswa);
                cmdCreate.Parameters.AddWithValue(ParamProdiIdUpper, dto.Prodi);
                cmdCreate.Parameters.AddWithValue("@CreatedBy", updatedBy);

                await cmdCreate.ExecuteNonQueryAsync();
            }
            return true; 
        }
           
            public async Task<bool> DeleteAsync(int dosenId, string angkatan, int prodiId, string status)
            {
                var data = await GetDetailAsync(dosenId, angkatan, prodiId, status);
                if (data == null) return false;

                await using var conn = new SqlConnection(_conn);
                await using var cmd = new SqlCommand("sia_deleteDosenWali", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue(ParamDosenIdUpper, data.DosenId);
                cmd.Parameters.AddWithValue(ParamProdiIdUpper, data.Prodi);
                cmd.Parameters.AddWithValue(ParamAngkatanUpper, data.Angkatan);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                return true;
            }

            public async Task<bool> SendApprovalAsync(int dosenId, string angkatan, int prodiId, string status, string updatedBy)
            {
                var data = await GetDetailAsync(dosenId, angkatan, prodiId, status);
                if (data == null) return false;

                
                if (data.Status != "Draft" && data.Status != "Revisi")
                    return false;

                await using var conn = new SqlConnection(_conn);
                await using var cmd = new SqlCommand("sia_sentDosenWali", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue(ParamDosenIdUpper, data.DosenId);
                cmd.Parameters.AddWithValue(ParamProdiIdUpper, data.Prodi);
                cmd.Parameters.AddWithValue(ParamAngkatanUpper, data.Angkatan);
                cmd.Parameters.AddWithValue(ParamUpdatedBy, updatedBy);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                return true;

            }

            public async Task<bool> ApproveAsync(int dosenId, string angkatan, int prodiId, string status, string updatedBy)
            {
                var data = await GetDetailAsync(dosenId, angkatan, prodiId, status);
                if (data == null) return false;

                await using var conn = new SqlConnection(_conn);
                await using var cmd = new SqlCommand("sia_approveDosenWali", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue(ParamDosenIdUpper, data.DosenId);
                cmd.Parameters.AddWithValue(ParamProdiIdUpper, data.Prodi);
                cmd.Parameters.AddWithValue(ParamAngkatanUpper, data.Angkatan);
                cmd.Parameters.AddWithValue(ParamUpdatedBy, updatedBy);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                return true;
            }

        public async Task<bool> RejectAsync(int dosenId, string angkatan, int prodiId, string status, string reason, string updatedBy)
        {
            var data = await GetDetailAsync(dosenId, angkatan, prodiId, status);
            if (data == null) return false;

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_rejectDosenWali", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue(ParamDosenIdUpper, data.DosenId);
            cmd.Parameters.AddWithValue(ParamProdiIdUpper, data.Prodi);
            cmd.Parameters.AddWithValue(ParamAngkatanUpper, data.Angkatan);
            cmd.Parameters.AddWithValue(ParamUpdatedBy, updatedBy);
            

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
            return true;
        }

        public async Task<(IEnumerable<WaliMahasiswa> list, int totalData)> GetAllAsync(GetAllWaliMhsRequest dto)
        {
            var list = new List<WaliMahasiswa>();
            int totalData = 0;

            string status = dto.Status == "Semua" ? "" : dto.Status ?? "";
            string angkatan = dto.Angkatan == "Semua" ? "" : dto.Angkatan ?? "";
            string Prodi = dto.ProdiId == "Semua" ? "" : dto.ProdiId ?? "";

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getDataDosenWali", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@SearchKeyword", dto.SearchKeyword ?? "");
            cmd.Parameters.AddWithValue("@Status", status);
            cmd.Parameters.AddWithValue("@Urut", dto.Urut ?? "dos_nama");
            cmd.Parameters.AddWithValue("@Angkatan", angkatan);
            cmd.Parameters.AddWithValue("@Role", dto.Role ?? "");
            cmd.Parameters.AddWithValue(ParamProdiIdUpper, Prodi);
            cmd.Parameters.AddWithValue("@Username", dto.Username ?? "");
            cmd.Parameters.AddWithValue("@Halaman", dto.PageNumber);
            cmd.Parameters.AddWithValue("@Limit", dto.PageSize);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                totalData = totalData == 0
                    ? reader.GetInt32(reader.GetOrdinal("Count"))
                    : totalData;

                list.Add(new WaliMahasiswa
                {
                    DosenId = Convert.ToInt32(reader[ColDosId]),
                    prodiId = Convert.ToInt32(reader[ColKonId]),
                    NamaDosen = reader[ColDosNama]?.ToString() ?? "",
                    NamaProdi = reader[ColKonSingkatan]?.ToString() ?? "",
                    Angkatan = reader[ColAngkatan]?.ToString() ?? "",
                    Status = reader[ColStatus]?.ToString() ?? ""
                });
            }
            return (list, totalData);
        }

        private async Task<string> GetDosenNameById(int dosenId)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getDosenById", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            
            cmd.Parameters.AddWithValue(ParamDosenIdUpper, dosenId.ToString());

            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();
            return result?.ToString() ?? "";
        }

        private async Task<string> GetKonsentrasiById(short Prodi)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getKonsentrasiById", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

           
            cmd.Parameters.AddWithValue("@kon_id", Prodi.ToString());

            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();
            return result?.ToString() ?? "";
        }

        private async Task<List<MahasiswaItem>> GetMahasiswaListView(int DosenId, short Prodi, string Angkatan)
        {
            var list = new List<MahasiswaItem>();
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getDataDosenWaliDetail", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            
            cmd.Parameters.AddWithValue("@Mode", "View"); 
            cmd.Parameters.AddWithValue(ParamAngkatanUpper, Angkatan); 
            cmd.Parameters.AddWithValue("@ProdiId", Prodi); 
            cmd.Parameters.AddWithValue("@DosenId", DosenId); 

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new MahasiswaItem
                {
                    NIM = reader["mhs_id"].ToString()!,
                    Nama = reader["mhs_nama"].ToString()!,
                    Kelas = reader["kel_id"].ToString()!
                });
            }
            return list;
        }

        public async Task<WaliMahasiswa?> GetDetailAsync(int dosenId,string angkatan , int prodiId , string status)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_detailDosenWali", conn) 
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue(ParamDosenIdUpper, dosenId); 
            cmd.Parameters.AddWithValue(ParamAngkatanUpper, angkatan); 
            cmd.Parameters.AddWithValue(ParamProdiIdUpper, prodiId); 
            cmd.Parameters.AddWithValue("@Status", status); 


            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            var master = new WaliMahasiswa
            {
                Id = dosenId,
                DosenId = Convert.ToInt32(reader["dos_id"]),
                Prodi = Convert.ToInt16(reader["kon_id"]),
                Angkatan = reader["angkatan"].ToString()!,
                Status = reader["dwa_status"].ToString() ?? ""
            };
            
            
            master.NamaDosen = await GetDosenNameById(master.DosenId); 
            master.NamaProdi = await GetKonsentrasiById(master.Prodi); 
            master.DaftarMahasiswa = await GetMahasiswaListView(master.DosenId, master.Prodi, master.Angkatan); 

            return master;
        }


        public async Task<bool> SetStatusAsync(int dosenId, string angkatan, int prodiId, string status, string updatedBy)
        {
            var data = await GetDetailAsync(dosenId, angkatan, prodiId, status);
            if (data == null) return false;

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_setStatusDosenWali", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@DosenId", data.DosenId);
            cmd.Parameters.AddWithValue("@ProdiId", data.Prodi); 
            cmd.Parameters.AddWithValue("@Angkatan", data.Angkatan);
            cmd.Parameters.AddWithValue("@UpdatedBy", updatedBy);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync(); 
            
            return true;
        }

        public async Task<List<Dictionary<string, object>>> GetListDosenAsync()
        {
            var list = new List<Dictionary<string, object>>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getListDosenByKonsentrasi", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new Dictionary<string, object>
                {
                    ["id"] = reader[ColDosId] ?? 0,
                    ["nama"] = reader[ColDosNama]?.ToString() ?? ""
                });

            }

            return list;
        }
        
        public async Task<List<string>> GetListAngkatanAsync()
        {
            var list = new List<string>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getListAngkatanAktif", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var angkatan = reader["mhs_angkatan"]?.ToString();
                if (!string.IsNullOrEmpty(angkatan))
                    list.Add(angkatan);
            }

            return list;
        }

        public async Task<List<Dictionary<string, object>>> GetListProdiAsync()
        {
            var list = new List<Dictionary<string, object>>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getListKonsentrasi", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
              list.Add(new Dictionary<string, object>
                {
                    ["id"] = reader[ColKonId] ?? 0,
                    ["nama"] = reader[ColKonNama]?.ToString() ?? ""
                });
            }
            return list;
        } 

    }
}