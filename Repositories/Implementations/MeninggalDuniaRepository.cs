using astratech_apps_backend.DTOs.MeninggalDunia;
using CutiAkademikDTOs = astratech_apps_backend.DTOs.CutiAkademik;
using astratech_apps_backend.Models;
using astratech_apps_backend.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace astratech_apps_backend.Repositories.Implementations
{
    public class MeninggalDuniaRepository(IConfiguration config) : IMeninggalDuniaRepository
    {
        private readonly string _conn = PolmanAstraLibrary.PolmanAstraLibrary.Decrypt(
            config.GetConnectionString("DefaultConnection")!, 
            Environment.GetEnvironmentVariable("DECRYPT_KEY_CONNECTION_STRING")
        );

        public async Task<string> CreateAsync(CreateMeninggalDuniaRequest dto, string createdBy)
        {
            try
            {
                await using var conn = new SqlConnection(_conn);
                await conn.OpenAsync();
                
                // Save file lampiran
                var lampiranFileName = SaveFile(dto.LampiranFile);
                
                await using var cmd = new SqlCommand("sia_createMeninggalDunia", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@Step", "STEP1");
                cmd.Parameters.AddWithValue("@Lampiran", lampiranFileName ?? "");
                cmd.Parameters.AddWithValue("@MahasiswaId", dto.MhsId ?? "");
                cmd.Parameters.AddWithValue("@CreatedBy", createdBy ?? "");

                // Karena SP menggunakan SET NOCOUNT ON, gunakan ExecuteNonQuery
                await cmd.ExecuteNonQueryAsync();
                return "DRAFT_CREATED";
            }
            catch (Exception)
            {
                throw new InvalidOperationException("Terjadi kesalahan saat membuat pengajuan meninggal dunia.");
            }
        }

        public async Task<string> CreateWithMahasiswaDataAsync(string mhsId, string lampiranFileName, MahasiswaDetailDto mahasiswaData, string createdBy)
        {
            try
            {
                await using var conn = new SqlConnection(_conn);
                await conn.OpenAsync();
                
                await using var cmd = new SqlCommand("sia_createMeninggalDunia", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@Step", "STEP1");
                cmd.Parameters.AddWithValue("@Lampiran", lampiranFileName ?? "");
                cmd.Parameters.AddWithValue("@MahasiswaId", mhsId ?? "");
                cmd.Parameters.AddWithValue("@CreatedBy", createdBy ?? "");

                await cmd.ExecuteNonQueryAsync();
                return "DRAFT_CREATED";
            }
            catch (Exception)
            {
                return "";
            }
        }

        public async Task<MahasiswaDetailDto?> GetMahasiswaDetailAsync(string mhsId)
        {
            try
            {
                await using var conn = new SqlConnection(_conn);
                await conn.OpenAsync();
                
                await using var cmd = new SqlCommand("sia_getDetailMeninggalDunia", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@MeninggalDuniaId", mhsId);
                await using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new MahasiswaDetailDto
                    {
                        MhsId = reader["mhs_id"]?.ToString() ?? "",
                        MhsNama = reader["mhs_nama"]?.ToString() ?? "",
                        MhsAngkatan = reader["mhs_angkatan"]?.ToString() ?? "",
                        Konsentrasi = reader["kon_nama"]?.ToString() ?? "",
                        KonsentrasiId = reader["kon_singkatan"]?.ToString() ?? ""
                    };
                }
                
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<MahasiswaDetailDto?> GetMahasiswaDetailUsingSPAsync(string mhsId)
        {
            try
            {
                await using var conn = new SqlConnection(_conn);
                await conn.OpenAsync();
                
                await using var cmd = new SqlCommand("sia_detailMahasiswa", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@MahasiswaId", mhsId);
                await using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new MahasiswaDetailDto
                    {
                        MhsId = reader["mhs_id"]?.ToString() ?? "",
                        MhsNama = reader["mhs_nama"]?.ToString() ?? "",
                        MhsAngkatan = reader["mhs_angkatan"]?.ToString() ?? "",
                        ProgramStudi = reader["kon_nama"]?.ToString() ?? "", // SP mengembalikan "pro_nama + ' (' + kon_singkatan + ')'" sebagai kon_nama
                        Konsentrasi = reader["kon_nama"]?.ToString() ?? "",
                        KonsentrasiId = reader["kon_id"]?.ToString() ?? ""
                    };
                }
                
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<string> FinalizeAsync(string draftId, string updatedBy)
        {
            int maxRetries = 10; // Increase retry attempts
            
            for (int attempt = 0; attempt < maxRetries; attempt++)
            {
                try
                {
                    await using var conn = new SqlConnection(_conn);
                    await conn.OpenAsync();

                    await using var cmd = new SqlCommand("sia_createMeninggalDunia", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    
                    cmd.Parameters.AddWithValue("@Step", "STEP2");
                    cmd.Parameters.AddWithValue("@Lampiran", draftId);
                    cmd.Parameters.AddWithValue("@MahasiswaId", updatedBy);
                    cmd.Parameters.AddWithValue("@CreatedBy", "");

                    // Add random delay before each attempt to reduce collision
                    if (attempt > 0)
                    {
                        var preDelayMs = new Random().Next(100, 500);
                        await Task.Delay(preDelayMs);
                    }

                    await using var reader = await cmd.ExecuteReaderAsync();
                    if (await reader.ReadAsync())
                    {
                        var result = reader["idbaru"]?.ToString();
                        if (!string.IsNullOrEmpty(result))
                        {
                            return result;
                        }
                    }
                    
                    // If no result, wait and retry
                    await Task.Delay(500);
                }
                catch (SqlException ex) when (ex.Number == 2627) // Duplicate key error
                {
                    if (attempt == maxRetries - 1)
                    {
                        throw new InvalidOperationException($"Gagal finalize setelah {maxRetries} percobaan karena duplicate key.");
                    }
                    
                    // Exponential backoff with jitter
                    var delayMs = (int)Math.Pow(2, attempt) * 1000 + new Random().Next(500, 2000);
                    await Task.Delay(Math.Min(delayMs, 10000)); // Max 10 seconds
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Error in FinalizeAsync: {ex.Message}");
                }
            }
            
            throw new InvalidOperationException("Gagal finalize setelah semua percobaan.");
        }

        public async Task<IEnumerable<MahasiswaDropdownDto>> GetMahasiswaListAsync(string? search = null)
        {
            var result = new List<MahasiswaDropdownDto>();
            
            try
            {
                await using var conn = new SqlConnection(_conn);
                await conn.OpenAsync();
                
                await using var cmd = new SqlCommand("lpm_getListMahasiswa", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                // SP membutuhkan 50 parameter
                for (int i = 1; i <= 50; i++)
                {
                    cmd.Parameters.AddWithValue($"@p{i}", i == 1 ? (search ?? "") : "");
                }

                await using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    result.Add(new MahasiswaDropdownDto
                    {
                        MhsId = reader["Value"]?.ToString() ?? "",
                        MhsNama = reader["Text"]?.ToString() ?? "",
                        MhsAngkatan = "",
                        ProgramStudi = "",
                        Konsentrasi = ""
                    });
                }
            }
            catch (Exception)
            {
                // Return empty list on error
            }
            
            return result;
        }

        public async Task<IEnumerable<ProgramStudiDropdownDto>> GetProgramStudiListAsync()
        {
            var result = new List<ProgramStudiDropdownDto>();
            
            try
            {
                await using var conn = new SqlConnection(_conn);
                await conn.OpenAsync();
                
                await using var cmd = new SqlCommand("sia_getListKonsentrasi", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@Username", "");
                cmd.Parameters.AddWithValue("@RoleId", "");

                await using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    result.Add(new ProgramStudiDropdownDto
                    {
                        ProId = reader["kon_id"]?.ToString() ?? "",
                        ProNama = reader["kon_nama"]?.ToString() ?? ""
                    });
                }
            }
            catch (Exception)
            {
                // Return empty list on error
            }
            
            return result;
        }

        public async Task<bool> UpdateSKAsync(string id, string sk, string spkb, string updatedBy)
        {
            try
            {
                await using var conn = new SqlConnection(_conn);
                await conn.OpenAsync();

                await using var cmd = new SqlCommand("sia_createSKMeninggalDunia", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@MeninggalDuniaId", id);
                cmd.Parameters.AddWithValue("@SuratKeteranganMeninggalDunia", sk);
                cmd.Parameters.AddWithValue("@SuratKeteranganPernahBerkuliah", spkb);
                cmd.Parameters.AddWithValue("@ModifiedBy", updatedBy);

                await cmd.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<(IEnumerable<MeninggalDuniaListDto> Data, int TotalData)> GetAllAsync(GetAllMeninggalDuniaRequest req)
        {
            var result = new List<MeninggalDuniaListDto>();
            
            try
            {
                await using var conn = new SqlConnection(_conn);
                await conn.OpenAsync();
                
                await using var cmd = new SqlCommand("sia_getDataMeninggalDunia", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@Status", req.Status ?? "");
                cmd.Parameters.AddWithValue("@RoleId", req.RoleId ?? "");

                await using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    result.Add(new MeninggalDuniaListDto
                    {
                        Id = reader["mdu_id"]?.ToString() ?? "",
                        NoPengajuan = reader["mdu_id_alternative"]?.ToString() ?? "",
                        TanggalPengajuan = reader["mdu_created_date"]?.ToString() ?? "",
                        NamaMahasiswa = reader["mhs_nama"]?.ToString() ?? "",
                        Nim = reader["nim"]?.ToString() ?? "",
                        Prodi = reader["pro_nama"]?.ToString() ?? "",
                        NomorSK = reader["srt_no"]?.ToString() ?? "-",
                        Status = reader["mdu_status"]?.ToString() ?? ""
                    });
                }

                // Apply search filter if needed
                if (!string.IsNullOrEmpty(req.SearchKeyword))
                {
                    var keyword = req.SearchKeyword.ToLower();
                    result = result.Where(x => 
                        x.NoPengajuan.ToLower().Contains(keyword) ||
                        x.NamaMahasiswa.ToLower().Contains(keyword) ||
                        x.Nim.ToLower().Contains(keyword)
                    ).ToList();
                }

                var total = result.Count;

                // Apply pagination
                result = result.Skip((req.PageNumber - 1) * req.PageSize)
                              .Take(req.PageSize)
                              .ToList();

                return (result, total);
            }
            catch (Exception)
            {
                return (new List<MeninggalDuniaListDto>(), 0);
            }
        }

        public async Task<MeninggalDuniaReportResponse?> GetReportAsync(string id)
        {
            try
            {
                await using var conn = new SqlConnection(_conn);
                await conn.OpenAsync();
                
                await using var cmd = new SqlCommand("sia_reportMeninggalDunia", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@p1", id);

                await using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new MeninggalDuniaReportResponse
                    {
                        MhsId = reader["mhs_id"]?.ToString() ?? "",
                        MhsNama = reader["mhs_nama"]?.ToString() ?? "",
                        Konsentrasi = reader["kon_nama"]?.ToString() ?? "",
                        TahunAjaran = reader["srt_tahun_ajaran"]?.ToString() ?? "",
                        SuratNo = reader["srt_no"]?.ToString() ?? "",
                        Kaprodi = reader["pro_kaprodi"]?.ToString() ?? "",
                        Wadir = reader["wadir"]?.ToString() ?? "",
                        Direktur = reader["dir"]?.ToString() ?? ""
                    };
                }
                
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<MeninggalDuniaDetailResponse?> GetDetailAsync(string id)
        {
            try
            {
                await using var conn = new SqlConnection(_conn);
                await conn.OpenAsync();
                
                await using var cmd = new SqlCommand("sia_getDetailMeninggalDunia", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@MeninggalDuniaId", id);

                await using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new MeninggalDuniaDetailResponse
                    {
                        MhsId = reader["mhs_id"]?.ToString() ?? "",
                        MhsNama = reader["mhs_nama"]?.ToString() ?? "",
                        KonNama = reader["kon_nama"]?.ToString() ?? "",
                        MhsAngkatan = reader["mhs_angkatan"]?.ToString() ?? "",
                        KonSingkatan = reader["kon_singkatan"]?.ToString() ?? "",
                        Lampiran = reader["mdu_lampiran"]?.ToString() ?? "",
                        Status = reader["mdu_status"]?.ToString() ?? "",
                        CreatedBy = reader["mdu_created_by"]?.ToString() ?? "",
                        ApproveDir1Date = reader["mdu_approve_dir1_date"]?.ToString() ?? "",
                        ApproveDir1By = reader["mdu_approve_dir1_by"]?.ToString() ?? "",
                        SuratNo = reader["srt_no"]?.ToString() ?? "-",
                        NoSpkb = reader["mdu_no_spkb"]?.ToString() ?? "",
                        SK = reader["mdu_sk"]?.ToString() ?? "",
                        SPKB = reader["mdu_spkb"]?.ToString() ?? ""
                    };
                }
                
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<MeninggalDunia?> GetByIdAsync(string id)
        {
            try
            {
                await using var conn = new SqlConnection(_conn);
                await conn.OpenAsync();
                
                await using var cmd = new SqlCommand("sia_getDetailMeninggalDunia", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@MeninggalDuniaId", id);

                await using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new MeninggalDunia
                    {
                        Id = id,
                        MhsId = reader["mhs_id"]?.ToString() ?? "",
                        Lampiran = reader["mdu_lampiran"]?.ToString() ?? "",
                        ApproveDir1By = reader["mdu_approve_dir1_by"]?.ToString() ?? "",
                        SrtNo = reader["srt_no"]?.ToString() ?? "",
                        NoSpkb = reader["mdu_no_spkb"]?.ToString() ?? "",
                        Sk = reader["mdu_sk"]?.ToString() ?? "",
                        Spkb = reader["mdu_spkb"]?.ToString() ?? "",
                        Status = reader["mdu_status"]?.ToString() ?? "",
                        CreatedBy = reader["mdu_created_by"]?.ToString() ?? ""
                    };
                }
                
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> UpdateAsync(string id, UpdateMeninggalDuniaRequest dto, string updatedBy)
        {
            try
            {
                await using var conn = new SqlConnection(_conn);
                await conn.OpenAsync();

                await using var cmd = new SqlCommand("sia_editMeninggalDunia", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@MeninggalDuniaId", id);
                cmd.Parameters.AddWithValue("@Lampiran", dto.Lampiran ?? "");
                cmd.Parameters.AddWithValue("@ModifiedBy", updatedBy);

                await cmd.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> SoftDeleteAsync(string id, string updatedBy)
        {
            try
            {
                await using var conn = new SqlConnection(_conn);
                await conn.OpenAsync();

                await using var cmd = new SqlCommand("sia_deleteMeninggalDunia", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@MeninggalDuniaId", id);
                cmd.Parameters.AddWithValue("@ModifiedBy", updatedBy);

                await cmd.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UploadSKAsync(string id, string sk, string spkb, string updatedBy)
        {
            return await UpdateSKAsync(id, sk, spkb, updatedBy);
        }

        public async Task<(IEnumerable<RiwayatMeninggalDuniaListDto> Data, int TotalData)> GetRiwayatAsync(GetRiwayatMeninggalDuniaRequest req)
        {
            var result = new List<RiwayatMeninggalDuniaListDto>();
            
            try
            {
                await using var conn = new SqlConnection(_conn);
                await conn.OpenAsync();
                
                await using var cmd = new SqlCommand("sia_getDataRiwayatMeninggalDunia", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@Keyword", req.Keyword ?? "");
                cmd.Parameters.AddWithValue("@Sort", req.Sort ?? "mdu_created_date desc");
                cmd.Parameters.AddWithValue("@Konsentrasi", req.Konsentrasi ?? "");
                cmd.Parameters.AddWithValue("@RoleId", req.RoleId ?? "");

                await using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    result.Add(new RiwayatMeninggalDuniaListDto
                    {
                        Id = reader["mdu_id"]?.ToString() ?? "",
                        NoPengajuan = reader["mdu_id"]?.ToString() ?? "",
                        TanggalPengajuan = reader["tanggal_buat"]?.ToString() ?? "",
                        NamaMahasiswa = reader["mhs_nama"]?.ToString() ?? "",
                        Nim = reader["mhs_id"]?.ToString() ?? "",
                        Prodi = reader["pro_nama"]?.ToString() ?? "",
                        NomorSK = reader["srt_no"]?.ToString() ?? "",
                        Status = reader["mdu_status"]?.ToString() ?? ""
                    });
                }

                // Apply search filter if keyword is provided
                if (!string.IsNullOrEmpty(req.Keyword))
                {
                    var keyword = req.Keyword.ToLower();
                    result = result.Where(x => 
                        x.NoPengajuan.ToLower().Contains(keyword) ||
                        x.NamaMahasiswa.ToLower().Contains(keyword) ||
                        x.Nim.ToLower().Contains(keyword) ||
                        x.Prodi.ToLower().Contains(keyword)
                    ).ToList();
                }

                // Return all data without pagination
                return (result, result.Count);
            }
            catch (Exception)
            {
                return (new List<RiwayatMeninggalDuniaListDto>(), 0);
            }
        }

        public async Task<IEnumerable<RiwayatMeninggalDuniaExcelResponse>> GetRiwayatExcelAsync(string sort, string konsentrasi)
        {
            var result = new List<RiwayatMeninggalDuniaExcelResponse>();
            
            try
            {
                await using var conn = new SqlConnection(_conn);
                await conn.OpenAsync();
                
                await using var cmd = new SqlCommand("sia_getDataRiwayatMeninggalDuniaExcel", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@Sort", sort ?? "");
                cmd.Parameters.AddWithValue("@Konsentrasi", konsentrasi ?? "");

                await using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    result.Add(new RiwayatMeninggalDuniaExcelResponse
                    {
                        NIM = reader["NIM"]?.ToString() ?? "",
                        NamaMahasiswa = reader["Nama Mahasiswa"]?.ToString() ?? "",
                        Konsentrasi = reader["Konsentrasi"]?.ToString() ?? "",
                        TanggalPengajuan = reader["Tanggal Pengajuan"]?.ToString() ?? "",
                        NoSK = reader["No SK"]?.ToString() ?? "",
                        NoPengajuan = reader["No Pengajuan"]?.ToString() ?? ""
                    });
                }
            }
            catch (Exception)
            {
                // Return empty result on error
            }
            
            return result;
        }

        public async Task<bool> ApproveAsync(string id, ApproveMeninggalDuniaRequest dto)
        {
            try
            {
                await using var conn = new SqlConnection(_conn);
                await conn.OpenAsync();

                await using var cmd = new SqlCommand("sia_setujuiMeninggalDunia", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@MeninggalDuniaId", id);
                cmd.Parameters.AddWithValue("@Role", dto.Role ?? "");
                cmd.Parameters.AddWithValue("@Username", dto.Username ?? "");

                await cmd.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> RejectAsync(string id, RejectMeninggalDuniaRequest dto)
        {
            try
            {
                await using var conn = new SqlConnection(_conn);
                await conn.OpenAsync();

                await using var cmd = new SqlCommand("sia_tolakMeninggalDunia", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@MeninggalDuniaId", id);
                cmd.Parameters.AddWithValue("@Role", dto.Role ?? "");
                cmd.Parameters.AddWithValue("@Username", dto.Username ?? "");

                await cmd.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<string> DetectUserRoleAsync(string username)
        {
            try
            {
                await using var conn = new SqlConnection(_conn);
                await conn.OpenAsync();
                
                await using var cmd = new SqlCommand("all_getIdentityByUser", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@UsernameToFind", username);
                await using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    var rolId = reader["rol_id"]?.ToString() ?? "";
                    var jabMainId = reader["jab_main_id"]?.ToString() ?? "";
                    
                    // Mapping rol_id ke role name
                    return rolId switch
                    {
                        "ROL999" => "wadir1",
                        "ROL71" => "prodi", 
                        _ when username.Contains("finance", StringComparison.OrdinalIgnoreCase) => "finance",
                        _ when jabMainId == "4" => "wadir1",  // fallback jika rol_id kosong
                        _ when jabMainId == "6" => "prodi",   // fallback jika rol_id kosong
                        _ => ""
                    };
                }
                
                return "";
            }
            catch (Exception)
            {
                return "";
            }
        }

        public async Task<IEnumerable<MahasiswaDropdownSPDto>> GetMahasiswaDropdownSPAsync()
        {
            var result = new List<MahasiswaDropdownSPDto>();
            
            try
            {
                await using var conn = new SqlConnection(_conn);
                await conn.OpenAsync();
                
                await using var cmd = new SqlCommand("lpm_getListMahasiswa", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                for (int i = 1; i <= 50; i++)
                {
                    cmd.Parameters.AddWithValue($"@p{i}", "");
                }

                await using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    result.Add(new MahasiswaDropdownSPDto
                    {
                        Value = reader["Value"]?.ToString() ?? "",
                        Text = reader["Text"]?.ToString() ?? "",
                        NimNama = reader["NimNama"]?.ToString() ?? ""
                    });
                }
            }
            catch (Exception)
            {
                // Return empty list on error
            }
            
            return result;
        }

        public async Task<MahasiswaProdiDto?> GetMahasiswaProdiSPAsync(string mhsId)
        {
            try
            {
                await using var conn = new SqlConnection(_conn);
                await conn.OpenAsync();
                
                await using var cmd = new SqlCommand("lpm_getListMahasiswaByProdi", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@p1", mhsId);

                for (int i = 2; i <= 50; i++)
                {
                    cmd.Parameters.AddWithValue($"@p{i}", "");
                }

                await using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new MahasiswaProdiDto
                    {
                        KonId = reader["kon_id"]?.ToString() ?? "",
                        ProId = reader["pro_id"]?.ToString() ?? "",
                        ProNama = reader["pro_nama"]?.ToString() ?? ""
                    };
                }
                
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<IEnumerable<KonsentrasiDropdownDto>> GetKonsentrasiBySekprodAsync(string username)
        {
            var result = new List<KonsentrasiDropdownDto>();
            
            try
            {
                await using var conn = new SqlConnection(_conn);
                await conn.OpenAsync();
                
                await using var cmd = new SqlCommand("sia_getListKonsentrasiBySekprod", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                // SP membutuhkan 50 parameter, kita isi parameter pertama dengan username
                cmd.Parameters.AddWithValue("@p1", username);
                for (int i = 2; i <= 50; i++)
                {
                    cmd.Parameters.AddWithValue($"@p{i}", "");
                }

                await using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    result.Add(new KonsentrasiDropdownDto
                    {
                        Id = SafeConvertToInt(reader["id"]),
                        Nama = reader["nama"]?.ToString() ?? ""
                    });
                }
            }
            catch (Exception)
            {
                // Return empty list on error
            }
            
            return result;
        }

        private static int SafeConvertToInt(object value)
        {
            if (value == null || value == DBNull.Value)
                return 0;
                
            var stringValue = value.ToString();
            if (string.IsNullOrEmpty(stringValue))
                return 0;
                
            if (int.TryParse(stringValue, out int result))
                return result;
                
            return 0; // Return 0 if conversion fails
        }

        private string? SaveFile(IFormFile? file)
        {
            if (file == null || file.Length == 0)
                return null;

            var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/meninggaldunia");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return fileName;
        }

        public async Task<IEnumerable<CutiAkademikDTOs.MahasiswaByKonsentrasiDto>> GetMahasiswaByKonsentrasiAsync(string username)
        {
            var result = new List<CutiAkademikDTOs.MahasiswaByKonsentrasiDto>();
            
            try
            {
                await using var conn = new SqlConnection(_conn);
                await conn.OpenAsync();
                
                // Gunakan stored procedure yang sudah ada
                await using var cmd = new SqlCommand("sia_getListMahasiswaByKonsentrasi", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@Id", username);
                await using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    result.Add(new CutiAkademikDTOs.MahasiswaByKonsentrasiDto
                    {
                        MhsId = reader["mhs_id"]?.ToString() ?? "",
                        MhsNama = reader["mhs_nama"]?.ToString() ?? ""
                    });
                }
            }
            catch (Exception)
            {
                // Return empty list on error
            }
            
            return result;
        }
    }
}