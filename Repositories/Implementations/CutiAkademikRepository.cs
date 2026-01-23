using astratech_apps_backend.DTOs.CutiAkademik;
using astratech_apps_backend.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace astratech_apps_backend.Repositories.Implementations
{
    public class CutiAkademikRepository : ICutiAkademikRepository
    {
        private readonly string _conn;

        public CutiAkademikRepository(IConfiguration config)
        {
            _conn = PolmanAstraLibrary.PolmanAstraLibrary.Decrypt(
                config.GetConnectionString("DefaultConnection")!,
                Environment.GetEnvironmentVariable("DECRYPT_KEY_CONNECTION_STRING")
            );
        }

        public async Task<string?> CreateDraftAsync(CreateDraftCutiAkademikRequest dto)
        {
            using var conn = new SqlConnection(_conn);
            await conn.OpenAsync();

            var fileSP = SaveFile(dto.LampiranSuratPengajuan);
            var fileLampiran = SaveFile(dto.Lampiran);

            await using var cmd = new SqlCommand("sia_createCutiAkademik", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Step", "STEP1");
            cmd.Parameters.AddWithValue("@TahunAjaran", dto.TahunAjaran ?? "");
            cmd.Parameters.AddWithValue("@Semester", dto.Semester ?? "");
            cmd.Parameters.AddWithValue("@LampiranSuratPengajuan", fileSP ?? "");
            cmd.Parameters.AddWithValue("@Lampiran", fileLampiran ?? "");
            cmd.Parameters.AddWithValue("@MahasiswaId", dto.MhsId);
            cmd.Parameters.AddWithValue("@DraftId", "");
            cmd.Parameters.AddWithValue("@ModifiedBy", "");

            var result = await cmd.ExecuteScalarAsync();
            return result?.ToString();
        }




        public async Task<string?> GenerateIdAsync(GenerateIdFinalCutiAkademikRequest dto)
        {
            await using var conn = new SqlConnection(_conn);
            await conn.OpenAsync();
            
            await using var cmd = new SqlCommand("sia_createCutiAkademik", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Step", "STEP2");
            cmd.Parameters.AddWithValue("@TahunAjaran", "");
            cmd.Parameters.AddWithValue("@Semester", "");
            cmd.Parameters.AddWithValue("@LampiranSuratPengajuan", "");
            cmd.Parameters.AddWithValue("@Lampiran", "");
            cmd.Parameters.AddWithValue("@MahasiswaId", "");
            cmd.Parameters.AddWithValue("@DraftId", dto.DraftId);
            cmd.Parameters.AddWithValue("@ModifiedBy", dto.ModifiedBy);

            var result = await cmd.ExecuteScalarAsync();
            return result?.ToString();
        }

        private string ConvertToRoman(int month)
        {
            return month switch
            {
                1 => "I",
                2 => "II", 
                3 => "III",
                4 => "IV",
                5 => "V",
                6 => "VI",
                7 => "VII",
                8 => "VIII",
                9 => "IX",
                10 => "X",
                11 => "XI",
                12 => "XII",
                _ => "I"
            };
        }

        public async Task<IEnumerable<CutiAkademikListResponse>> GetAllAsync(
            string mhsId, string status, string userId, string role, string search = "")
        {
            var result = new List<CutiAkademikListResponse>();

            await using var conn = new SqlConnection(_conn);
            await conn.OpenAsync();
            
            await using var cmd = new SqlCommand("sia_getDataCutiAkademik", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@MahasiswaId", mhsId ?? "");
            cmd.Parameters.AddWithValue("@Status", status ?? "");
            cmd.Parameters.AddWithValue("@UserId", userId ?? "");
            cmd.Parameters.AddWithValue("@Search", search ?? "");

            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                result.Add(new CutiAkademikListResponse
                {
                    Id = reader["cak_id"]?.ToString() ?? "",
                    IdDisplay = reader["id"]?.ToString() ?? "",
                    MhsId = reader["mhs_id"]?.ToString() ?? "",
                    NamaMahasiswa = reader["mhs_nama"]?.ToString() ?? "",
                    Prodi = reader["kon_nama"]?.ToString() ?? "",
                    TahunAjaran = reader["cak_tahunajaran"]?.ToString() ?? "",
                    Semester = reader["cak_semester"]?.ToString() ?? "",
                    ApproveProdi = reader["approve_prodi"]?.ToString() ?? "",
                    ApproveDir1 = reader["approve_dir1"]?.ToString() ?? "",
                    Tanggal = reader["tanggal"]?.ToString() ?? "",
                    SuratNo = reader["srt_no"]?.ToString() ?? "",
                    Status = reader["status"]?.ToString() ?? "",
                });
            }

            return result;
        }

        public async Task<CutiAkademikDetailResponse?> GetDetailAsync(string id)
        {
            await using var conn = new SqlConnection(_conn);
            await conn.OpenAsync();

            await using var cmd = new SqlCommand("sia_detailCutiAkademik", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@CutiAkademikId", id);

            await using var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
                return null;

            return new CutiAkademikDetailResponse
            {
                Id = reader["cak_id"].ToString(),
                MhsId = reader["mhs_id"].ToString(),
                Mahasiswa = reader["mhs_nama"]?.ToString() ?? "",
                Konsentrasi = reader["kon_nama"]?.ToString() ?? "",
                Angkatan = reader["mhs_angkatan"]?.ToString() ?? "",
                KonsentrasiSingkatan = reader["kon_singkatan"]?.ToString() ?? "",
                TahunAjaran = reader["cak_tahunajaran"]?.ToString() ?? "",
                Semester = reader["cak_semester"]?.ToString() ?? "",
                LampiranSP = reader["cak_lampiran_suratpengajuan"]?.ToString() ?? "",
                Lampiran = reader["cak_lampiran"]?.ToString() ?? "",
                Status = reader["cak_status"]?.ToString() ?? "",
                CreatedBy = reader["cak_created_by"]?.ToString() ?? "",
                TglPengajuan = reader["tgl"]?.ToString() ?? "",
                Sk = reader["cak_sk"]?.ToString() ?? "",
                SrtNo = reader["srt_no"]?.ToString() ?? "",
                ProdiNama = reader["pro_nama"]?.ToString() ?? "",
                Kaprodi = reader["kaprod"]?.ToString() ?? "",
                AppProdiDate = reader["cak_app_prodi_date"]?.ToString() ?? "",
                ApprovalProdi = reader["cak_approval_prodi"]?.ToString() ?? "",
                AppDir1Date = reader["cak_app_dir1_date"]?.ToString() ?? "",
                ApprovalDir1 = reader["cak_approval_dir1"]?.ToString() ?? "",
                Alamat = reader["mhs_alamat"]?.ToString() ?? "",
                Menimbang = reader["cak_menimbang"]?.ToString() ?? "",
                BulanCuti = reader["BulanCuti"]?.ToString() ?? "",
                Direktur = reader["direktur"]?.ToString() ?? "",
                Wadir1 = reader["wadir1"]?.ToString() ?? "",
                Wadir2 = reader["wadir2"]?.ToString() ?? "",
                Wadir3 = reader["wadir3"]?.ToString() ?? "",
                KodePos = reader["mhs_kodepos"]?.ToString() ?? "",
            };
        }


        public async Task<bool> UpdateAsync(string id, UpdateCutiAkademikRequest dto)
        {
            await using var conn = new SqlConnection(_conn);
            await conn.OpenAsync();

            var fileSP = SaveFile(dto.LampiranSuratPengajuan);
            var fileLampiran = SaveFile(dto.Lampiran);

            var cmd = new SqlCommand("sia_editCutiAkademik", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@CutiAkademikId", id);
            cmd.Parameters.AddWithValue("@TahunAjaran", dto.TahunAjaran ?? "");
            cmd.Parameters.AddWithValue("@Semester", dto.Semester ?? "");
            cmd.Parameters.AddWithValue("@LampiranSuratPengajuan", fileSP ?? "");
            cmd.Parameters.AddWithValue("@Lampiran", fileLampiran ?? "");
            cmd.Parameters.AddWithValue("@ModifiedBy", dto.ModifiedBy ?? "");

            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result ?? 0) > 0;
        }

        public async Task<bool> DeleteAsync(string id, string modifiedBy)
        {
            try
            {
                await using var conn = new SqlConnection(_conn);
                await using var cmd = new SqlCommand("sia_deleteCutiAkademik", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@CutiAkademikId", id);
                cmd.Parameters.AddWithValue("@ModifiedBy", modifiedBy);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();

                return true;   
            }
            catch
            {
                return false;  
            }
        }


        public async Task<string?> CreateDraftByProdiAsync(CreateDraftCutiAkademikByProdiRequest dto)
        {
            await using var conn = new SqlConnection(_conn);
            await conn.OpenAsync();

            try
            {
                
                var fileSP = SaveFile(dto.LampiranSuratPengajuan);
                var fileLampiran = SaveFile(dto.Lampiran);

              
                var cmd = new SqlCommand("sia_createCutiAkademikByProdi", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@Step", "STEP1");
                cmd.Parameters.AddWithValue("@TahunAjaran", dto.TahunAjaran ?? "");
                cmd.Parameters.AddWithValue("@Semester", dto.Semester ?? "");
                cmd.Parameters.AddWithValue("@LampiranSuratPengajuan", fileSP ?? "");
                cmd.Parameters.AddWithValue("@Lampiran", fileLampiran ?? "");
                cmd.Parameters.AddWithValue("@MahasiswaId", dto.MhsId ?? "");
                cmd.Parameters.AddWithValue("@Menimbang", dto.Menimbang ?? "");
                cmd.Parameters.AddWithValue("@ApprovalProdi", dto.ApprovalProdi ?? "");
                cmd.Parameters.AddWithValue("@DraftId", ""); 
                cmd.Parameters.AddWithValue("@ModifiedBy", ""); 

                var result = await cmd.ExecuteScalarAsync();
                return result?.ToString();
            }
            catch (SqlException ex) when (ex.Number == 2627) 
            {
                
                return await CreateDraftByProdiDirectAsync(dto, conn);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<string?> CreateDraftByProdiDirectAsync(CreateDraftCutiAkademikByProdiRequest dto, SqlConnection conn)
        {
            var fileSP = SaveFile(dto.LampiranSuratPengajuan);
            var fileLampiran = SaveFile(dto.Lampiran);

            var cmd = new SqlCommand("sia_createCutiAkademikByProdi", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Step", "STEP1");
            cmd.Parameters.AddWithValue("@TahunAjaran", dto.TahunAjaran ?? "");
            cmd.Parameters.AddWithValue("@Semester", dto.Semester ?? "");
            cmd.Parameters.AddWithValue("@LampiranSuratPengajuan", fileSP ?? "");
            cmd.Parameters.AddWithValue("@Lampiran", fileLampiran ?? "");
            cmd.Parameters.AddWithValue("@MahasiswaId", dto.MhsId ?? "");
            cmd.Parameters.AddWithValue("@Menimbang", dto.Menimbang ?? "");
            cmd.Parameters.AddWithValue("@ApprovalProdi", dto.ApprovalProdi ?? "");
            cmd.Parameters.AddWithValue("@DraftId", ""); 
            cmd.Parameters.AddWithValue("@ModifiedBy", dto.ApprovalProdi ?? "");

            var result = await cmd.ExecuteScalarAsync();
            return result?.ToString();
        }

        public async Task<string?> GenerateIdByProdiAsync(GenerateIdFinalCutiAkademikByProdiRequest dto)
        {
            await using var conn = new SqlConnection(_conn);
            await conn.OpenAsync();

            try
            {
                
                var cmd = new SqlCommand("sia_createCutiAkademikByProdi", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@Step", "STEP2");
                cmd.Parameters.AddWithValue("@TahunAjaran", ""); 
                cmd.Parameters.AddWithValue("@Semester", ""); 
                cmd.Parameters.AddWithValue("@LampiranSuratPengajuan", ""); 
                cmd.Parameters.AddWithValue("@Lampiran", ""); 
                cmd.Parameters.AddWithValue("@MahasiswaId", ""); 
                cmd.Parameters.AddWithValue("@Menimbang", ""); 
                cmd.Parameters.AddWithValue("@ApprovalProdi", ""); 
                cmd.Parameters.AddWithValue("@DraftId", dto.DraftId ?? "");
                cmd.Parameters.AddWithValue("@ModifiedBy", dto.ModifiedBy ?? "");

                var result = await cmd.ExecuteScalarAsync();
                return result?.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<CutiAkademikListResponse>> GetRiwayatAsync(
    string userId, string status, string search)
        {
            var result = new List<CutiAkademikListResponse>();

            await using var conn = new SqlConnection(_conn);
            await conn.OpenAsync();
            
            await using var cmd = new SqlCommand("sia_getDataRiwayatCutiAkademik", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@UserId", userId ?? "");
            cmd.Parameters.AddWithValue("@Status", status ?? "");
            cmd.Parameters.AddWithValue("@Search", search ?? "");

            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                result.Add(new CutiAkademikListResponse
                {
                    Id = reader["cak_id"]?.ToString() ?? "",
                    IdDisplay = reader["id"]?.ToString() ?? "",
                    MhsId = reader["mhs_id"]?.ToString() ?? "",
                    NamaMahasiswa = reader["mhs_nama"]?.ToString() ?? "",
                    Prodi = reader["kon_nama"]?.ToString() ?? "",
                    TahunAjaran = reader["cak_tahunajaran"]?.ToString() ?? "",
                    Semester = reader["cak_semester"]?.ToString() ?? "",
                    ApproveProdi = reader["approve_prodi"]?.ToString() ?? "",
                    ApproveDir1 = reader["approve_dir1"]?.ToString() ?? "",
                    Tanggal = reader["tanggal"]?.ToString() ?? "",
                    SuratNo = reader["srt_no"]?.ToString() ?? "",
                    Status = reader["status"]?.ToString() ?? ""
                });
            }

            return result;
        }

        public async Task<IEnumerable<CutiAkademikRiwayatExcelResponse>> GetRiwayatExcelAsync(string userId)
        {
            var result = new List<CutiAkademikRiwayatExcelResponse>();

            await using var conn = new SqlConnection(_conn);
            await conn.OpenAsync();
            
            await using var cmd = new SqlCommand("sia_getDataRiwayatCutiAkademikExcel", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@UserId", userId ?? "");

            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                result.Add(new CutiAkademikRiwayatExcelResponse
                {
                    NIM = reader["NIM"]?.ToString() ?? "",
                    NamaMahasiswa = reader["Nama Mahasiswa"]?.ToString() ?? "",
                    Konsentrasi = reader["Konsentrasi"]?.ToString() ?? "",
                    TanggalPengajuan = reader["Tanggal Pengajuan"]?.ToString() ?? "",
                    NoSK = reader["No SK"]?.ToString() ?? "",
                    NoPengajuan = reader["No Pengajuan"]?.ToString() ?? ""
                });
            }

            return result;
        }

        private string? SaveFile(IFormFile? file)
        {
            if (file == null || file.Length == 0)
                return null;

            var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/cuti");

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

       
        public async Task<bool> ApproveCutiAsync(ApproveCutiAkademikRequest dto)
        {
            await using var conn = new SqlConnection(_conn);
            await conn.OpenAsync();

            var cmd = new SqlCommand("sia_setujuiCutiAkademik", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@CutiAkademikId", dto.Id);
            cmd.Parameters.AddWithValue("@Role", dto.Role.ToLower());
            cmd.Parameters.AddWithValue("@ApprovedBy", dto.ApprovedBy);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var success = Convert.ToBoolean(reader["Success"]);
                return success;
            }

            return false;
        }

        public async Task<bool> ApproveProdiCutiAsync(ApproveCutiAkademikByProdiRequest dto)
        {
            await using var conn = new SqlConnection(_conn);
            await conn.OpenAsync();

            var cmd = new SqlCommand("sia_setujuiCutiAkademikProdi", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@CutiAkademikId", dto.Id);
            cmd.Parameters.AddWithValue("@Menimbang", dto.Menimbang ?? "");
            cmd.Parameters.AddWithValue("@ApprovedBy", dto.ApprovedBy);

            var rowsAffected = await cmd.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

       
        public async Task<bool> RejectCutiAsync(RejectCutiAkademikRequest dto)
        {
            try
            {

                await using var conn = new SqlConnection(_conn);
                await conn.OpenAsync();

                var checkCmd = new SqlCommand(
                    "SELECT cak_id, cak_status FROM sia_mscutiakademik WHERE cak_id = @id", conn);
                checkCmd.Parameters.AddWithValue("@id", dto.Id);

                var reader = await checkCmd.ExecuteReaderAsync();
                if (!await reader.ReadAsync())
                {
                    await reader.CloseAsync();
                    return false;
                }

                var currentStatus = reader["cak_status"].ToString();
                await reader.CloseAsync();
                

                var spCmd = new SqlCommand("sia_tolakCutiAkademik", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                spCmd.Parameters.AddWithValue("@CutiAkademikId", dto.Id);
                spCmd.Parameters.AddWithValue("@Role", dto.Role);
                spCmd.Parameters.AddWithValue("@Keterangan", ""); 

                var spRows = await spCmd.ExecuteNonQueryAsync();

                var newStatusCmd = new SqlCommand("SELECT cak_status FROM sia_mscutiakademik WHERE cak_id = @id", conn);
                newStatusCmd.Parameters.AddWithValue("@id", dto.Id);
                var newStatus = (await newStatusCmd.ExecuteScalarAsync())?.ToString();
                
                bool statusChanged = !string.Equals(currentStatus, newStatus, StringComparison.OrdinalIgnoreCase);
                
                if (statusChanged)
                {
                    return true;
                }
                else if (spRows > 0)
                {
                    return true;
                }

                
                var directCmd = new SqlCommand(@"
                    UPDATE sia_mscutiakademik 
                    SET cak_keterangan = '',
                        cak_status = @newStatus
                    WHERE cak_id = @id", conn);

                directCmd.Parameters.AddWithValue("@id", dto.Id);
                directCmd.Parameters.AddWithValue("@newStatus", $"Ditolak {dto.Role}");

                var directRows = await directCmd.ExecuteNonQueryAsync();

                var success = directRows > 0;
                
                return success;
            }
            catch (Exception)
            {
                throw; 
            }
        }

       
        public async Task<string?> CreateSKAsync(CreateSKRequest dto)
        {
            try
            {

                await using var conn = new SqlConnection(_conn);
                await conn.OpenAsync();

                var checkCmd = new SqlCommand(@"
                    SELECT cak_id, cak_status, srt_no 
                    FROM sia_mscutiakademik 
                    WHERE cak_id = @id", conn);
                checkCmd.Parameters.AddWithValue("@id", dto.Id);

                var reader = await checkCmd.ExecuteReaderAsync();
                if (!await reader.ReadAsync())
                {
                    await reader.CloseAsync();
                    return null;
                }

                var currentStatus = reader["cak_status"]?.ToString() ?? "";
                var existingSrtNo = reader["srt_no"]?.ToString() ?? "";
                await reader.CloseAsync();
                

                if (currentStatus != "Belum Disetujui Finance" && currentStatus != "Menunggu Upload SK")
                {
                    return null;
                }

                string noSK = dto.NoSK ?? existingSrtNo;
                
                if (string.IsNullOrEmpty(noSK))
                {
                    
                    var year = DateTime.Now.Year;
                    var month = DateTime.Now.Month;
                    
                    
                    var getLastNoCmd = new SqlCommand(@"
                        SELECT TOP 1 srt_no 
                        FROM sia_mscutiakademik 
                        WHERE srt_no LIKE @pattern 
                        ORDER BY srt_no DESC", conn);
                    getLastNoCmd.Parameters.AddWithValue("@pattern", $"%/SK-CA/{month:D2}/{year}");
                    
                    var lastNo = await getLastNoCmd.ExecuteScalarAsync();
                    int sequence = 1;
                    
                    if (lastNo != null)
                    {
                        var lastNoStr = lastNo.ToString();
                        var parts = lastNoStr?.Split('/');
                        if (parts != null && parts.Length > 0 && int.TryParse(parts[0], out int lastSeq))
                        {
                            sequence = lastSeq + 1;
                        }
                    }
                    
                    noSK = $"{sequence:D3}/SK-CA/{month:D2}/{year}";
                }

                
                try
                {
                    var spCmd = new SqlCommand("sia_createSKCutiAkademik", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    spCmd.Parameters.AddWithValue("@CutiAkademikId", dto.Id);
                    spCmd.Parameters.AddWithValue("@NomorSK", noSK);
                    spCmd.Parameters.AddWithValue("@ModifiedBy", dto.CreatedBy);

                    var spRows = await spCmd.ExecuteNonQueryAsync();

                    if (spRows > 0)
                    {
                        return noSK;
                    }
                }
                catch (Exception)
                {
                    
                }

               
                var updateCmd = new SqlCommand(@"
                    UPDATE sia_mscutiakademik 
                    SET srt_no = @noSK,
                        cak_status = 'Menunggu Upload SK',
                        cak_modif_date = GETDATE(),
                        cak_modif_by = @createdBy
                    WHERE cak_id = @id", conn);

                updateCmd.Parameters.AddWithValue("@id", dto.Id);
                updateCmd.Parameters.AddWithValue("@noSK", noSK);
                updateCmd.Parameters.AddWithValue("@createdBy", dto.CreatedBy);

                var rows = await updateCmd.ExecuteNonQueryAsync();

                if (rows > 0)
                {
                    return noSK;
                }
                
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<bool> UploadSKAsync(UploadSKCutiAkademikRequest dto)
        {
            try
            {

                await using var conn = new SqlConnection(_conn);
                await conn.OpenAsync();

                var checkCmd = new SqlCommand(
                    "SELECT cak_id, cak_status, srt_no FROM sia_mscutiakademik WHERE cak_id = @id", conn);
                checkCmd.Parameters.AddWithValue("@id", dto.Id);

                var reader = await checkCmd.ExecuteReaderAsync();
                if (!await reader.ReadAsync())
                {
                    await reader.CloseAsync();
                    return false;
                }

                var currentStatus = reader["cak_status"].ToString();
                var existingSrtNo = reader["srt_no"]?.ToString() ?? "";
                await reader.CloseAsync();
                

                if (currentStatus != "Menunggu Upload SK" && currentStatus != "Disetujui")
                {
                    return false;
                }

                var fileName = SaveFile(dto.FileSK);
                if (string.IsNullOrEmpty(fileName))
                {
                    return false;
                }


                var skNumber = await GenerateSKNumberAsync(conn);

                var updateCmd = new SqlCommand(@"
                    UPDATE sia_mscutiakademik 
                    SET cak_sk = @fileName,
                        cak_status = 'Disetujui',
                        cak_status_cuti = 'Cuti',
                        cak_approval_dakap = GETDATE(),
                        cak_modif_date = GETDATE(),
                        cak_modif_by = @uploadBy
                    WHERE cak_id = @id;
                    
                    -- Also update mahasiswa status
                    UPDATE sia_msmahasiswa 
                    SET mhs_status_kuliah = 'Cuti' 
                    WHERE mhs_id = (SELECT mhs_id FROM sia_mscutiakademik WHERE cak_id = @id);", conn);

                updateCmd.Parameters.AddWithValue("@id", dto.Id);
                updateCmd.Parameters.AddWithValue("@fileName", fileName);
                updateCmd.Parameters.AddWithValue("@uploadBy", dto.UploadBy);

                var rowsAffected = await updateCmd.ExecuteNonQueryAsync();

                if (rowsAffected > 0)
                {
                    
                    var verifyCmd = new SqlCommand(
                        "SELECT cak_sk, cak_status FROM sia_mscutiakademik WHERE cak_id = @id", conn);
                    verifyCmd.Parameters.AddWithValue("@id", dto.Id);
                    
                    var verifyReader = await verifyCmd.ExecuteReaderAsync();
                    if (await verifyReader.ReadAsync())
                    {
                        var finalSk = verifyReader["cak_sk"]?.ToString() ?? "";
                        var finalStatus = verifyReader["cak_status"]?.ToString() ?? "";
                    }
                    await verifyReader.CloseAsync();
                    
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                throw; 
            }
        }

        private async Task<string> GenerateSKNumberAsync(SqlConnection conn)
        {
            try
            {
                var dateInfo = GetCurrentDateInfo();
                var maxSequence = await GetMaxSequenceForYearAsync(conn, dateInfo.year);
                return await GenerateUniqueSkNumberAsync(conn, dateInfo.romanMonth, dateInfo.year, maxSequence + 1);
            }
            catch (Exception)
            {
                var now = DateTime.Now;
                var romanMonth = ConvertToRoman(now.Month);
                return $"999/PA-WADIR-I/SKC/{romanMonth}/{now.Year}";
            }
        }

        private (string romanMonth, int year) GetCurrentDateInfo()
        {
            var now = DateTime.Now;
            return (ConvertToRoman(now.Month), now.Year);
        }

        private async Task<int> GetMaxSequenceForYearAsync(SqlConnection conn, int year)
        {
            var getLastSkCmd = new SqlCommand(@"
                SELECT srt_no 
                FROM sia_mscutiakademik 
                WHERE srt_no LIKE '%/PA-WADIR-I/SKC/%/' + CAST(@year AS VARCHAR(4))
                  AND srt_no IS NOT NULL 
                  AND srt_no != ''
                  AND LEN(srt_no) > 10
                ORDER BY srt_no DESC", conn);
            
            getLastSkCmd.Parameters.AddWithValue("@year", year);
            
            var reader = await getLastSkCmd.ExecuteReaderAsync();
            int maxSequence = 0;
            
            while (await reader.ReadAsync())
            {
                var srtNo = reader["srt_no"]?.ToString() ?? "";
                var sequence = ExtractSequenceFromSkNumber(srtNo);
                if (sequence > maxSequence)
                {
                    maxSequence = sequence;
                }
            }
            await reader.CloseAsync();
            
            return maxSequence;
        }

        private int ExtractSequenceFromSkNumber(string srtNo)
        {
            if (string.IsNullOrEmpty(srtNo) || srtNo.Length < 3)
                return 0;

            try
            {
                var sequenceStr = srtNo.Substring(0, 3);
                return int.TryParse(sequenceStr, out int sequence) ? sequence : 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private async Task<string> GenerateUniqueSkNumberAsync(SqlConnection conn, string romanMonth, int year, int startSequence)
        {
            string skFormat = $"/PA-WADIR-I/SKC/{romanMonth}/{year}";
            
            for (int attempt = 0; attempt < 100; attempt++)
            {
                string candidateSkNumber = $"{(startSequence + attempt):D3}{skFormat}";
                
                if (await IsSkNumberUniqueAsync(conn, candidateSkNumber))
                {
                    return candidateSkNumber;
                }
            }
            
            var timestamp = DateTimeOffset.Now.ToUnixTimeSeconds() % 999;
            return $"{timestamp + 500:D3}{skFormat}";
        }

        private async Task<bool> IsSkNumberUniqueAsync(SqlConnection conn, string candidateSkNumber)
        {
            var checkExistCmd = new SqlCommand(@"
                SELECT COUNT(*) 
                FROM sia_mscutiakademik 
                WHERE srt_no = @candidateSkNumber", conn);
            checkExistCmd.Parameters.AddWithValue("@candidateSkNumber", candidateSkNumber);
            
            var count = (int)(await checkExistCmd.ExecuteScalarAsync() ?? 0);
            return count == 0;
        }

        public async Task<string> DetectUserRoleAsync(string username)  
        {
            try
            {
                await using var conn = new SqlConnection(_conn);
                await conn.OpenAsync();
                
                var role = await TryDetectRoleFromStoredProcedureAsync(conn, username);
                if (!string.IsNullOrEmpty(role))
                {
                    return role;
                }
                
                return await TryDetectRoleFromDirectQueryAsync(conn, username);
            }
            catch (Exception)
            {
                return "";
            }
        }

        private async Task<string> TryDetectRoleFromStoredProcedureAsync(SqlConnection conn, string username)
        {
            try
            {
                await using var cmd = new SqlCommand("all_getIdentityByUser", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@UsernameToFind", username);
                await using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return ProcessUserRoleFromReader(reader, username);
                }
                
                return "";
            }
            catch
            {
                return "";
            }
        }

        private async Task<string> TryDetectRoleFromDirectQueryAsync(SqlConnection conn, string username)
        {
            try
            {
                await using var directCmd = new SqlCommand(@"
                    SELECT a.kry_username, a.jab_main_id, a.str_main_id, b.rol_id, a.kry_id
                    FROM ess_mskaryawan a 
                    RIGHT JOIN sso_msuser b ON a.kry_username = b.usr_id 
                    WHERE b.usr_id = @username", conn);
                directCmd.Parameters.AddWithValue("@username", username);
                
                await using var directReader = await directCmd.ExecuteReaderAsync();
                if (await directReader.ReadAsync())
                {
                    return ProcessUserRoleFromReader(directReader, username);
                }
                
                return "";
            }
            catch
            {
                return "";
            }
        }

        private string ProcessUserRoleFromReader(SqlDataReader reader, string username)
        {
            var strMainId = reader["str_main_id"]?.ToString() ?? "";
            var jabMainId = reader["jab_main_id"]?.ToString() ?? "";
            
            var role = jabMainId switch
            {
                "4" => "wadir1",                    
                "6" => "prodi",                       
                "1" when username.ToLower().Contains("finance") => "finance", 
                _ => "other"                       
            };
            
            if (username.ToLower().Equals("user_finance") && jabMainId == "1" && strMainId == "27")
            {
                role = "finance";
            }
            
            return role;
        }
            
    }
}
